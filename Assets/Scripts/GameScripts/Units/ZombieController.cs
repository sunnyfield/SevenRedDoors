using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : UnitScript
{
    public LayerMask whatToHit;
    private Vector2 startPosition;
    GameObject ground;

    private float seeDistance = 4f;
    [HideInInspector]
    public float leftBorder = -100f;
    [HideInInspector]
    public float rightBorder = 100f;
    private bool inDamageRange;

    [HideInInspector]
    public AIBehavior behavior = AIBehavior.PASSIVE;
    [HideInInspector]
    public float restRaitTimer;
    [HideInInspector]
    public float restTimer;

    private IZombieState zombieState;
    public readonly Idle idleState = new Idle();
    public readonly Run runState = new Run();
    public readonly Attack attackState = new Attack();

    void Start()
    {
        UnitSetup();
        maxSpeed = 1.5f;
        healthPoints = 3;
        startPosition = transform.position;
        zombieState = idleState;   
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
                if ((collision.contacts[0].point - (Vector2)transform.position).y < 0)
                {
                    if (sideHorizontal == -1) leftBorder = collision.contacts[0].point.x + threshold;
                    else if (sideHorizontal == 1) rightBorder = collision.contacts[0].point.x - threshold;
                }
            }
        } 
    }

    protected void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject == ground) transform.position = startPosition; }

    private void Update() { zombieState.StateUpdate(this); }  

    private void FixedUpdate() { Move(); }

    private Vector2 VectorToPlayer() { return PlayerController.instance.transform.position - transform.position; }

    public void Attack()
    {
        var target = Physics2D.OverlapCircle(firePoint.position, 0.5f, whatToHit);
        if (target != null) target.GetComponent<ICanDie>().TakeDamage();
    }

    public override void Death()
    {
        if (healthPoints != 0)
        {
            healthPoints = 0;     
            Pool.Pull(Group.VFX_BloodExplosion, transform.position, Quaternion.identity, 1.5f);
            Pool.Pull(Group.VFX_Meat, transform.position - new Vector3(0.1f, 0.56f, 0f), Quaternion.identity);
            AIManager.zombies.Remove(this);
            Destroy(gameObject);
        }
    }

    public void SetIdleState()
    {
        zombieState = idleState;
        zombieState.OnEnter(this);
    }

    public void SetRunState()
    {
        zombieState = runState;
        zombieState.OnEnter(this);
    }

    public void HandleInput(MoveInput move, ActionInput action)
    {
        IZombieState state = zombieState.HandleInput(this, move, action);
        if (state != null)
        {
            zombieState = state;
            zombieState.OnEnter(this, move, action);
        }
    }
}
