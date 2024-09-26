using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public SpriteRenderer SpriteRenderer;
    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        moveVelocity = Vector2.ClampMagnitude(moveVelocity, 1);
        rb.velocity = moveVelocity * speed;

        //if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        //{
        //    rb.velocity = Vector2.zero;
        //}

        if (moveVelocity.x < 0)
        {
            SpriteRenderer.flipX = true;
        }
        else if (moveVelocity.x > 0)
        {
            SpriteRenderer.flipX = false;
        }
    }
}
