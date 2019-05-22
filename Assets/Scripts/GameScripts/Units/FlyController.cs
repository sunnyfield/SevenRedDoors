using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : UnitScript, IZombie
{
    public LayerMask whatToHit;
    private Vector2 startPosition;
    private ParticleSystem bloodExplosion;
    private GameObject projectileClone;
    private float seeDistance = 10f;

    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;

    private int sideVertical = 0;
    private bool inDamageRange;


    void Start()
    {
        UnitSetup();
        StartCoroutine(PassiveBehavior());
        maxSpeed = 1.5f;
        healthPoints = 1;
        startPosition = transform.position;
        topBorder = Mathf.Infinity;
        bottomBorder = -Mathf.Infinity;
        //leftBorder = -Mathf.Infinity;
        //rightBorder = Mathf.Infinity;
    }

    protected override void UnitSetup()
    {
        unitsSprite = GetComponent<SpriteRenderer>();
        rigidBodyUnit2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        firePoint = transform.GetChild(0);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    { }


    protected override void OnCollisionExit2D(Collision2D collision)
    { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Flip"))
        //{
        //    sideHorizontal = -sideHorizontal;
        //    if(transform.right.x > 0)
        //        rightBorder = collision.transform.position.x;
        //    else
        //        leftBorder = collision.transform.position.x;

        //    Destroy(collision.gameObject);
        //}
    }

    private void FixedUpdate()
    {
        Move();
    }

    private Vector2 VectorToPlayer()
    {
        Vector2 directionToPlayer = PlayerController.instance.transform.position - transform.position;
        return directionToPlayer;
    }

    private IEnumerator PassiveBehavior()
    {
        Vector2 directionToPlayer;
        float distanceToPlayer;
        sideHorizontal = 1;

        directionToPlayer = VectorToPlayer();
        distanceToPlayer = directionToPlayer.magnitude;

        float timer = Random.Range(1.5f, 3f); //rest timer init
        sideHorizontal = 1;

        while (gameObject.activeInHierarchy)
        {
            directionToPlayer = VectorToPlayer();
            distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer >= seeDistance)
            {
                if (timer > 0f)
                {
                    if (transform.position.x < leftBorder)
                        sideHorizontal = 1;
                    else if (transform.position.x > rightBorder)
                        sideHorizontal = -1;
                }
                else
                {
                    sideHorizontal = 0;
                    yield return new WaitForSeconds(Random.Range(0.8f, 1.3f)); //get rest
                    sideHorizontal = -(int)transform.right.x;
                    timer = Random.Range(1.5f, 3f); //rest timer reset
                }
                timer -= Time.deltaTime;
            }
            else
            {
                yield return StartCoroutine(AgressiveBehavior(directionToPlayer, distanceToPlayer));
            }

            yield return null;

        } //while
    }

    private IEnumerator AgressiveBehavior(Vector2 direction, float distance)
    {
        bool isAttack;
        float attackDistance = 5f;
        float shootDistance = attackDistance + 2f;
        float fireRate = 0.5f;
        float timer = 0f;

        if (transform.right.x * direction.x < 0)
            sideHorizontal = -sideHorizontal;

        sideVertical = (int)Mathf.Sign(direction.y - transform.position.y);

        while (direction.x < seeDistance)
        {
            isAttack = anim.GetCurrentAnimatorStateInfo(0).IsName("Fire_Fly");

            if (Mathf.Abs(direction.x) <= shootDistance && !isAttack)
            {
                timer += Time.deltaTime;

                if (timer >= fireRate)
                {
                    anim.SetTrigger("shoot");
                    
                    Pool.Pull(Group.Fireball, firePoint.position, firePoint.rotation);
                    timer = 0;
                }
            }

            if (Mathf.Abs(direction.x) >= attackDistance)
            {
                sideHorizontal = (int)transform.right.x;
                if (direction.x * sideHorizontal < 0)
                   sideHorizontal = -sideHorizontal;
                else if ((transform.position.x < leftBorder) && sideHorizontal < 0)
                   sideHorizontal = 0;
                else if ((transform.position.x > rightBorder) && sideHorizontal > 0)
                   sideHorizontal = 0;
            }
            else
                sideHorizontal = 0;

            if (Mathf.Abs(direction.y) > 0.2f)
            {
                sideVertical = -(int)transform.up.y;
                if (direction.y * sideVertical < 0)
                   sideVertical = -sideVertical;
                else if ((transform.position.y > topBorder) && sideVertical > 0)
                   sideVertical = 0;
                else if ((transform.position.y < bottomBorder) && sideVertical < 0)
                   sideVertical = 0;
            }
            else
                sideVertical = 0;

            yield return null;

            direction = VectorToPlayer();
        } //while
    }

    private void AttackPlayer ()
    {
        inDamageRange  = Physics2D.OverlapCircle(firePoint.position, 0.5f, whatToHit);
        if (inDamageRange)
            PlayerController.instance.TakeDamage();
    }

    protected override void Move()
    {
        Vector2 moveVector;
        if (enableMovement)
        {
            if (transform.right.x * sideHorizontal < 0)
                Flip();

            moveVector.x = sideHorizontal * maxSpeed;
            moveVector.y = sideVertical * maxSpeed;

            rigidBodyUnit2d.velocity = moveVector;
            anim.SetInteger("speedH", Mathf.Max(Mathf.Abs(sideHorizontal), Mathf.Abs(sideVertical)));
        }
        else
        {
            //rigidBodyUnit2d.velocity = Vector2.zero;
            anim.SetInteger("speedH", 0);
        }
    }

    private void MoveVertical()
    {
        Vector2 moveVector;
        if (enableMovement)
        {

            moveVector.x = rigidBodyUnit2d.velocity.x;
            moveVector.y = sideVertical * maxSpeed;

            rigidBodyUnit2d.velocity = moveVector;
            anim.SetInteger("speedH", Mathf.Abs(sideVertical));
        }
        else
        {
            //rigidBodyUnit2d.velocity = Vector2.zero;
            anim.SetInteger("speedH", 0);
        }
    }

    public override void Death()
    {
        if (healthPoints != 0)
        {
            healthPoints = 0;
            Pool.Pull(Group.VFX_BloodExplosion, transform.position, Quaternion.identity, 1.5f);
            Destroy(gameObject);
        }
    }
}
