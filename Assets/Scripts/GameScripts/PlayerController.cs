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

public enum State
{
    IDLE,
    RUN,
    JUMP,
    ATTACK,
    RELOAD,
    BOUNCE
}

public class PlayerController : UnitScript
{
    public static PlayerController instance;

    private CameraFollow cameraFollow;
    private Transform reloadFlag;
    private Transform reloadArrowIcon;
    private ProjectilesMove projectileClone;
    private ParticleSystem gunCaseParticleSystem;
    private Transform startPoint;
    [SerializeField]
    private Vector3 respawnPosition;
    private Coroutine bounceDelayRoutine = null;
    private GameObject boxRef;
    [SerializeField]
    private List<GameObject> currentGround = new List<GameObject>();

    public LayerMask whatToHit;

    public State state = State.JUMP;

    private float jumpForce = 10.5f;
    private int blinkCount = 8;

    private uint ammo = 5;
    private uint maxAmmo = 5;
    private float reloadTime = 1f;
    private float reloadTimer = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AnimationClip clip;
        AnimationEvent evt;

        UnitSetup();
        clip = anim.runtimeAnimatorController.animationClips[2];
        evt = new AnimationEvent();
        evt.time = 0.35f;
        evt.functionName = "SetIdleState";
        clip.AddEvent(evt);

        currentGround.Add(null);
        gunCaseParticleSystem = transform.GetChild(3).GetComponent<ParticleSystem>();

        reloadFlag = transform.GetChild(2);
        reloadArrowIcon = reloadFlag.GetChild(0);
        reloadFlag.gameObject.SetActive(false);

        startPoint = GameObject.Find("/Scene/StartPosition").GetComponent<Transform>();

        healthPoints = 3;
        lerpSpeed = 40f;

        cameraFollow = CameraFollow.instance;
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        respawnPosition = startPoint.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Profiler.BeginSample("Collision enter check");
        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            if (collision.contacts[0].normal.y > 0.2f)
            {
                if(currentGround[0] == null)
                    currentGround[0] = collision.gameObject;
                else
                    currentGround.Add(collision.gameObject);

                if (sideHorizontal == 0)
                    state = State.IDLE;
                else
                    state = State.RUN;
            }
        }
        Profiler.EndSample();

        if (collision.gameObject.CompareTag("Zombie") || collision.gameObject.CompareTag("Traps"))
        {
            TakeDamage();
            Bounce(collision);
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        Profiler.BeginSample("Collision exit check");
        if (whatIsGround == (whatIsGround | 1 << collision.gameObject.layer))
        {
            if (currentGround.Count == 1 && currentGround[0] == collision.gameObject)
            {
                currentGround[0] = null;
                state = State.JUMP;
            }
            else
                currentGround.Remove(collision.gameObject);       
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
                Respawn();
                break;
            case (int)Layer.CheckPoint:
                respawnPosition = collision.gameObject.transform.GetChild(0).position;
                break;
            default:
                break;
        }
    }

    private void Update()
    {

#if UNITY_EDITOR


        switch(state)
        {
            case State.IDLE:
                rigidBodyUnit2d.drag = 1000000f;

                sideHorizontal = (int)Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                    state = State.JUMP;
                }
                else if (sideHorizontal != 0)
                    state = State.RUN;
                else if (Input.GetKeyDown(KeyCode.F) && ammo > 0)
                {
                    Shoot();
                    state = State.ATTACK;
                }
                else if (Input.GetKeyDown(KeyCode.R) && ammo < maxAmmo)
                {
                    state = State.RELOAD;
                    StartCoroutine(ReloadAnimation());                       
                }
                break;

            case State.RUN:
                rigidBodyUnit2d.drag = 1f;
                sideHorizontal = (int)Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                    state = State.JUMP;
                }
                else if (sideHorizontal == 0)
                    state = State.IDLE;
                break;

            case State.JUMP:
                rigidBodyUnit2d.drag = 1f;
                sideHorizontal = (int)Input.GetAxisRaw("Horizontal");
                break;

            case State.ATTACK:
                break;

            case State.RELOAD:
                if (enableMovement)
                {
                    sideHorizontal = (int)Input.GetAxisRaw("Horizontal");
                    if (Input.GetButtonDown("Jump"))
                    {
                        Jump();
                        state = State.JUMP;
                    }
                    else if (sideHorizontal != 0)
                        state = State.RUN;
                    else if (Input.GetKeyDown(KeyCode.F) && ammo > 0)
                    {
                        Shoot();
                        state = State.ATTACK;
                    }
                    else if (reloadTimer > 0)
                    {
                        
                        reloadTimer -= Time.deltaTime;
                    }
                    else
                    {
                        ammo++;
                        GameController.instance.AmmoBarIncreaseUI();
                        reloadTimer = reloadTime;
                        if (ammo == maxAmmo)
                            state = State.IDLE;
                    }
                }
                else
                    state = State.IDLE;
                break;

            case State.BOUNCE:
                rigidBodyUnit2d.drag = 1f;
                break;
        }
