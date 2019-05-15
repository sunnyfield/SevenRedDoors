using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitScript
{
    public static PlayerController instance;

    private CameraFollow cameraFollow;
    private Transform reloadFlag;
    private Transform reloadArrowIcon;
    private ProjectilesMove projectileClone;
    private ParticleSystem gunCaseParticleSystem;
    private Transform startPosition;
    private Coroutine inputDelayRoutine = null;
    private GameObject boxRef;

    public LayerMask whatToHit;


    private float jumpForce = 9.8f;
    private int blinkCount = 8;

    private uint ammo = 5;
    private bool isAttack = false;
    private bool isReloading = false;

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
        UnitSetup();
        gunCaseParticleSystem = transform.Find("VFX_ GunCase").GetComponent<ParticleSystem>();

        reloadFlag = transform.Find("ReloadAmmoSprite");
        reloadArrowIcon = GameObject.Find("/Scene/Player/ReloadAmmoSprite/ReloadFlagSprite").GetComponent<Transform>();
        reloadFlag.gameObject.SetActive(false);

        startPosition = GameObject.Find("/Scene/StartPosition").GetComponent<Transform>();

        healthPoints = 3;
        lerpSpeed = 40f;

        cameraFollow = CameraFollow.instance;
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Zombie") || collision.gameObject.CompareTag("Traps"))
        {
            TakeDamage();
            Bounce(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Traps"))
        {
            TakeDamage();
            Bounce(collision);
        }

        else if (collision.gameObject.CompareTag("Coins"))
            PickUpCoin(collision.gameObject);

        else if (collision.gameObject.CompareTag("Keys"))
            PickUpKey(collision.gameObject);

        else if (collision.gameObject.CompareTag("Health"))
            PickUpHealth(collision.gameObject);

        else if (collision.gameObject.CompareTag("RedDoor"))
            GameController.instance.GameWinCheck();

        else if (collision.gameObject.CompareTag("DeathObj"))
        {
            TakeDamage();
            MoveToStart();
        }
    }

    private void Update()
    {
        isAttack = anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot");

#if UNITY_EDITOR
        sideHorizontal = (int)Input.GetAxisRaw("Horizontal");

        if (grounded && Input.GetButtonDown("Jump"))
            Jump();

        if (!isAttack && Input.GetKeyDown(KeyCode.F))
            Shoot();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
#endif
    }

    

    public void Jump()
    {
        if (grounded && enableMovement)
        {
            rigidBodyUnit2d.drag = 1f;
            grounded = false;
            anim.SetBool("ground", false);
            rigidBodyUnit2d.velocity += Vector2.up * jumpForce;
        }
    }

    private void Bounce(Collision2D collision)
    {
        ContactPoint2D point = collision.GetContact(0);
        Rigidbody2D rb2d = collision.gameObject.GetComponent<Rigidbody2D>();

            SingleRoutineStart(ref inputDelayRoutine, InputDelay(0.15f));
            rigidBodyUnit2d.velocity = new Vector2(point.normal.x * 12f, rigidBodyUnit2d.velocity.y);
    }

    private void Bounce(Collider2D collider)
    {
        float x;
        SingleRoutineStart(ref inputDelayRoutine, InputDelay(0.15f));
        x = rigidBodyUnit2d.velocity.x;
        if (Mathf.Abs(x) > 0f)
            rigidBodyUnit2d.velocity = new Vector2( -Mathf.Sign(x) * (x / x) * 5f, rigidBodyUnit2d.velocity.y);
    }

    private IEnumerator InputDelay(float delay)
    {    
        enableMovement = false;
        yield return new WaitForSeconds(delay);
        enableMovement = true;
        inputDelayRoutine = null;
    }

    private void MoveToStart()
    {
        SingleRoutineStart(ref inputDelayRoutine, InputDelay(1.3f));
        rigidBodyUnit2d.velocity = Vector2.zero;
        transform.position = startPosition.position;       
    }

    public void Shoot()
    {
        if (ammo > 0 && grounded && !isAttack && enableMovement && rigidBodyUnit2d.velocity.magnitude < 0.01f)
        {
            float projectileDist = 0f;
            float fireRange = 100f;

            fireRange = cameraFollow.camLenght/2f + firePoint.right.x * (cameraFollow.transform.position.x - transform.position.x) - 0.4f;

            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, fireRange);
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


            --ammo;
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

    public void Reload()
    {
        if (!isReloading)
        {
            StartCoroutine(ReloadConditionsCheck());
            StartCoroutine(ReloadWithDelay());
        }
    }

    private IEnumerator ReloadWithDelay()
    {
        while (isReloading)
        {
            yield return new WaitForSeconds(1f);
            if(isReloading)
            {
                ammo++;
                GameController.instance.AmmoBarIncreaseUI();
            }
        }
    }

    private IEnumerator ReloadConditionsCheck()
    {
        isReloading = true;
        StartCoroutine(RloadAnimation());
        while (isReloading)
        {
            if ((rigidBodyUnit2d.velocity.magnitude > 0.01) || (ammo > 4) || isAttack)
            {
                isReloading = false;
            }
            yield return null;
        }
    }

    private IEnumerator RloadAnimation()
    {
        float rotationSpeed = 150f;
        float scaleSpeed = 0.6f;
        Vector3 rotationAngle = new Vector3(0f, 0f, 1f);
        Vector3 scaleIncrement = new Vector3(1f, 1f, 0f);

        reloadFlag.gameObject.SetActive(true);
        while (isReloading)
        {
            if (reloadFlag.localScale.x >= 1.2 || reloadFlag.localScale.x <= 1)
                scaleSpeed = -scaleSpeed;
            reloadFlag.localScale += scaleIncrement * Time.deltaTime * scaleSpeed;
            reloadArrowIcon.eulerAngles += rotationAngle * Time.deltaTime * rotationSpeed;
            yield return null;
        }
        reloadFlag.localScale = new Vector3(1f, 1f, 1f);
        reloadArrowIcon.eulerAngles = new Vector3(0f, 0f, 0f);
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
