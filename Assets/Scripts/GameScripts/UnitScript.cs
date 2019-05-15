using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitScript : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rigidBodyUnit2d;
    protected Transform firePoint;
    protected SpriteRenderer unitsSprite;
    public LayerMask whatIsGround;
    private Transform groundCheckPoint;
    private float overrlapRadius = 0.08f;
    protected Coroutine takeDamageBlinkingRoutine = null;



    protected float maxSpeed = 4.5f;
    protected float lerpSpeed = 20f;
    //[HideInInspector]
    public int sideHorizontal;

    protected byte healthPoints;
    [SerializeField]
    protected bool grounded = false;
    [SerializeField]
    public bool enableMovement = true;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
    //    Gizmos.DrawSphere(groundCheckLeft.position, overrlapRadius);
    //}

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
            Invoke("GroundCheck", 0.1f);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
            Invoke("GroundCheck", 0.1f);
    }

    protected virtual void UnitSetup()
    {
        unitsSprite = GetComponent<SpriteRenderer>();
        rigidBodyUnit2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

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
        Vector2 moveVector;
        if (enableMovement)
        {
            if (transform.right.x * sideHorizontal < 0)
                Flip();

            moveVector.x = (grounded) ? (sideHorizontal * maxSpeed) : (sideHorizontal * (maxSpeed * 0.95f));
            moveVector.y = rigidBodyUnit2d.velocity.y;

            if (!grounded)
                rigidBodyUnit2d.drag = 1f;
            else if (Mathf.Abs(moveVector.x) > 0 || moveVector.y > 0)
                rigidBodyUnit2d.drag = 1f;
            else if (Mathf.Abs(moveVector.x) <= 0.001)
                rigidBodyUnit2d.drag = 1000000f;

            rigidBodyUnit2d.velocity = moveVector;
            anim.SetInteger("speedH", Mathf.Abs(sideHorizontal));
        }
        else
        {
            //rigidBodyUnit2d.velocity = Vector2.zero;
            anim.SetInteger("speedH", 0);
        }
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f, Space.Self);
    }

    private void GroundCheck()
    {
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, overrlapRadius, whatIsGround);
        anim.SetBool("ground", grounded);
    }

    /*private void FlipOld()
    {
    if (sideHorizontal == 0)
        return;

    if (sideHorizontal < 0)
    {
        unitsSprite.flipX = true;
        firePoint.localPosition = new Vector2(-1.19f, firePoint.localPosition.y);
        firePoint.eulerAngles = 180f * Vector3.up;
        gunCaseEmiter.eulerAngles = new Vector2(0f, 180f);
    }
    else
    {
        unitsSprite.flipX = false;
        firePoint.localPosition = new Vector2(1.19f, firePoint.localPosition.y);
        firePoint.eulerAngles = 0f * Vector3.up;
        gunCaseEmiter.eulerAngles = new Vector2(0f, 0f);
    }

    }*/

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
}
