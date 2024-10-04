using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    /*
    private float speed;
    public ConveyorBeltTimer Timer;
    private ConveyorBelt currentBelt;
    private Vector3 position;

    private void Start()
    {
        Timer.OnBeltTick.AddListener(MoveToNextBelt);
    }

    private void MoveToNextBelt()
    {
        if (currentBelt == null) return;

        float delta = speed * Timer.Interval;
        currentBelt.MoveItems(delta);

        if (currentBelt.nextBeltSegment != null)
        {
            ConveyorBelt nextBelt = currentBelt.nextBeltSegment.GetComponent<ConveyorBelt>();
            if (nextBelt != null && !nextBelt.isOccupied)
            {
                position = nextBelt.transform.position;
                position.z = -3;
                transform.position = position;
                currentBelt.isOccupied = false;
                nextBelt.isOccupied = true;
                currentBelt = nextBelt;
            }
        }
    }

    public void SetSpeed(List<Transform> waypoints, float speed)
    {
        this.speed = speed;
    }
    */
}