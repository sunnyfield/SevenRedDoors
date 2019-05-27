using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum Layer
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Water = 4,
    UI,
    Player = 8,
    Ground,
    Background,
    RedDoor,
    Boxes,
    Interactive,
    IgnoreZombie,
    Traps,
    FireBall,
    Zombie,
    BottomLevelLimit,
    Coins,
    Health,
    Key,
    CheckPoint
}

public enum AnimationState
{
    IDLE,
    RUN,
    JUMP,
    ATTACK,
    RELOAD,
    BOUNCE
}

public enum MoveInput
{ 
    LEFT = -1,
    NONE,
    RIGHT
}

public enum ActionInput
{
    FIRE,
    JUMP,
    RELOAD,
    ACTIVATE,
    NONE
}

public class PlayerController : UnitScript
{
    public static PlayerController instance;

    public LayerMask bounceLayers;
    private CameraFollow cameraFollow;
    private Transform reloadFlag;
    private Transform reloadArrowIcon;
    private ProjectilesMove projectileClone;
    private ParticleSystem gunCaseParticleSystem;
    private Transform startPoint;
    [SerializeField]
    private Vector3 respawnPosition;
    private GameObject boxRef;
    public InteractiveTrigger trigger;
    [HideInInspector]
    private List<GameObject> currentGround = new List<GameObject>();

    public LayerMask whatToHit;

    private const float jumpForce = 10.5f;
    private const int blinkCount = 8;

    private uint ammo = 5;
    private const uint maxAmmo = 5;
    private const byte maxHealth = 3;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UnitSetup();

        currentGround.Add(null);

        gunCaseParticleSystem = transform.GetChild(3).GetComponent<ParticleSystem>();

        reloadFlag = transform.GetChild(2);
        reloadArrowIcon = reloadFlag.GetChild(0);
        reloadFlag.gameObject.SetActive(false);

        startPoint = GameObject.Find("/Scene/StartPosition").GetComponent<Transform>();

        healthPoints = maxHealth;
        lerpSpeed = 40f;

        cameraFollow = CameraFollow.instance;

        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        respawnPosition = startPoint.position;

