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

 
        int selectedOption = PlayerPrefs.GetInt("selectedOption", 0);
        CharacterDatabase characterDB = FindObjectOfType<CharacterDatabase>();

        if (characterDB != null)
        {
            Character character = characterDB.GetCharacter(selectedOption);
            SpriteRenderer.sprite = character.characterSprite;
        }
        else
        {
            Debug.LogError("CharacterDatabase not found in the scene.");
        }

        LayerMask layer = LayerMask.NameToLayer("Hologram");
        Physics2D.IgnoreLayerCollision(0, layer, true);
    }

 
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
