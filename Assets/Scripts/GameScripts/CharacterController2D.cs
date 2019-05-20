using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    private Vector2 velocity;
    public float gravityModifire = 1f;

    private const float MIN_MOVE_DISTANCE = 0.001f;
    private const float SHELL_RADIUS = 0.01f;

    private void OnEnable()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        velocity.y += gravityModifire * Physics2D.gravity.y * Time.fixedDeltaTime;

        Movement(velocity);
    }

    void Movement(Vector2 velocity)
    {
        Vector2 move = velocity * Time.fixedDeltaTime;
        float distance = move.magnitude;

        if(distance > MIN_MOVE_DISTANCE)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + SHELL_RADIUS);
        }

        rb2d.MovePosition(rb2d.position + move);
    }
}
