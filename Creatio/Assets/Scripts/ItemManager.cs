using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private float speed;

    public ConveyorBeltTimer Timer;
    public Rigidbody2D rb;
    ContactFilter2D filter2D;
    public Collider2D[] beltList = new Collider2D[1];
    ConveyorBelt currentBelt;
    GameObject nextBelt;
    Vector3 position;

    int framrate;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;

        Timer.OnBeltTick.AddListener(MoveToNextBelt);
    }

    // Update is called once per frame
    void Update()
    {   
        /*
        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                waypoints = null; // Reached the end of the path
            }
        }
        */
        //Timer.OnBeltTick.AddListener(MoveToNextBelt); 

        
            
                      
        // check if is on top of conveyor belt (layermask 6)
        // set position to nextBeltSegment of that conveyor after T if nexBeltSegment isnt occupied

        // OR
        // make belt building similar to satisfactory straight mode
        // where a belt is dragged between 2 points
        // and items are slowly transported between the startPosition and endPosition of the belt segment

    }

    void MoveToNextBelt() {
        beltList[0]= null;
        GetComponent<Collider2D>().OverlapCollider(filter2D, beltList);
        if(beltList[0] != null) {
            currentBelt = beltList[0].gameObject.GetComponent<ConveyorBelt>();
            currentBelt.isOccupied = true;
        }

        if(currentBelt == null) return;

        if(currentBelt.nextBeltSegment != null) 
        {
            nextBelt = currentBelt.nextBeltSegment.gameObject;
            if(nextBelt.GetComponent<ConveyorBelt>() != null && nextBelt.GetComponent<ConveyorBelt>().isOccupied == false) 
            {
                position = nextBelt.transform.position; 
                position.z = -3;
                gameObject.transform.position = position;
                currentBelt.isOccupied = false;
                return;
            }
        }
    }

    public void SetPath (List<Transform> waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;
        currentWaypointIndex = 0;
    }
}

//redo belts so the game is actually playable
//maybe satisfactory-esque belt segment? 
//definitely no physics and colliders - lag spikes on beltUpdateTick
//
