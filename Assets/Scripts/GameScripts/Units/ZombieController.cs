using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : UnitScript
{
    public LayerMask whatToHit;
    private Vector2 startPosition;
    GameObject ground;

    private float seeDistance = 4f;
    [SerializeField]
    public float leftBorder;
    [SerializeField]
    public float rightBorder;
    private bool inDamageRange;

    public AIBehavior behavior = AIBehavior.PASSIVE;
    public float restRaitTimer;
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
        //StartCoroutine(PassiveBehavior());    
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        float size, offset;
        float threshold = 0.5f;

        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            ground = collision.gameObject;
            size = ground.GetComponent<BoxCollider2D>().size.x;
            offset = ground.GetComponent<BoxCollider2D>().offset.x;
            leftBorder = (-size / 2) + ground.transform.position.x + offset + threshold;
            rightBorder = (size / 2) + ground.transform.position.x + offset - threshold;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject == ground) transform.position = startPosition; }

    private void Update() { zombieState.StateUpdate(this); }  

    private void FixedUpdate() { Move(); }

    private Vector2 VectorToPlayer() { return PlayerController.instance.transform.position - transform.position; }

    private IEnumerator PassiveBehavior()
    {
        Vector2 directionToPlayer;
        float distanceToPlayer;

        directionToPlayer = VectorToPlayer();
        distanceToPlayer = directionToPlayer.magnitude;

        float timer = Random.Range(1.5f, 3f); //rest timer init
        HandleInput(MoveInput.RIGHT, ActionInput.NONE);

        while (gameObject.activeInHierarchy)
        {
            directionToPlayer = VectorToPlayer();
            distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer >= seeDistance)
            {
                if (timer > 0f)
                {
                    if (transform.position.x < leftBorder) HandleInput(MoveInput.RIGHT, ActionInput.NONE);
                    else if (transform.position.x > rightBorder) HandleInput(MoveInput.LEFT, ActionInput.NONE);
                }
                else
                {
                    HandleInput(MoveInput.NONE, ActionInput.NONE);
                    yield return new WaitForSeconds(Random.Range(0.8f, 1.3f)); //get rest
                    HandleInput((MoveInput)(int)(-transform.right.x), ActionInput.NONE);
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
        float attackDistance = 0.6f;

        if (transform.right.x * direction.x < 0) HandleInput((MoveInput)(-sideHorizontal), ActionInput.NONE);

        while (distance < seeDistance)
        {
            if (distance <= attackDistance)
            {
                if (direction.x * transform.right.x < 0) Flip();

                HandleInput(MoveInput.NONE, ActionInput.FIRE);
            }
            else
            {
                HandleInput((MoveInput)(int)transform.right.x, ActionInput.NONE);

                if (direction.x * sideHorizontal < 0) HandleInput((MoveInput)(-sideHorizontal), ActionInput.NONE);
                else if ((transform.position.x < leftBorder) && sideHorizontal < 0) HandleInput(MoveInput.NONE, ActionInput.NONE);
                else if ((transform.position.x > rightBorder) && sideHorizontal > 0) HandleInput(MoveInput.NONE, ActionInput.NONE);
            }

            yield return null;

            direction = VectorToPlayer();
            distance = direction.magnitude;
        } //while
    }

    public void Attack() { Physics2D.OverlapCircle(firePoint.position, 0.5f, whatToHit).GetComponent<ICanDie>().TakeDamage(); }

    public override void Death()
    {
        if (healthPoints != 0)
        {
            healthPoints = 0;     
            Pool.Pull(Group.VFX_BloodExplosion, transform.position, Quaternion.identity, 1.5f);
            Pool.Pull(Group.VFX_Meat, transform.position - new Vector3(0.1f, 0.56f, 0f), Quaternion.identity);
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
