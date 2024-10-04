using UnityEngine;

public class IronIngotSpawner : MonoBehaviour
{
    public GameObject ironIngotPrefab; // Reference to the Iron_Ingot prefab
    public Transform spawnPoint = null; // The point where the Iron_Ingot will be instantiated
    public ConveyorBeltSegment targetBelt = null; // Reference to the neighboring belt

    public float spawnInterval = 2.0f; // Time interval between spawns
    private float timer;

    public BuildingPlacement buildingPlacement; // Reference to the BuildingPlacement script
    private ContactFilter2D filter2D;
    private Vector3 ingotPosition;

    private void Start()
    {
        buildingPlacement.BuildingPlaced.AddListener(CheckForNeighboringBelt);

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnIronIngot();
            timer = 0f;
        }
    }

    private void SpawnIronIngot()
    {
        if (ironIngotPrefab != null && ingotPosition != null && targetBelt != null && HasRoomOnBelt(targetBelt))
        {
            Instantiate(ironIngotPrefab, ingotPosition, Quaternion.identity);
            Debug.Log("Iron ingot spawned");
            // Optionally, you can add the new ingot to the target belt's item list
            targetBelt.AddItem(0f); // Assuming 0f is the initial distance for the new item

            //test
        }
    }

    private void CheckForNeighboringBelt()
    {
        // Assuming the building is placed at the position of the spawner
        Vector3 worldPosition = transform.position;

        // Define the directions to check for neighboring belts
        Vector3[] directions = {
            Vector3.right,
         };

        foreach (var direction in directions)
        {
            Vector3 neighborPosition = worldPosition + direction;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(neighborPosition, 0.1f, filter2D.layerMask);
            foreach (var collider in colliders)
            {
                ConveyorBeltSegment belt = collider.GetComponent<ConveyorBeltSegment>();
                if (belt != null)
                {
                    ingotPosition = belt.transform.position;
                    ingotPosition.z = -3;
                    targetBelt = belt.GetComponent<ConveyorBeltSegment>();
                    // Perform any additional logic here, such as connecting belts
                }
            }
        }
    }

        private bool HasRoomOnBelt(ConveyorBeltSegment belt)
    {
        Debug.Log("Checking if there is room on the belt");
        // Check if there is enough room on the belt to spawn a new item
        float minDistanceBetweenItems = 0.5f; // Define the minimum distance required between items
        float finalGap = belt.finalGap; // Assuming finalGap is a public property of ConveyorBeltSegment

        // Check if the final gap is large enough to accommodate a new item
        return finalGap >= minDistanceBetweenItems;
    }
}