using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Rigidbody2D rigidbody2Dcomp;

    void OnEnable()
    {
        rigidbody2Dcomp = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2Dcomp.velocity = Vector2.right * 7f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage();
            gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
    }
}
