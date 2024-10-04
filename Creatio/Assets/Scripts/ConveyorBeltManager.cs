using System;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    public List<ConveyorBeltSegment> beltSegments = new List<ConveyorBeltSegment>();
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
        CheckAndConnectNeighboringSegments(newSegment);
    }

    private void CheckAndConnectNeighboringSegments(ConveyorBeltSegment segment)
    {
        float rotationZ = segment.transform.localEulerAngles.z;
        rotationOffset = new Vector2(MathF.Round(Mathf.Cos(rotationZ * Mathf.Deg2Rad)), MathF.Round(Mathf.Sin(rotationZ * Mathf.Deg2Rad)));

        Vector2 neighborPosition = segment.transform.position;
        neighborPosition += rotationOffset;

        Collider2D collider = Physics2D.OverlapCircle(neighborPosition, 0.1f, filter2D.layerMask);
        Debug.Log("Checking Collider");

        if (collider == null)
        {
            Debug.Log("No Collider");
            return;
        }

        ConveyorBeltSegment neighborSegment = collider.GetComponent<ConveyorBeltSegment>();
        if (neighborSegment != null && neighborSegment != segment)
        {
            if (ShouldConnectSegments(segment, neighborSegment))
            {
                segment.ConnectToNextSegment(neighborSegment);
            }
        }
    }

    private bool ShouldConnectSegments(ConveyorBeltSegment segment, ConveyorBeltSegment neighborSegment)
    {
        Debug.Log("Checking if segments should connect");
        float neighborRotationZ = neighborSegment.transform.localEulerAngles.z;
        if(MathF.Abs(segment.transform.localEulerAngles.z - neighborRotationZ) <= 90)
        {
            return true;
        }
        return false;
    }
}