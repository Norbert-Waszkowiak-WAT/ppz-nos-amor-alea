using UnityEngine;
using UnityEngine.UIElements;

// TODO
// Spawn objects using polling method

public class ItemInput : MonoBehaviour
{
    // public BuildingPlacement buildingPlacement; // Reference to the BuildingPlacement script
    // [SerializeField] private int spawnItem; // Reference to the item prefab
    public int buffer; // Number of items in the buffer
    public int maxBuffer;
    public int BufferItem;

    // static ContactFilter2D filter2D;
    // Vector3 spawnPoint; // The point where the item will be instantiated
    // public ConveyorBeltSegment targetBelt = null; // Reference to the neighboring belt from which to take
    // GameObject item; // Reference to the spawned item




    private void Start()
    {
        // if(buildingPlacement != null ) buildingPlacement.BeltsModified.AddListener(CheckForNeighboringBelt);

        // filter2D = new ContactFilter2D();
        // filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        // filter2D.useLayerMask = true;
    }

    private void Update()
    {
    }

    public bool TakeItem(GameObject item) {
        if(buffer >= maxBuffer) {
            Debug.Log("Buffer full");
            return false;
            
        }

        if(item.GetComponent<ItemDataLocal>().id == BufferItem) {
            buffer++;
            Destroy(item);
            Debug.Log("Item taken");
            return true;
        }

        Debug.Log("Item not taken - wrong id");
        return false;
    }

    // private void CheckForNeighboringBelt()
    // {
    //     Collider2D collider = Physics2D.OverlapCircle(transform.position + transform.right, 0.1f, filter2D.layerMask);
        
    //     if (collider != null && collider != GetComponent<Collider2D>())
    //     {
    //         targetBelt = collider.GetComponent<ConveyorBeltSegment>();
    //         spawnPoint = collider.transform.position - targetBelt.length / 2 * collider.transform.right;
    //         spawnPoint.z = -3;
    //         Debug.Log("Found Belt to spawn");
    //     }

    //     else
    //     {
    //         targetBelt = null;
    //     }
    // }

    public void Initialize(int item)
    {
        BufferItem = item;
    }

    // public void SetManager(BuildingPlacement reference)
    // {
    //     buildingPlacement = reference;
    // }
}