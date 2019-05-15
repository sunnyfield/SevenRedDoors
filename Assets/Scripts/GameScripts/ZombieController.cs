using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : UnitScript, IZombie
{
    public LayerMask whatToHit;
    private Vector2 startPosition;

    private float seeDistance = 4f;
    [SerializeField]
    private float leftBorder;
    [SerializeField]
    private float rightBorder;
    private bool inDamageRange;


    void Start()
    {
        UnitSetup();
        StartCoroutine(PassiveBehavior());
        maxSpeed = 1.5f;
        healthPoints = 3;
        startPosition = transform.position;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        float size, offset;
        float threshold = 0.5f;

        base.OnCollisionEnter2D(collision);
        if(collision.gameObject.CompareTag("Ground"))
        {
            size = collision.gameObject.GetComponent<BoxCollider2D>().size.x;
            offset = collision.gameObject.GetComponent<BoxCollider2D>().offset.x;
            leftBorder = (-size / 2) + collision.gameObject.transform.position.x + offset + threshold;
            rightBorder = (size / 2) + collision.gameObject.transform.position.x + offset - threshold;
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.position = startPosition;
        }
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
        float attackDistance = 0.6f;

        if (transform.right.x * direction.x < 0)
            sideHorizontal = -sideHorizontal;

        while (distance < seeDistance)
        {
            isAttack = anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot");

            if (distance <= attackDistance && !isAttack)
            {
                sideHorizontal = 0;
                if (direction.x * transform.right.x < 0)
                    Flip();
                anim.SetTrigger("shoot");
                yield return new WaitForSeconds(1.3f); //get rest after attack
            }
            else if(!isAttack)
            {
                sideHorizontal = (int)transform.right.x;

                if (direction.x * sideHorizontal < 0)
                    sideHorizontal = -sideHorizontal;
                else if ((transform.position.x < leftBorder) && sideHorizontal < 0)
                    sideHorizontal = 0;
                else if ((transform.position.x > rightBorder) && sideHorizontal > 0)
                    sideHorizontal = 0;
            }

            yield return null;

            direction = VectorToPlayer();
            distance = direction.magnitude;
        } //while
    }

    private void AttackPlayer ()
    {
        inDamageRange  = Physics2D.OverlapCircle(firePoint.position, 0.5f, whatToHit);
        if (inDamageRange)
            PlayerController.instance.TakeDamage();
    }

    public override void Death()
    {
        if (healthPoints != 0)
        {
            healthPoints = 0;
            anim.enabled = false;
            enableMovement = false;
            unitsSprite.enabled = false;
            Pool.Pull(Group.VFX_BloodExplosion, transform.position, Quaternion.identity, 1.5f);
            Pool.Pull(Group.VFX_Meat, transform.position - new Vector3(0.1f, 0.56f, 0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
