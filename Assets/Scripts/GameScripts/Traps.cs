using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public enum TrapType { Saw, Arrow };
    public TrapType currentTrapType;
    private Rigidbody2D rigidbodySaw;
    private Vector2 moveDirectionSaw;
    private Vector2 moveDirectionArrow;
    private Transform arrowPoint;
    private Rigidbody2D arrowRB2D;
    private Transform arrowTransform;
    private float sawSpeed = 5f;
    private float arrowSpeed = 3f;

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
            default:
                break;
        }
    }

    private void SawSetup()
    {
        moveDirectionSaw = Vector2.right * sawSpeed;
        rigidbodySaw = gameObject.GetComponent<Rigidbody2D>();
        rigidbodySaw.velocity = moveDirectionSaw;
        StartCoroutine(RotateSaw());
    }

    private void ArrowSetup()
    {
        moveDirectionArrow = Vector2.up * arrowSpeed;
        arrowPoint = transform.GetChild(0);
        arrowRB2D = transform.GetChild(1).gameObject.GetComponent<Rigidbody2D>();
        arrowTransform = transform.GetChild(1);
        arrowRB2D.velocity = moveDirectionArrow;
        StartCoroutine(ArrowShoot());
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

    private void SawTrigger(Collider2D collision)
    {
        if (collision.CompareTag("Flip"))
        {
            moveDirectionSaw = -moveDirectionSaw;
            rigidbodySaw.velocity = moveDirectionSaw;
        }
    }

    private IEnumerator ArrowShoot()
    {
        while (gameObject.activeInHierarchy)
        {
            if (arrowTransform.localPosition.y > arrowPoint.localPosition.y)
            {
                arrowRB2D.velocity = Vector2.zero;
                arrowTransform.localPosition = Vector2.zero;
                arrowTransform.gameObject.SetActive(false);
                yield return new WaitForSeconds(Random.Range(0.2f, 1.2f));
            }
            arrowTransform.gameObject.SetActive(true);
            arrowRB2D.velocity = moveDirectionArrow;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator RotateSaw()
    {
        while (gameObject.activeInHierarchy)
        {
            rigidbodySaw.MoveRotation(rigidbodySaw.rotation + Time.deltaTime * 10000f);
            yield return null;
        }
    }
}
