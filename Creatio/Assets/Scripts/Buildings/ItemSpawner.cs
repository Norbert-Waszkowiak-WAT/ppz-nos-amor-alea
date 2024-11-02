using UnityEngine;
using UnityEngine.UIElements;

// TODO
// Spawn objects using polling method

public class IronIngotSpawner : MonoBehaviour
{
    public BuildingPlacement buildingPlacement; // Reference to the BuildingPlacement script
    public string spawnItem; // Reference to the item prefab
    public float spawnInterval = 2.0f; // Time interval between spawns [s]
    [SerializeField] float timer;


    static ContactFilter2D filter2D;
    Vector3 spawnPoint; // The point where the item will be instantiated
    ConveyorBeltSegment targetBelt = null; // Reference to the neighboring belt
    // GameObject item; // Reference to the spawned item




    private void Start()
    {
        buildingPlacement.BeltsModified.AddListener(CheckForNeighboringBelt);

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
        if (spawnItem != null && spawnPoint != null && targetBelt != null && targetBelt.HasRoomOnBelt())
        {
            // item = Instantiate(spawnItem, spawnPoint, Quaternion.identity);
            //Debug.Log("Iron ingot spawned");
            // Optionally, you can add the new ingot to the target belt's item list
            targetBelt.AddItem("iron_ingot", 0f); // Assuming 0f is the initial distance for the new item
        }

        else if(targetBelt != null && !targetBelt.HasRoomOnBelt())
        {
            //Debug.Log("No room on belt");
        }
    }

    private void CheckForNeighboringBelt()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position + transform.right, 0.1f, filter2D.layerMask);
        
        if (collider != null && collider != GetComponent<Collider2D>())
        {
            targetBelt = collider.GetComponent<ConveyorBeltSegment>();
            spawnPoint = collider.transform.position - targetBelt.length / 2 * collider.transform.right;
            spawnPoint.z = -3;
            Debug.Log("Found Belt to spawn");
        }

        else
        {
            targetBelt = null;
        }
    }
}