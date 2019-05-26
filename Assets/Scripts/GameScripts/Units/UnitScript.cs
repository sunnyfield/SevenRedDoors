using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICanDie
{
    void TakeDamage();
}

public class UnitScript : MonoBehaviour, ICanDie
{
    protected Animator anim;
    protected Rigidbody2D rigidBodyUnit2d;
    protected Transform firePoint;
    protected SpriteRenderer unitsSprite;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;
    public float overrlapRadius = 0.03f;
    protected Coroutine takeDamageBlinkingRoutine = null;
    protected Vector2 moveVector = Vector2.zero;

    protected IState unitState;

    public readonly Idle idleState = new Idle();
    public readonly Run runState = new Run();
    public readonly Jump jumpState = new Jump();
    public readonly Attack attackState = new Attack();
    public readonly Reload reloadState = new Reload();
    public readonly Bounce bounceState = new Bounce();

    protected float moveIncrement = 0.1f;

    protected float maxSpeed = 4.5f;
    protected float lerpSpeed = 20f;
    //[HideInInspector]
    public int sideHorizontal;

    protected byte healthPoints;

    [SerializeField]
    public bool enableMovement = true;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
    //    Gizmos.DrawSphere(groundCheckLeft.position, overrlapRadius);
    //}

    protected virtual void UnitSetup()
    {
        unitsSprite = GetComponentInChildren<SpriteRenderer>();
        rigidBodyUnit2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        groundCheckPoint = transform.GetChild(1);
        firePoint = transform.GetChild(0);
    }

    public virtual void TakeDamage()
    {
        if (healthPoints - 1 > 0)
        {
            healthPoints--;
            SingleRoutineStart(ref takeDamageBlinkingRoutine, TakeDamageBlinking(1));
        }
        else Death();
    }

    public virtual void Death()
    {
        healthPoints = 0;
        Destroy(gameObject);
    }


    protected void SingleRoutineStart(ref Coroutine routineRef, IEnumerator routine)
    {
        if (routineRef == null)
            routineRef = StartCoroutine(routine);
        else
        {
            StopCoroutine(routineRef);
            routineRef = StartCoroutine(routine);
        }
    }

    protected IEnumerator TakeDamageBlinking(int count)
    {
        int i = count;
        unitsSprite.color = Color.white;
        while (i > 0)
        {
            while (unitsSprite.color.g >= 0.01)
            {
                unitsSprite.color = Color.Lerp(unitsSprite.color, Color.red, Time.deltaTime * lerpSpeed);
                yield return null;
            }
            unitsSprite.color = Color.red;

            while (unitsSprite.color.g <= 0.8)
            {
                unitsSprite.color = Color.Lerp(unitsSprite.color, Color.white, Time.deltaTime * lerpSpeed);
                yield return null;
            }
            unitsSprite.color = Color.white;

            i--;
        }
        unitsSprite.color = Color.white;
        takeDamageBlinkingRoutine = null;
    }

    protected virtual void Move()
    {  
        if (transform.right.x * sideHorizontal < 0) Flip();

        if (sideHorizontal != 0)
        {
            if (Mathf.Abs(moveVector.x) < Mathf.Abs(sideHorizontal * maxSpeed))
            {
                moveVector.x += moveIncrement * sideHorizontal;
                moveIncrement *= 2f;
            }
            else moveVector.x = sideHorizontal * maxSpeed;
        }
        else
        {
            moveVector.x = 0f;
            moveIncrement = 0.1f;
        }
        moveVector.y = rigidBodyUnit2d.velocity.y;

        rigidBodyUnit2d.velocity = moveVector;
    }

    public void Flip()
    {
        var temp = transform.localPosition;
        transform.localRotation *= Quaternion.Euler(Vector3.up * 180);
        transform.localPosition = temp;
        //transform.Rotate(0f, 180f, 0f);
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, -groundCheckPoint.up, overrlapRadius, whatIsGround);
        //if (hit && hit.distance <= 0.01f)
        //    grounded = true;
        //else
        //    grounded = false;
    }

    public void MoveRight()
    {
        sideHorizontal = 1;
    }

    public void MoveLeft()
    {
        sideHorizontal = -1;
    }

    public void Stop()
    {
        sideHorizontal = 0;
    }

    public void SetDrag(float drag) { rigidBodyUnit2d.drag = drag; }

    public void SetAnimation(int state) { anim.SetInteger("STATE", state); } 
}
