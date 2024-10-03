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
        hologram = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        moveVelocity = Vector2.ClampMagnitude(moveVelocity, 1);
        rb.velocity = moveVelocity * speed;
        hologram = buildingManager.hologram;

        if (hologram != null)
        {  
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hologram.GetComponent<Collider2D>(), true);
        }

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
