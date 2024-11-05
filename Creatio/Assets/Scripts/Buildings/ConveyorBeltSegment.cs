using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting.FullSerializer;
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
        if(buildingManager != null) buildingManager.BeltsModified.AddListener(UpdateNextBelt);

        filter2D = new ContactFilter2D();
        filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
        filter2D.useLayerMask = true;

        UpdateNextBelt();

        itemDistances = new List<float>();
        items = new List<GameObject>();

        initialGap = length;
        finalGap = 0;
        lastNonZeroGapIndex = -1;

        if (length % 2 == 0) {
            globalBeltBegin = transform.position - length / 2 * transform.right;
            globalBeltEnd = transform.position + length / 2 * transform.right;
        }
        else {
            globalBeltBegin =  transform.position - (length / 2 + 0.5f) * transform.right;
            globalBeltEnd = transform.position + length / 2 * transform.right;
        }

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
    
    private void UpdateNextBelt()
    {
        Collider2D collider = Physics2D.OverlapCircle(NextBeltPosition(), 0.1f, filter2D.layerMask);

        if (collider != null && collider != GetComponent<Collider2D>())
        {
            ConveyorBeltSegment potentialNextSegment = collider.GetComponent<ConveyorBeltSegment>();

            Vector3 nextLengthOffset;
            if (potentialNextSegment.length % 2 == 0)
            {
                nextLengthOffset = (potentialNextSegment.length / 2 - 0.5f) * potentialNextSegment.transform.right;
            }
            else
            {
                nextLengthOffset = potentialNextSegment.length / 2 * potentialNextSegment.transform.right;
            }

            Vector3 nextSegmentBegin = potentialNextSegment.transform.position - nextLengthOffset;
            nextSegmentBegin.z = -2;

            Vector3 position = NextBeltPosition();
            position.z = -2;

            if (potentialNextSegment != null && nextSegmentBegin == position)
            {
                nextSegment = potentialNextSegment;
                Debug.Log("Next segment found");
                return;
            }
        }

        nextSegment = null;
    }

    private Vector2 NextBeltPosition() 
    {
        Vector2 currentPosition = transform.position;
        Vector2 lengthOffset;

        if (length % 2 == 0) {
            lengthOffset = (length / 2 + .5f) * transform.right;
        }
        else {
            lengthOffset = (length / 2 + 1) * transform.right;
        }

        return currentPosition + lengthOffset;
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
                itemDistances.Add(distanceFromEnd);
            }
            else
            {
                float previousDistance = 0; 
                for(int i = 0; i < itemDistances.Count; i++) {
                    previousDistance += itemDistances[i];
                }

                itemDistances.Add(length - previousDistance);
                finalGap = 0;
            }
    
            items.Add(itemObject);
    
            if (lastNonZeroGapIndex == -1 && itemDistances[itemDistances.Count - 1] > 0.5f)
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
    
    public void AddItem(GameObject item)
    {
        float distanceFromEnd = length - 0.5f;
    
        if (items.Count == 0)
        {
            initialGap = distanceFromEnd;
            itemDistances.Add(distanceFromEnd);
        }
        else
        {
            float previousDistance = 0; 
            for(int i = 0; i < itemDistances.Count; i++) {
                previousDistance += itemDistances[i];
            }

            itemDistances.Add(length - previousDistance);
            finalGap = 0;
        }
    
        items.Add(item);
    
        if (lastNonZeroGapIndex == -1 && itemDistances[itemDistances.Count - 1] > 0.5f)
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

        if(nextSegment != null && itemDistances[0] <= -0.5f) {
            PassItemToNextSegment(0);
        }

        if (initialGap > 0)
        {
            initialGap -= delta;
            if (initialGap < 0) initialGap = 0;
            finalGap += delta;
            // if (finalGap > length - initialGap - itemDistances.Count * 0.5f) finalGap = length - initialGap - itemDistances.Count * 0.5f;
            itemDistances[0] -= delta;
        }

        else if (lastNonZeroGapIndex >= 0 && lastNonZeroGapIndex < itemDistances.Count)
        {
            itemDistances[lastNonZeroGapIndex] -= delta;
            finalGap += delta;
            if (finalGap > length - initialGap - itemDistances.Count * 0.5f) finalGap = length - initialGap - itemDistances.Count * 0.5f;

            if (nextSegment == null && itemDistances[0] <= 0) {
                itemDistances[0] = 0;
            }

            if (itemDistances[lastNonZeroGapIndex] <= 0.5f)
            {
                itemDistances[lastNonZeroGapIndex] = 0.5f;
                lastNonZeroGapIndex++;
                return;
            }
            // gap 0 - end of segment.1st item
            // gap 1 - 1st item.2nd item
            // gap 3 - 2nd item.belt begin

        }

        else {
            lastNonZeroGapIndex = -1;
        }

        if (itemDistances.Count > 0 && itemDistances[0] <= 0)
        {
            if (nextSegment != null)
            {
                if (nextSegment.HasRoomOnBelt())
                {
                    if (itemDistances[0] < 0f) PassItemToNextSegment(0);
                }
                else
                {
                    itemDistances[0] = 0;
                }
            }
            else
            {
                itemDistances[0] = 0;
            }
        }
    }

    private void PassItemToNextSegment(int index)
    {
        Debug.Log($"Passing item {index} to next segment");
        if (nextSegment != null)
        {
            nextSegment.enabled = true;
            nextSegment.AddItem(items[index]);
            itemDistances.RemoveAt(index);
            items.RemoveAt(index);

            lastNonZeroGapIndex = 0;
            initialGap = itemDistances.First();
        }
        else
        {
            itemDistances[index] = 0;
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
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) continue;
    
            cumulativeDistance += itemDistances[i];
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

        buildingManager.BeltsModified.RemoveListener(UpdateNextBelt);
        buildingManager.BeltsModified.Invoke();
    }

    public void Initialize(BuildingPlacement reference, int length) {
        buildingManager = reference;
        this.length = length;
    }
}