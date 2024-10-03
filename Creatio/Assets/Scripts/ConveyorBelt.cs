using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public BuildingPlacement builderManager;
    public Collider2D conveyorCollider;
    public GameObject nextBeltSegment;
    private Vector2 rayDirection;
    private float localRotation;
    public bool isOccupied;
    private ContactFilter2D filter2D;

    private Collider2D[] colliderHit = new Collider2D[1];

    private RaycastHit2D[] hit = new RaycastHit2D[1];

    // Start is called before the first frame update
    void Start()
    {
        conveyorCollider = GetComponent<Collider2D>();
        isOccupied = false;

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;

        FindNextSegment();
    }

    // Update is called once per frame
    void Update()
        {
        builderManager.BuildingPlaced.AddListener(FindNextSegment); 

        //Check if neighbors another belt
        //set nextBeltSegment
    }

    void FindNextSegment() {
        hit[0] = new RaycastHit2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));

        localRotation = gameObject.transform.localEulerAngles.z;
        localRotation *= Mathf.Deg2Rad;

        rayDirection.x = Mathf.Cos(localRotation);
        rayDirection.y = Mathf.Sin(localRotation);

        GetComponent<Collider2D>().Raycast(rayDirection,filter2D, hit, 1f);
        if(hit[0].collider != null) {
            nextBeltSegment = hit[0].collider.gameObject;
            //Debug.Log("Next segment found");
        }
        else nextBeltSegment = null;
    }

    /*
    void ResetIsOccupied() {
        filter2D.SetLayerMask(LayerMask.GetMask("Items"));
        GetComponent<Collider2D>().OverlapCollider(filter2D, colliderHit);
        if(colliderHit[0] == null) {
            isOccupied = false;
            Debug.Log("Item removed");
        }
    }
    */
    //vector direction = cos(x) sin(y)


    //github way
    //or
    //during building set the begin and end of the belt
    //and during frame move the items between the two points
}
