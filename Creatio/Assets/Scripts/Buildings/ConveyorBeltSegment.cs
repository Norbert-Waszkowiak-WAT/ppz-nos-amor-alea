using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ConveyorBeltSegment : MonoBehaviour
{
    // Mateusz    -  zrob ze kurwa ta mape, system generacji
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
    [SerializeField] bool clogged = false; // can the first item move
    public int length; 

    static public float moveSpeed; // speed of belt
                            // 1 = 120 ipm / 2ips [items per minute/second]
    bool isHologram = true; 
    static ContactFilter2D filter2D;

    [SerializeField] Dictionary<string, ItemData> itemDataDictionary;

    [SerializeField] List<float> itemDistances; // Distances between items
    [SerializeField] List<string> itemIDs; // IDs of the items - IDs will be used for crafting and machine Implementation
    List<bool> isStuck; // List of bools representing whether the item is stuck
    List<GameObject> items; // GameObjects representing the items


    Vector3 globalBeltEnd;
    Vector3 itemPosition;

    public float initialGap = 0f; // Gap between beginning of the segment and first item

    // public int lastNonZeroGapIndex = -1; // Index of the last non-zero gap

    void Start()
    {
        if(!isHologram) {
            buildingManager.BeltsModified.AddListener(UpdateNextBelt);

            filter2D = new ContactFilter2D();
            filter2D.SetLayerMask(LayerMask.GetMask("ConveyorBelts"));
            filter2D.useLayerMask = true;

            UpdateNextBelt();

            itemDistances = new List<float>();
            itemIDs = new List<string>();
            isStuck = new List<bool>();
            items = new List<GameObject>();

            itemDataDictionary = ItemData.LoadItemData();

            initialGap = length;

            if (length % 2 == 0) {
                globalBeltEnd = length / 2 * transform.right + transform.position;
            }
            else {
                globalBeltEnd =  (length / 2 + 0.5f) * transform.right + transform.position;
            }
        }
    }

    void Update()
    {
        delta = moveSpeed * Time.deltaTime;

        if(!isHologram) {
            MoveItems(delta);
            UpdateItemTransforms();
            // CheckItemsProximityToPlayer();
        }

    }

    public float GetSpeed() {
        return moveSpeed;
    }
    private void UpdateNextBelt() 
    {
        if(isHologram) return;
        // Debug.Log("Updating next belt");

        Collider2D collider = Physics2D.OverlapCircle(NextBeltPosition(), 0.1f, filter2D.layerMask);
        
        if (collider != null && collider != GetComponent<Collider2D>())
        {
            ConveyorBeltSegment potentialNextSegment = collider.GetComponent<ConveyorBeltSegment>();

            Vector3 nextLengthOffset;
            if (potentialNextSegment.length % 2 == 0) {
            nextLengthOffset = (potentialNextSegment.length / 2 - 0.5f) * potentialNextSegment.transform.right;
            }
            else {
                nextLengthOffset = potentialNextSegment.length / 2 * potentialNextSegment.transform.right;
            }

            // Debug.Log($"Next segment length offset: {nextLengthOffset}");

            Vector3 nextSegmentBegin = potentialNextSegment.transform.position - nextLengthOffset;
            nextSegmentBegin.z = -2;

            // Instantiate(debug, nextSegmentBegin, Quaternion.identity);


            Vector3 position = NextBeltPosition();
            position.z = -2;

            // Debug.Log($"Next segment begin: {nextSegmentBegin}, position: {position}");

            if (potentialNextSegment != null && nextSegmentBegin == position)
            {
                nextSegment = potentialNextSegment;
                clogged = false;
                if(isStuck.Count > 0) isStuck[0] = false;
                Debug.Log("Next segment found");
                return;
            }
            // Debug.Log("Position doesnt match");
        }

        nextSegment = null;
    }

    public void AddItem(string ID, float distance)
    {
        // Calculate the distance from the end of the belt
        float distanceFromEnd = length - distance;

        itemDistances.Add(distanceFromEnd);
        itemIDs.Add(ID);
        items.Add(null);
        isStuck.Add(false);

        //Debug.Log($"Item added to segment, distance from end: {distanceFromEnd}, items count: {items.Count}");
    }

    public void MoveItems(float delta)
    {
        if(itemDistances.Count > 0) 
        {
            // Move each item and adjust distances
            for (int i = 0; i < itemDistances.Count; i++)
            {
                if(!isStuck[i] || !clogged) 
                {
                    itemDistances[i] -= delta;

                    // Ensure minimum distance between items
                    // if (i < itemDistances.Count - 1 && itemDistances[i + 1] - itemDistances[i] < .5f)
                    // {
                    //     itemDistances[i + 1] = itemDistances[i] + 0.5f;
                    //     Debug.Log($"Adjusting item {i + 1}");
                    // }
                }
            }


            // If the item has moved past the end of the belt
            if (itemDistances[0] <= 0) 
            {
                if (nextSegment != null)
                {
                    isStuck[0] = false;
                    
                    if(nextSegment.HasRoomOnBelt())
                    {
                        clogged = false;
                        isStuck[0] = false;
                        if(itemDistances[0] < -0.5f) PassItemToNextSegment(0);
                    }  
                    else 
                    {
                        itemDistances[0] = 0;
                        isStuck[0] = true;
                        clogged = true;
                    }               
                }

                else 
                {
                    itemDistances[0] = 0;
                    isStuck[0] = true;
                    clogged = true;
                }
            }

            if (clogged) {
                for (int i = 1; i < itemDistances.Count; i++)
                {
                    if (isStuck[i]) continue;

                    if(itemDistances[i] <= i * 0.5f) 
                    {
                        itemPosition = globalBeltEnd - (0.5f * i * transform.right);
                        itemPosition.z = -3;
                        if(items[i] != null) items[i].transform.position = itemPosition;
                        itemDistances[i] = i * 0.5f;
                        isStuck[i] = true;
                    }
                }
            }

            else {
                for (int i = 0; i < itemDistances.Count; i++)
                {
                    isStuck[i] = false;
                }
            }

            // if(isStuck.Count > 0 && isStuck[0]) isStuck[0] = false;

            if (itemDistances.Count > 0)initialGap = length - itemDistances[^1];
        }

        else initialGap = length;
    }

    private void PassItemToNextSegment(int index)
    {
        Debug.Log($"Passing item {index} to next segment");
        if (nextSegment != null)
        {
            // Pass the item to the next segment - distance is 0.5f carried over so the item is centered on turn
            nextSegment.AddItem(itemIDs[index], 0.5f);
            itemDistances.RemoveAt(index);
            items.RemoveAt(index);
            isStuck.RemoveAt(index);
            //Debug.Log($"Item passed to next segment, remaining items: {itemDistances.Count}");
        }
        else 
        {
            itemDistances[index] = 0;
        }

        // Update the lastNonZeroGapIndex
        // if (index == lastNonZeroGapIndex && itemDistances.Count > 0)
        // {
        //     lastNonZeroGapIndex = itemDistances.Count - 1;
        // }
    }

    private void UpdateItemTransforms()
    {
        for (int i = 0; i < items.Count; i++)
        {
            //if (!items[i].GetComponent<Renderer>().isVisible) continue;
            if (isStuck[i] || items[i] == null) continue;
        
            itemPosition = globalBeltEnd - itemDistances[i] * transform.right;
            itemPosition.z = -3;

            items[i].transform.position = itemPosition;
            //Debug.Log($"Item {i} position: {itemPosition}. Item distance: {itemDistances[i]}");
        }
    }

    // private void CheckItemsProximityToPlayer()
    // {
    //     for (int i = 0; i < items.Count; i++)
    //     {
    //         Vector3 itemPosition = globalBeltEnd - itemDistances[i] * transform.right;

    //         float distanceToPlayer = Vector3.Distance(itemPosition, player.transform.position);
    //         if (distanceToPlayer <= 100.0f && IsVisible(itemPosition))
    //         {
    //             CreateItem(itemIDs[i], itemPosition, i);
    //         }
    //         else if (items[i] != null)
    //         {
    //             Destroy(items[i]);
    //             items[i] = null;
    //         }
    //     }
    // }

    private bool IsVisible( Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    private void CreateItem(string itemID, Vector3 position, int index)
    {
        if (itemDataDictionary.TryGetValue(itemID, out ItemData itemData))
        {
            GameObject itemObject = Instantiate(itemPrefab, position, Quaternion.identity);
            items[index] = itemObject;

            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = itemData.sprite;
            }
            else
            {
                Debug.LogError($"Item prefab does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning($"No sprite found for itemID: {itemID}");
        }
    }




    public bool HasRoomOnBelt()
    {
        return initialGap > 0.5f;
    }

    public void SetSpeed(float speed) 
    {
        moveSpeed = speed;
    }

    public void RemoveHologramFlag()
    {
        isHologram = false;
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

    private void OnDestroy() 
    {
        if(!isHologram) 
        {
            buildingManager.BeltsModified.Invoke();
            buildingManager.BeltsModified.RemoveListener(UpdateNextBelt);
                    
            foreach (var item in items)
            {
                Destroy(item);
            }
        }
    }

    public void SetManager(BuildingPlacement refernce) {
        buildingManager = refernce;
    }
}