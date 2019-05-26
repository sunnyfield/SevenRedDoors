using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : AIUnit
{ 
    private Vector2 startPosition;
    private GameObject ground;

    [HideInInspector]
    public float yThreshold = 0.65f;
    private bool inDamageRange;
    

    [HideInInspector]
    public IBehavior behaviorState;

    void Start()
    {
        UnitSetup();
        leftBorder = -100f;
        rightBorder = 100f;
        maxSpeed = 1.3f;
        healthPoints = 3;
        seeDistance = 5f;
        attackDistance = 1f;
        startPosition = transform.localPosition;
        SetState(idleState);
        behaviorState = passiveBehavior;
        behaviorState.OnEnter(this);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        float size, offset;
        float threshold = 0.5f;

        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            if (collision.contacts[0].normal.y > 0.35f)
            {
                ground = collision.gameObject;
                size = ground.GetComponent<BoxCollider2D>().size.x;
                offset = ground.GetComponent<BoxCollider2D>().offset.x;
                leftBorder = (-size / 2) + ground.transform.position.x + offset + threshold;
                rightBorder = (size / 2) + ground.transform.position.x + offset - threshold;
            }
            else
            {
                if ((collision.contacts[0].point - (Vector2)transform.localPosition).y < 0)
                {
                    if (sideHorizontal == -1) leftBorder = collision.contacts[0].point.x + threshold;
                    else if (sideHorizontal == 1) rightBorder = collision.contacts[0].point.x - threshold;
                }
            }
        } 
    }

    protected void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject == ground) transform.localPosition = startPosition; }

    private void FixedUpdate()
    {
        AIUpdate();
        unitState.StateUpdate(this);
        Move();
    }

    private Vector2 VectorToPlayer() { return PlayerController.instance.transform.localPosition - transform.localPosition; }

    public void Attack()
    {
        var target = Physics2D.OverlapCircle(firePoint.position, 0.5f, whatToHit);
        if (target != null) target.GetComponent<ICanDie>().TakeDamage();
    }

    public void SetFollowSpeed()
    {
        anim.SetFloat("runSpeed", 1.6f);
        maxSpeed = 2.2f;
    }
    public void SetPassiveSpeed()
    {
        anim.SetFloat("runSpeed", 1.0f);
        maxSpeed = 1.3f;
    }

    public override void Death()
    {
        if (healthPoints != 0)
        {
            healthPoints = 0;     
            Pool.Pull(Group.VFX_BloodExplosion, transform.localPosition, Quaternion.identity, 1.5f);
            Pool.Pull(Group.VFX_Meat, transform.localPosition - new Vector3(0.1f, 0.56f, 0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void AIUpdate()
    {
        IBehavior state = behaviorState.StateUpdate(this);
        if (state != null)
        {
            behaviorState = state;
            behaviorState.OnEnter(this);
        }
    }

    public void SetState(IState state)
    {
        unitState = state;
        unitState.OnEnter(this);
    }

    public void HandleInput(MoveInput move, ActionInput action)
    {
        IState state = unitState.HandleInput(this, move, action);
        if (state != null)
        {
            unitState = state;
            unitState.OnEnter(this, move, action);
        }
    }
}
