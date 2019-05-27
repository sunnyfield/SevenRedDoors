using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExtrapolateRender : MonoBehaviour
{
    Rigidbody2D rb2d;
    Transform spriteObject;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteObject = transform.GetChild(4);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb2d.velocity.magnitude > 0.01f) spriteObject.localPosition = rb2d.velocity * (Time.time - Time.fixedTime) + new Vector2(0.48f, 0f);
        else spriteObject.localPosition = new Vector2(0.48f, 0f);
    }
}
