using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class ConveyorBeltManager : MonoBehaviour
{

    
    /*public List<ConveyorBeltSegment> beltSegments = new List<ConveyorBeltSegment>();
    public float moveSpeed = 1.0f;

    private ContactFilter2D filter2D;
    private Vector2 rotationOffset;

    void Start() {

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;
    }

    void Update()
    {
        float delta = moveSpeed * Time.deltaTime;
        foreach (var segment in beltSegments)
        {
            segment.MoveItems(delta);
        }
    }

    public void AddItemToSegment(int segmentIndex, float distance)
    {
        if (segmentIndex >= 0 && segmentIndex < beltSegments.Count)
        {
            beltSegments[segmentIndex].AddItem(distance);
        }
    }

    public void PlaceBeltSegment(ConveyorBeltSegment newSegment, Vector3 position)
    {
        newSegment.transform.position = position;
        beltSegments.Add(newSegment);
    }

    public void RemoveBeltSegment(ConveyorBeltSegment segment)
    {
        beltSegments.Remove(segment);
    }

    /*
    public void CheckAndConnectNeighboringSegments(ConveyorBeltSegment currentSegment)
    {
        float rotationZ = currentSegment.transform.localEulerAngles.z;
        rotationOffset = new Vector2(MathF.Round(Mathf.Cos(rotationZ * Mathf.Deg2Rad)), MathF.Round(Mathf.Sin(rotationZ * Mathf.Deg2Rad)));

        //look behind and find previous segment
        Vector2 currentPosition = currentSegment.transform.position;
        Vector2 neighborPosition = currentSegment.transform.position;
        neighborPosition -= rotationOffset;

        Collider2D collider = Physics2D.OverlapCircle(neighborPosition, 0.1f, filter2D.layerMask);

        if (collider == null)
        {
            Debug.Log("No Collider");
            return;
        }

        ConveyorBeltSegment neighborSegment = collider.GetComponent<ConveyorBeltSegment>();

        if (neighborSegment != null && neighborSegment != currentSegment)
        {
            if (neighborSegment.NextBeltPosition() == currentPosition)
            {
                neighborSegment.ConnectToNextSegment(currentSegment);
            }
        }
    }
    */
    
}