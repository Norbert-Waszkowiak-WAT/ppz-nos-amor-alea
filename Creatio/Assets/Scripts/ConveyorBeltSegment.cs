using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltSegment : MonoBehaviour
{
    public List<float> itemDistances = new List<float>(); // Distances between items
    public float initialGap = 0f; // Gap at the start of the segment
    public float finalGap = 0f; // Gap at the end of the segment
    private int lastNonZeroGapIndex = -1; // Index of the last non-zero gap

    public ConveyorBeltSegment nextSegment = null; // Reference to the next segment

    public void AddItem(float distance)
    {
        itemDistances.Add(distance);
        if (distance > 0)
        {
            lastNonZeroGapIndex = itemDistances.Count - 1;
        }
    }

    public void MoveItems(float delta)
    {
        if (finalGap > 0)
        {
            finalGap -= delta;
            if (finalGap < 0)
            {
                delta = -finalGap;
                finalGap = 0;
            }
            else
            {
                return;
            }
        }

        for (int i = lastNonZeroGapIndex; i >= 0; i--)
        {
            if (itemDistances[i] > 0)
            {
                itemDistances[i] -= delta;
                if (itemDistances[i] < 0)
                {
                    delta = -itemDistances[i];
                    itemDistances[i] = 0;
                }
                else
                {
                    lastNonZeroGapIndex = i;
                    return;
                }
            }
        }

        initialGap -= delta;
        if (initialGap < 0)
        {
            initialGap = 0;
        }
    }

    public void ConnectToNextSegment(ConveyorBeltSegment next)
    {
        nextSegment = next;
    }

    public void DisconnectFromNextSegment()
    {
        if (nextSegment != null) nextSegment = null;
    }
}