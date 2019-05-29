using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public enum TrapType { Saw, Arrow, Platform };
    public TrapType currentTrapType;
    private Rigidbody2D rigidbodySaw;
    private Rigidbody2D platformRB2D;
    private HingeJoint2D platformJoint;
    private Vector2 moveDirectionSaw;
    private Vector2 moveDirectionArrow;
    private Transform arrowPoint;
    private Rigidbody2D arrowRB2D;
    private Transform arrowTransform;
    private float sawSpeed = 5f;
    private float arrowSpeed = 3f;
    private float trapTimer;

    private void Start()
    {
        switch (currentTrapType)
        {
            case (TrapType.Saw):
                SawSetup();
                break;
            case (TrapType.Arrow):
                ArrowSetup();
                break;
            case TrapType.Platform:
                PlatformSetup();
                break;
            default:
                break;
        }
    }

    private void SawSetup()
    {
        moveDirectionSaw = Vector2.right * sawSpeed;
        rigidbodySaw = gameObject.GetComponent<Rigidbody2D>();
        rigidbodySaw.velocity = moveDirectionSaw;
    }

    private void ArrowSetup()
    { 
        arrowPoint = transform.GetChild(0);
        arrowRB2D = transform.GetChild(1).gameObject.GetComponent<Rigidbody2D>();
        arrowTransform = transform.GetChild(1);
        moveDirectionArrow = (arrowPoint.position - arrowTransform.position).normalized * arrowSpeed;
        arrowRB2D.velocity = moveDirectionArrow;
        trapTimer = Random.Range(0.2f, 1.2f);
    }

    private void PlatformSetup()
    {
        platformRB2D = gameObject.GetComponent<Rigidbody2D>();
        platformJoint = gameObject.GetComponent<HingeJoint2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (currentTrapType)
        {
            case (TrapType.Saw):
                SawTrigger(collision);
                break;
            default:
                break;
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentTrapType)
        {
            case (TrapType.Platform):
                if (collision.gameObject.layer == (int)Layer.Player) Invoke("ActivatePlatform", 1f);
                break;
            default:
                break;
        }
    }

    private void ActivatePlatform()
    {
        if (platformJoint != null && platformRB2D != null)
        {
            platformJoint.enabled = true;
            platformRB2D.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gameObject, 2f);
        }
    }

    private void FixedUpdate()
    {
        switch (currentTrapType)
        {
            case (TrapType.Arrow):
                if (arrowTransform.localPosition.y + 1f > arrowPoint.localPosition.y)
                {
                    arrowTransform.gameObject.SetActive(false);
                    arrowTransform.localPosition = Vector2.zero;
                    trapTimer = Random.Range(0.2f, 1.2f);
                }
                if (trapTimer > 0)
                    trapTimer -= Time.fixedDeltaTime;
                else
                {
                    if (!arrowTransform.gameObject.activeInHierarchy)
                    {
                        arrowTransform.gameObject.SetActive(true);
                        arrowRB2D.velocity = moveDirectionArrow;
                    }
                }
                break;
            case (TrapType.Saw):
                rigidbodySaw.MoveRotation(rigidbodySaw.rotation + Time.deltaTime * 10000f);
                break;
            default:
                break;
        }
    }

    private void SawTrigger(Collider2D collision)
    {
        if (collision.CompareTag("Flip"))
        {
            moveDirectionSaw = -moveDirectionSaw;
            rigidbodySaw.velocity = moveDirectionSaw;
        }
    }
}
