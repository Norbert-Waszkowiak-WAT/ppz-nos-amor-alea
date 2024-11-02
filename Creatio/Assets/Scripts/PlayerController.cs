using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float speed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    private Vector2 beltVelocity;

    [SerializeField] ContactFilter2D filter2D;
    List<Collider2D> hitObject;
    GameObject belt;

   private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.useFullKinematicContacts = true;
        transform.position = new Vector3(80, 80, -5);

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("ConveyorBelts"), true);
        
        hitObject = new List<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputMove();
        BeltMove();

        rb.velocity = moveVelocity + beltVelocity;
    }

    void InputMove() {
        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        moveVelocity = Vector2.ClampMagnitude(moveVelocity, 1);
        moveVelocity *= speed;

        if (moveVelocity.x < 0)
        {
            SpriteRenderer.flipX = true;
        }
        else if (moveVelocity.x > 0)
        {
            SpriteRenderer.flipX = false;
        }
    }

    public void BeltMove() {
        hitObject.Clear();
        if(GetComponent<Collider2D>().OverlapCollider(filter2D, hitObject) != 0) {
            Debug.Log("OnBelt");

            belt = hitObject[0].gameObject;

            float speed = belt.GetComponent<ConveyorBeltSegment>().GetSpeed();
            if(belt.transform.rotation.eulerAngles.z == 0)
            {
                beltVelocity = new Vector2(1, 0) * speed;
            }

            else if (belt.transform.rotation.eulerAngles.z == 180)
            {
                beltVelocity = new Vector2(-1, 0) * speed;
            }

            else if (belt.transform.rotation.eulerAngles.z == 90)
            {
                beltVelocity = new Vector2(0, -1) * speed;
            }

            else if (belt.transform.rotation.eulerAngles.z == 270)
            {
                beltVelocity = new Vector2(0, 1) * speed;
            }
        }

        if(hitObject.Count == 0)
        {
            beltVelocity = Vector2.zero;
        }
    }
}
