using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBeltSegment : MonoBehaviour
{
    // Mateusz    -  zrob ze ta mape, system generacji
    // Agnieszka  -  tekstury itemow, pomoc przy mapie 
    // Blizniuk   -  tekstury budynkow, zakret pasa - 2 sprite renderery? 1 kratka na zakret, reszta prosty tile, animacja gracza i pasow
    // Filip      -  pomyslec nad itemami, przepisami i progresja - tier 1,2,3 - wrzucic itemy do .json
    //
    // To wszystko na etap I

    // public ConveyorBeltManager conveyorBeltManager; - unused
    public BuildingPlacement buildingManager;
    // public GameObject player;
    public GameObject itemPrefab; //object used to Instantiate items as game objects;
    [SerializeField] ConveyorBeltSegment nextSegment = null; // Reference to the next segment
    [SerializeField] ItemInput itemInput = null; // Reference to the neighboring item input
    [SerializeField] float delta; // Distance to move items each frame, less = higher fps
    public int length; 

    public static float moveSpeed = 1; // speed of belt
                            // 1 = 120 ipm / 2ips [items per minute/second]
    static ContactFilter2D filter2D;

    //[SerializeField] Dictionary<string, ItemData> itemDataDictionary;

    [SerializeField] List<float> itemDistances; // Distances between items
    [SerializeField] List<GameObject> items; // GameObjects representing the items

    Vector3 globalBeltBegin;
    Vector3 globalBeltEnd;
    Vector3 itemPosition;

    public float initialGap = 0f; // Gap between beginning of the segment and first item
    public float finalGap = 0f;
    [SerializeField] private int lastNonZeroGapIndex = -1;

    // public int lastNonZeroGapIndex = -1; // Index of the last non-zero gap

    void Start()
    {
        if(buildingManager != null) buildingManager.BuildingPlaced.AddListener(UpdateNext);

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts") | (LayerMask.GetMask("Buildings")));
        filter2D.useLayerMask = true;

        UpdateNext();

        itemDistances = new List<float>();
        items = new List<GameObject>();

        finalGap = 0;
        lastNonZeroGapIndex = -1;

        Debug.Log(globalBeltBegin + " " + globalBeltEnd);
    }

    void Update()
    {
        if(itemDistances.Count > 0) {
            delta = moveSpeed * Time.deltaTime;
            MoveItems(delta);
            UpdateItemTransforms();
        }

        else enabled = false;
    }
    
    private void UpdateNext()
    {
        Collider2D collider = Physics2D.OverlapCircle(globalBeltEnd + transform.right * 0.5f, 0.1f, filter2D.layerMask);

        if (collider != null && collider != GetComponent<Collider2D>())
        {
            if(collider.GetComponent<ItemInput>() != null) {
                itemInput = collider.GetComponent<ItemInput>();
                return;
            }
            
            if(collider.GetComponent<ConveyorBeltSegment>() != null) {
                ConveyorBeltSegment potentialNextSegment = collider.GetComponent<ConveyorBeltSegment>();
                if (globalBeltEnd + transform.right * 0.5f == potentialNextSegment.globalBeltBegin + potentialNextSegment.transform.right * 0.5f)
                {
                    nextSegment = potentialNextSegment;
                    Debug.Log("Next segment found");
                    return;
                }
            }
        }

        nextSegment = null;
    }

    public void AddItem(int itemID)
    {
        float distanceFromEnd = length;
    
        GameObject itemObject = CreateItem(itemID);
        if (itemObject != null)
        {
            if (items.Count == 0)
            {
                initialGap = distanceFromEnd;
                itemDistances.Add(distanceFromEnd); // Adjusted
            }
            else
            {
                float previousDistance = 0; 
                for(int i = 0; i < itemDistances.Count; i++) {
                    previousDistance += itemDistances[i] + 0.5f; // Adjusted
                }

                itemDistances.Add(length - previousDistance - 0.5f); // Adjusted
                finalGap = 0;
            }

            itemObject.GetComponent<ItemDataLocal>().id = itemID;
            items.Add(itemObject);
    
            if (lastNonZeroGapIndex == -1 && itemDistances[itemDistances.Count - 1] > 0)
            {
                lastNonZeroGapIndex = itemDistances.Count - 1;
            }
            
            //itemObject.transform.position = globalBeltBegin;

            // Fix last gap index
            // fix ghost item prefab at 0,0,0
            // fix gap between 1st and 2nd item during consecutive addition

            // after that start on ItemInput and Smelter or Crafter class

            //Smelter
            //Crafter
            //Combiner
            //Manufacturer
            //Foundry
            //Refinery
            //Dockyard
            //Storage
            //Space Elevator - spaceship dockyard, sell point, terminal
            //Space terminal - send items between planets, sell items
            //Aircraft assembly plant
            //Vehicle assembly plant
            //Trains for intra-planetary transport

            //ultimate item - space super carrier - take inspiration for spaceships from cod iw?
        }
    }
    
    public void AddItemFromBelt(GameObject item)
    {
        if(itemDistances.Count == 0) {
            initialGap = length - 1.5f;
            itemDistances.Add(initialGap);
        }
        else {
            itemDistances.Add(finalGap - 0.5f);
        }

        finalGap = 0;
        items.Add(item);
    
        if (lastNonZeroGapIndex == -1 && itemDistances[itemDistances.Count - 1] > 0)
        {
            lastNonZeroGapIndex = itemDistances.Count - 1;
        }

        //item.transform.position = globalBeltBegin;
    }

    private GameObject CreateItem(int itemID)
    {
        ItemData itemData = ItemDataLoader.GetItemData(itemID);
        if (itemData != null)
        {
            GameObject itemObject = Instantiate(itemPrefab);
            itemObject.transform.position = globalBeltBegin;
            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(itemData.sprite);
            }
            else
            {
                Debug.LogError($"Item prefab does not have a SpriteRenderer component.");
            }
            return itemObject;
        }
        else
        {
            Debug.LogWarning($"No data found for itemID: {itemID}");
            return null;
        }
    }

    public void MoveItems(float delta)
    {
        if (itemDistances.Count == 0) return;

        if (nextSegment == null && itemInput == null)
        {
            if (initialGap > 0)
            {
                initialGap -= delta;
                itemDistances[0] -= delta;

                if(itemDistances[0] < 0)
                {
                    itemDistances[0] = 0;
                    initialGap = 0;
                }
                else finalGap += delta; 
            }
            else {
                initialGap = 0;
                itemDistances[0] = 0;
            }
        }

        else {
            initialGap -= delta;
            itemDistances[0] -= delta;

            if(itemDistances[0] <= -0.5f)
            {
                if(nextSegment != null && nextSegment.HasRoomOnBelt()) PassItemToNextSegment(0);
                else if (itemInput != null && itemInput.TakeItem(items[0])) {
                    itemDistances.RemoveAt(0);
                    items.RemoveAt(0);
                }
                else {
                    itemDistances[0] = -0.5f;
                    initialGap = -0.5f;
                }
            }  
            else finalGap += delta;                       
        }

        if (initialGap == 0 && lastNonZeroGapIndex >= 0 && lastNonZeroGapIndex < itemDistances.Count)
        {
            itemDistances[lastNonZeroGapIndex] -= delta;
            finalGap += delta;
            if (nextSegment == null && itemDistances[lastNonZeroGapIndex] <= 0)
            {
                itemDistances[lastNonZeroGapIndex] = 0;
            }
            if (itemDistances[lastNonZeroGapIndex] <= 0)
            {
                itemDistances[lastNonZeroGapIndex] = 0;
                lastNonZeroGapIndex++;
                return;
            }
        }
        else if (initialGap == 0)
        {
            lastNonZeroGapIndex = -1;
        }
    }

    private void PassItemToNextSegment(int index)
    {
        Debug.Log($"Passing item {index} to next segment");
        if (nextSegment != null)
        {
            nextSegment.enabled = true;
            nextSegment.AddItemFromBelt(items[index]);
            itemDistances.RemoveAt(index);
            items.RemoveAt(index);

            lastNonZeroGapIndex = 0;
            initialGap = itemDistances.First();
        }
        else
        {
            // itemDistances[index] = 0;
        }

        // if (lastNonZeroGapIndex == -1 && itemDistances[itemDistances.Count - 1] > 0.5f)
        // {
        //     lastNonZeroGapIndex = itemDistances.Count - 1;
        // }
    }

    private void UpdateItemTransforms()
    {
        float cumulativeDistance = 0;

        if(itemDistances.Count == 0) return;

        if(items[0] != null) {
            cumulativeDistance += itemDistances[0];// Adjusted
            itemPosition = globalBeltEnd - cumulativeDistance * transform.right;
            itemPosition.z = -3;
            items[0].transform.position = itemPosition;
        }
        
        for (int i = 1; i < items.Count; i++)
        {
            if (items[i] == null) continue;
            
            cumulativeDistance += itemDistances[i] + 0.5f; // Adjusted
            itemPosition = globalBeltEnd - cumulativeDistance * transform.right;
            itemPosition.z = -3;
    
            items[i].transform.position = itemPosition;
            //Debug.Log(i + " " + items[i].transform.position);
        }
    }

    public bool HasRoomOnBelt()
    {
        return finalGap > 0.5f || itemDistances.Count == 0;
    }

    public void SetSpeed(float speed) 
    {
        moveSpeed = speed;
    }

    public float GetSpeed() 
    {
        return moveSpeed;
    }

    private void OnDestroy() 
    {   
        if (items != null) {
            foreach (var item in items)
            {
                Destroy(item);
            }
        }    

        if(buildingManager == null) return;

        buildingManager.BuildingPlaced.RemoveListener(UpdateNext);
        buildingManager.BuildingPlaced.Invoke();
        //Destroy(itemPrefab);
    }

    public void Initialize(BuildingPlacement reference, int param) {
        buildingManager = reference;
        length = param + 1;
        initialGap = length;

        if (length % 2 == 0) {
            globalBeltBegin = transform.position - (length / 2 - 0.5f) * transform.right;
            globalBeltEnd = transform.position + (length / 2 - 0.5f) * transform.right;
        }
        else {
            globalBeltBegin =  transform.position - (length) / 2 * transform.right;
            globalBeltEnd = transform.position + (length) / 2 * transform.right;
        }
    }
}