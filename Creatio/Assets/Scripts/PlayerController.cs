using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float speed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

   private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LayerMask layer = LayerMask.NameToLayer("Hologram");
        Physics2D.IgnoreLayerCollision(0, layer, true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        moveVelocity = Vector2.ClampMagnitude(moveVelocity, 1);
        rb.velocity = moveVelocity * speed;

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