#endif

    }

    private void SetIdleState()
    {
        state = State.IDLE;
    }

    public void SetReloadState()
    {
        state = State.RELOAD;
    }

    public void Jump()
    {
        //if (grounded && enableMovement)
        {
            rigidBodyUnit2d.drag = 1f;
            grounded = false;
            anim.SetBool("ground", false);
            rigidBodyUnit2d.velocity += Vector2.up * jumpForce;
        }
    }

    private void Bounce(Collision2D collision)
    {
        state = State.BOUNCE;
        ContactPoint2D point = collision.GetContact(0);
        Rigidbody2D rb2d = collision.gameObject.GetComponent<Rigidbody2D>();
        SingleRoutineStart(ref bounceDelayRoutine, BounceDelay(0.15f));
        rigidBodyUnit2d.velocity = new Vector2(point.normal.x * 10f, rigidBodyUnit2d.velocity.y);
    }

    private void Bounce(Collider2D collider)
    {
        float x;
        state = State.BOUNCE;
        SingleRoutineStart(ref bounceDelayRoutine, BounceDelay(0.15f));
        Rigidbody2D rb2d = collider.GetComponent<Rigidbody2D>();
        if(rb2d != null)
        {
            rigidBodyUnit2d.velocity = new Vector2(rb2d.velocity.x * 2.5f, rigidBodyUnit2d.velocity.y);
        }
        else
        {
            x = rigidBodyUnit2d.velocity.x;
            if (Mathf.Abs(x) > 0f)
                rigidBodyUnit2d.velocity = new Vector2(-Mathf.Sign(x) * (x / x) * 5f, rigidBodyUnit2d.velocity.y);
        }
    }

    private IEnumerator BounceDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bounceDelayRoutine = null;
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, -groundCheckPoint.up, overrlapRadius, whatIsGround);
        if (hit && hit.distance <= 0.01f)
            state = State.IDLE;
        else
            state = State.JUMP;
    }

    private void Respawn()
    {
        SingleRoutineStart(ref bounceDelayRoutine, BounceDelay(1.3f));
        rigidBodyUnit2d.velocity = Vector2.zero;
        transform.position = respawnPosition;       
    }

    public void Shoot()
    {
        //if (ammo > 0 && grounded && !isAttack && enableMovement && rigidBodyUnit2d.velocity.magnitude < 0.01f)
        {
            float projectileDist = 0f;
            float fireRange = 100f;

            fireRange = cameraFollow.camLenght/2f + firePoint.right.x * (cameraFollow.transform.position.x - transform.position.x) - 0.4f;

            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, fireRange, whatToHit);
            if (hit)
            {
                if (hit.transform.CompareTag("Boxes"))
                {
                    boxRef = hit.transform.gameObject;
                    Invoke("BoxDestroy", hit.distance / 60f);
                }

                else if (hit.transform.CompareTag("Zombie"))
                    hit.transform.GetComponent<IZombie>().TakeDamage();

                projectileDist = hit.distance;
            }
            else
                projectileDist = fireRange;


            ammo--;
            GameController.instance.AmmoBarDecreaseUI();

            anim.SetTrigger("shoot");
            StartCoroutine(CameraFollow.instance.Recoil());

            projectileClone = Pool.Pull(Group.Projectile, firePoint.position, firePoint.rotation).GetComponent<ProjectilesMove>();
            if(projectileClone != null)
                projectileClone.maxExistDistance = projectileDist;
            
            gunCaseParticleSystem.Emit(1);
        }
    }

    private void BoxDestroy()
    {
        Pool.Pull(Group.VFX_BoxCrush, boxRef.transform.position, Quaternion.identity, 1f);
        Destroy(boxRef);
    }

    //public void Reload()
    //{
    //    if (!isReloading)
    //    {
    //        StartCoroutine(ReloadConditionsCheck());
    //        StartCoroutine(ReloadWithDelay());
    //    }
    //}

    //private IEnumerator ReloadWithDelay()
    //{
    //    while (isReloading)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        if(isReloading)
    //        {
    //            ammo++;
    //            GameController.instance.AmmoBarIncreaseUI();
    //        }
    //    }
    //}

    //private IEnumerator ReloadConditionsCheck()
    //{
    //    isReloading = true;
    //    StartCoroutine(RloadAnimation());
    //    while (isReloading)
    //    {
    //        if ((rigidBodyUnit2d.velocity.magnitude > 0.01) || (ammo > 4) || isAttack)
    //        {
    //            isReloading = false;
    //        }
    //        yield return null;
    //    }
    //}

    private IEnumerator ReloadAnimation()
    {
        float rotationSpeed = 150f;
        float scaleSpeed = 0.6f;
        Vector3 rotationAngle = new Vector3(0f, 0f, 1f);
        Vector3 scaleIncrement = new Vector3(1f, 1f, 0f);

        reloadFlag.gameObject.SetActive(true);

        while (state == State.RELOAD)
        {
            if (reloadFlag.localScale.x >= 1.2)
                scaleSpeed = -0.6f;
            else if (reloadFlag.localScale.x <= 1)
                scaleSpeed = 0.6f;

            reloadFlag.localScale += scaleIncrement * Time.deltaTime * scaleSpeed;
            reloadArrowIcon.Rotate(0f, 0f, 1f * Time.deltaTime * rotationSpeed);
            yield return null;
        }

        reloadFlag.localScale = new Vector3(1f, 1f, 1f);
        reloadArrowIcon.localRotation = Quaternion.identity;
        reloadFlag.gameObject.SetActive(false);
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
            GameController.instance.HealthBarIncreaseUI();
            Destroy(health);
        }   
    }

    public override void TakeDamage()
    {
        if (healthPoints - 1 > 0)
        {
            healthPoints--;
            GameController.instance.HealthBarDecreaseUI();
            SingleRoutineStart(ref takeDamageBlinkingRoutine, TakeDamageBlinking(blinkCount));
        }
        else
        {
            SingleRoutineStart(ref takeDamageBlinkingRoutine, TakeDamageBlinking(blinkCount));
            Invoke("Death", 0.3f);
        }
    }
    
    public override void Death()
    {
        healthPoints = 0;
        gameObject.SetActive(false);
        GameController.instance.HealthBarOnZeroUI();
        GameController.instance.GameOver();
    }


}
