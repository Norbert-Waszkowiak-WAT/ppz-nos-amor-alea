using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BuildingPlacement buildingManager;
    public SpriteRenderer SpriteRenderer;
    public float speed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    private GameObject hologram;

   private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LayerMask layer2 = LayerMask.NameToLayer("Hologram");
        Physics2D.IgnoreLayerCollision(0, layer2, true);
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
