using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZombie
{
    void TakeDamage();
}

public class ZombieBigController : UnitScript, IZombie
{
    public LayerMask whatToHit;
    private Vector2 startPosition;
    [SerializeField]
    private ParticleSystem bloodExplosion;
    [SerializeField]
    private GameObject meat;
    private float seeDistance;
    [SerializeField]
    private float leftBorder;
    [SerializeField]
    private float rightBorder;
    private bool inDamageRange;


    void Start()
    {
        UnitSetup();
        meat = transform.GetChild(4).gameObject;
        meat.SetActive(false);
        StartCoroutine(PassiveBehavior());
        maxSpeed = 1.5f;
        healthPoints = 6;
        seeDistance = 5f;
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

    private Vector2 VectorToPlayer(ref Vector3 playerPosition)
    {
        playerPosition = PlayerController.instance.transform.position;
        Vector2 directionToPlayer = playerPosition - transform.position;    
        return directionToPlayer;
    }

    /*private IEnumerator AIcontrol()
    {
        Vector2 directionToPlayer;
        float distanceToPlayer;
        sideHorizontal = 1;

        while (gameObject.activeInHierarchy)
        {
            directionToPlayer = VectorToPlayer();
            distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer < seeDistance)
                    yield return StartCoroutine(AgressiveBehavior(directionToPlayer, distanceToPlayer));
            else
                    yield return StartCoroutine(PassiveBehavior(directionToPlayer, distanceToPlayer));
        } //while
    }*/

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
                    sideHorizontal = (int)transform.right.x;
                    timer = Random.Range(2f, 3.5f); //rest timer reset
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
        Vector3 playerPosition = Vector3.zero;
        bool isAttack = false;
        float attackDistance = 0.6f;
        float minRushDistance = 2f;

        if (transform.right.x * direction.x < 0)
            sideHorizontal = -sideHorizontal;

        while (distance < seeDistance)
        {
            if (distance >= minRushDistance && playerPosition.x < rightBorder && playerPosition.x > leftBorder && !isAttack)
            {
                isAttack = true;
                sideHorizontal = (int)transform.right.x * 3;
                if (direction.x * sideHorizontal < 0)
                    sideHorizontal = -sideHorizontal;

                yield return StartCoroutine(RushAttack(playerPosition.x));
                yield return new WaitForSeconds(1.3f);
                isAttack = false;
            }else if (distance <= attackDistance && !isAttack)
            {
                isAttack = true;
                sideHorizontal = 0;
                if (direction.x * transform.right.x < 0)
                    Flip();
                anim.SetTrigger("shoot");
                yield return new WaitForSeconds(1f); //get rest after attack
                isAttack = false;
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

            direction = VectorToPlayer(ref playerPosition);
            //direction = VectorToPlayer();
            distance = direction.magnitude;
        } //while
    }

    private IEnumerator RushAttack (float playerPositionHorizontal)
    {
        gameObject.layer = 15;
        float distanceToRush = Mathf.Abs(playerPositionHorizontal - transform.position.x);
        while (distanceToRush >= 2.3f)
        {
                yield return null;
                distanceToRush = Mathf.Abs(playerPositionHorizontal - transform.position.x);
        }

        anim.SetBool("rush", true);
        while (distanceToRush >= 0.3f)
        {
            yield return null;
            distanceToRush = Mathf.Abs(playerPositionHorizontal - transform.position.x);
        }
        sideHorizontal = 0;
        anim.SetBool("rush", false);
        gameObject.layer = 14;
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
            bloodExplosion = ObjectPooler.instance.GetPooledObject("VFX_BloodExplosion").GetComponent<ParticleSystem>();
            bloodExplosion.transform.position = transform.position;
            unitsSprite.enabled = false;
            meat.SetActive(true);
            bloodExplosion.gameObject.SetActive(true);
            Invoke("BloodExplosionReset", 1.5f);
            Destroy(gameObject, 2f);
        }
    }

    private void BloodExplosionReset()
    {
        bloodExplosion.gameObject.SetActive(false);
    }
}
