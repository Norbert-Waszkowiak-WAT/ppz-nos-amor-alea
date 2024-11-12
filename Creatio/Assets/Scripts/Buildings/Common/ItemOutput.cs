using UnityEngine;
using UnityEngine.UIElements;

// TODO
// Spawn objects using polling method

public class ItemOutput : MonoBehaviour
{
    public BuildingPlacement buildingPlacement; // Reference to the BuildingPlacement script
    [SerializeField] private int spawnItem; // Reference to the item prefab
    public int buffer; // Number of items in the buffer
    public int maxBuffer;
    static ContactFilter2D filter2D;
    Vector3 spawnPoint; // The point where the item will be instantiated
    public ConveyorBeltSegment targetBelt = null; // Reference to the neighboring belt
    // GameObject item; // Reference to the spawned item




    private void Start()
    {
        if(buildingPlacement != null ) buildingPlacement.BuildingPlaced.AddListener(CheckForNeighboringBelt);

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;
    }

    private void Update()
    {
        if(buffer > 0)
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        if(targetBelt == null || spawnPoint == null) return;
        targetBelt.enabled = true;
        
        if (targetBelt.HasRoomOnBelt())
        {
            // item = Instantiate(spawnItem, spawnPoint, Quaternion.identity);
            //Debug.Log("Iron ingot spawned");
            // Optionally, you can add the new ingot to the target belt's item list
            targetBelt.AddItem(spawnItem); // Assuming 0f is the initial distance for the new item
            buffer--;
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

    public void Initialize(int item)
    {
        spawnItem = item;
    }

    public void SetManager(BuildingPlacement reference)
    {
        buildingPlacement = reference;
    }
    public bool AddToBuffer()
    {
        if(buffer < maxBuffer) {
            buffer++;
            return true;
        }
        else return false;
    }
}