        SetState(jumpState);
    }

    private void FixedUpdate()
    {
        Move();
        unitState.StateUpdate(this);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            if (collision.contacts[0].normal.y > 0.35f)
            {
                if(currentGround[0] == null)
                    currentGround[0] = collision.gameObject;
                else
                    currentGround.Add(collision.gameObject);

                if (unitState != bounceState)
                {
                    if (sideHorizontal == 0) SetState(idleState);
                    else SetState(runState);
                }
            }
        }
        else if (bounceLayers == (bounceLayers | 1 << collision.gameObject.layer))
        {
            TakeDamage();
            Bounce(collision);
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        Profiler.BeginSample("Collision exit check");
        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            if (currentGround.Count == 1 && currentGround[0] == collision.gameObject)
            {
                currentGround[0] = null;
                if(unitState != bounceState) SetState(jumpState);
            }
            else currentGround.Remove(collision.gameObject);       
        }
        Profiler.EndSample();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.layer)
        {
            case (int)Layer.Traps:
                TakeDamage();
                Bounce(collision);
                break;
            case (int)Layer.Coins:
                PickUpCoin(collision.gameObject);
                break;
            case (int)Layer.Health:
                PickUpHealth(collision.gameObject);
                break;
            case (int)Layer.Key:
                PickUpKey(collision.gameObject);
                break;
            case (int)Layer.RedDoor:
                GameController.instance.GameWinCheck();
                break;
            case (int)Layer.BottomLevelLimit:
                TakeDamage();
                ResetPosition();
                break;
            case (int)Layer.CheckPoint:
                respawnPosition = collision.gameObject.transform.GetChild(0).position;
                break;
            default:
                break;
        }
    }

    public void ReloadAnimationStart() { StartCoroutine(ReloadAnimation()); }

    public bool AddAmmo()
    {
        if(ammo < maxAmmo)
        {
            ammo++;
            UIController.instance.AmmoBarIncrease();
            return true;
        }
        return false;
    }

    public bool TakeAmmo()
    {
        if(ammo > 0)
        {
            ammo--;
            UIController.instance.AmmoBarDecrease();
            return true;
        }
        return false;
    }

    public bool IsGrounded()
    {
        if (currentGround[0] != null) return true;
        return false;
    }

    public void Jump() { rigidBodyUnit2d.velocity += Vector2.up * jumpForce; }

    private void Bounce(Collision2D collision)
    {
        SetState(bounceState);
        ContactPoint2D point = collision.contacts[0];
        Rigidbody2D rb2d = collision.gameObject.GetComponent<Rigidbody2D>();
        rigidBodyUnit2d.velocity += new Vector2(point.normal.x * 10f, rigidBodyUnit2d.velocity.y);
    }

    private void Bounce(Collider2D collider)
    {
        SetState(bounceState);
        Rigidbody2D rb2d = collider.GetComponent<Rigidbody2D>();

        if (rb2d != null) { rigidBodyUnit2d.velocity = new Vector2(rb2d.velocity.x * 2.5f, rigidBodyUnit2d.velocity.y); }
        else
        {
            float x;
            x = rigidBodyUnit2d.velocity.x;
            if (Mathf.Abs(x) > 0f) rigidBodyUnit2d.velocity += new Vector2(-Mathf.Sign(x) * (x / x) * 8f, rigidBodyUnit2d.velocity.y);
        }
    }

    private void ResetPosition()
    {
        rigidBodyUnit2d.velocity = Vector2.zero;
        transform.position = respawnPosition;       
    }

    public void Shoot()
    {
        float projectileDist = 0f;
        float fireRange = cameraFollow.camLenght / 2f + firePoint.right.x * (cameraFollow.transform.position.x - transform.position.x) - 0.4f;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, fireRange, whatToHit);
        if (hit)
        {
            if (hit.transform.CompareTag("Boxes"))
            {
                    boxRef = hit.transform.gameObject;
                    Invoke("BoxDestroy", hit.distance / 60f);
            }
            else if (hit.transform.CompareTag("Zombie")) hit.transform.GetComponent<ICanDie>().TakeDamage();

            projectileDist = hit.distance;
        }
        else projectileDist = fireRange;

        CameraFollow.instance.Recoil();

        projectileClone = Pool.Pull(Group.Projectile, firePoint.position, firePoint.rotation).GetComponent<ProjectilesMove>();
        if(projectileClone != null) projectileClone.maxExistDistance = projectileDist;
            
        gunCaseParticleSystem.Emit(1);
    }

    private void BoxDestroy()
    {
        Pool.Pull(Group.VFX_BoxCrush, boxRef.transform.position, Quaternion.identity, 1f);
        Destroy(boxRef);
    }

    private IEnumerator ReloadAnimation()
    {
        float rotationSpeed = 150f;
        float scaleSpeed = 0.6f;
        Vector3 rotationAngle = new Vector3(0f, 0f, 1f);
        Vector3 scaleIncrement = new Vector3(1f, 1f, 0f);

        if (ammo < 5)
        {
            reloadFlag.gameObject.SetActive(true);

            while (unitState == reloadState && ammo < 5)
            {
                if (reloadFlag.localScale.x >= 1.2)
                    scaleSpeed = -0.6f;
                else if (reloadFlag.localScale.x <= 1)
                    scaleSpeed = 0.6f;

                reloadFlag.localScale += scaleIncrement * Time.fixedDeltaTime * scaleSpeed;
                reloadArrowIcon.Rotate(0f, 0f, 1f * Time.fixedDeltaTime * rotationSpeed);
                yield return new WaitForFixedUpdate();
            }

            reloadFlag.localScale = new Vector3(1f, 1f, 1f);
            reloadArrowIcon.localRotation = Quaternion.identity;
            reloadFlag.gameObject.SetActive(false);
        }
    }

    private void PickUpCoin(GameObject coin)
    {
        GameController.instance.CoinIncrease();
        Destroy(coin);
    }

    private void PickUpKey(GameObject key)
    {
        GameController.instance.GetKey();
        Destroy(key);
    }

    private void PickUpHealth(GameObject health)
    {
        if (healthPoints < 3)
        {
            healthPoints++;
            UIController.instance.HealthBarIncrease();
            Destroy(health);
        }   
    }

    public override void TakeDamage()
    {
        if (healthPoints - 1 > 0)
        {
            healthPoints--;
            UIController.instance.HealthBarDecrease();
            SingleRoutineStart(ref takeDamageBlinkingRoutine, TakeDamageBlinking(blinkCount));
        }
        else Death();
    }
    
    public override void Death()
    {
        healthPoints = 0;
        gameObject.SetActive(false);
        GameController.instance.GameOver();
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
