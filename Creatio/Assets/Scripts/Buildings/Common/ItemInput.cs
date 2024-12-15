using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// TODO
// Spawn objects using polling method

public class ItemInput : MonoBehaviour
{
    // public BuildingPlacement buildingPlacement; // Reference to the BuildingPlacement script
    // [SerializeField] private int spawnItem; // Reference to the item prefab
    private ItemBuffer buffer;

    // static ContactFilter2D filter2D;
    // Vector3 spawnPoint; // The point where the item will be instantiated
    // public ConveyorBeltSegment targetBelt = null; // Reference to the neighboring belt from which to take
    // GameObject item; // Reference to the spawned item




    private void Start()
    {
        buffer = GetComponentInParent<ItemBuffer>();
    }

    private void Update()
    {
    }

    public bool TakeItem(GameObject item) {
        if(buffer.itemInputs.ContainsKey(item.GetComponent<ItemDataLocal>().id) && buffer.itemInputs[item.GetComponent<ItemDataLocal>().id] < 100) {
            buffer.itemInputs[item.GetComponent<ItemDataLocal>().id]++;
            Destroy(item);
            Debug.Log("Item taken");
            return true;
        }

        Debug.Log("Item not taken - wrong id");
        return false;
    }
}