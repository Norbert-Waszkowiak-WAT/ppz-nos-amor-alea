using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent BuildingPlaced;
    public UnityEngine.Events.UnityEvent BeltsModified;

    public bool buildMode;
    public bool deleteMode;

    public Grid grid;
    public enum BuildingType
    {
        smelter,
        constructor,
        belt
    }

    //public GameObject beltPrefab;
    public GameObject smelterPrefab;
    public GameObject constructorPrefab;
    public GameObject beltPrefab;
    public GameObject debugObject;

    GameObject hologram = null;
    ContactFilter2D filter;


    BuildingType currentBuildingType;
    // Dictionary to map BuildingType to GameObject
    Dictionary<BuildingType, GameObject> buildingPrefabs;

    bool isPlacingBelt;
    int segmentCount;
    bool canPlaceBelt;

    Vector3 initialPosition;
    GameObject firstBelt = null;

    List<GameObject> beltHolograms = new List<GameObject>();


    /*
    Z Positioning:
     1: Prefabs
     0: Map
    -1: Small Map Objects [Rocks, grass patches, etc.], Ore depostis
    -2: Conveyor Belts
    -3: Items
    -4: Buildings
    -5: Player, enemies, etc.
    -6: Tools
    -7: Large Trees
    -8: 
    -9: UI
    -10: Camera
    */


    // Start is called before the first frame update
    void Start()
    {
        // Initialize the dictionary with your building prefabs
        buildingPrefabs = new Dictionary<BuildingType, GameObject>
        {
            { BuildingType.smelter, smelterPrefab },
            { BuildingType.constructor, constructorPrefab },
            { BuildingType.belt, beltPrefab }
            // Add other building types and their prefabs here
        };
        currentBuildingType = BuildingType.belt;
        buildMode = false;
        deleteMode = false;

        filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("ConveyorBelts") | LayerMask.GetMask("Buildings") | LayerMask.GetMask("Large Flora");
        filter.useLayerMask = true;

    }

    // Update is called once per frame
    void Update()
    {        
        EnterBuildMode();
        EnterDeleteMode();

        if(buildMode)
        {
            Rotate(hologram);
            SwitchBuildingType(); 
            if(currentBuildingType == BuildingType.belt)
            {
                Rotate(firstBelt);
                BuildBelt();
            }
            else {
                BuildMode();
            }
        }  

        else 
        {
            foreach (var beltHologram in beltHolograms)
            {
                Destroy(beltHologram);
            }
            beltHolograms.Clear();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = -3;
            Instantiate(debugObject, SnapToGrid(position, grid), transform.rotation);
            Debug.Log("Debug Object placed");
        }     
    }

    Vector3 SnapToGrid(Vector3 Position, Grid grid)
    {
        Vector3 gridSize = grid.cellSize;
        Position.x = Mathf.FloorToInt(Position.x / gridSize.x) + gridSize.x/2;
        Position.y = Mathf.FloorToInt(Position.y / gridSize.y) + gridSize.y/2;
        return Position;
    }

    void Build(BuildingType buildingType)
    {
        if (buildingPrefabs.TryGetValue(buildingType, out GameObject buildingPrefab))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = -4;
            GameObject created = Instantiate(buildingPrefab, SnapToGrid(position, grid), hologram.transform.rotation);
            created.name = buildingType.ToString();
            Debug.Log("placed");
        }
        
        else
        {
            Debug.LogError("Building type not found in dictionary");
            return;
        }
    }

    void BuildBelt() 
    {   
        if (hologram == null && !isPlacingBelt)
        {
            CreateHologram();
        }
        
        if (hologram != null)
        {
            hologram.transform.position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
        }

        //Fix rotation after first click
        //fix hologram flickering at the end
        //figure out how to connect the belt segments

        int hologramLayer = LayerMask.NameToLayer("Hologram"); 
        int beltLayer = LayerMask.NameToLayer("ConveyorBelts");

        if (Input.GetMouseButtonDown(0))
        {
            if(!isPlacingBelt) {
                // First click: create the first hologram
                firstBelt = Instantiate(hologram, initialPosition, hologram.transform.rotation);
                initialPosition = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
                firstBelt.transform.position = initialPosition;
                firstBelt.layer = hologramLayer;

                isPlacingBelt = true;
                Destroy(hologram);
                beltHolograms.Add(firstBelt);
            } 
            
            else if (canPlaceBelt)
            {
                // Second click: place the belt segment
                // Length + 1 to account for the segment being in the middle of the grid

                // Create the belt segment and initialize it
                Vector3 position = segmentCount / 2 * firstBelt.transform.right + initialPosition;
                if(segmentCount % 2 == 0) {
                    position -= 0.5f * firstBelt.transform.right;
                }

                position.z = -2;

                GameObject beltSegment = Instantiate(beltPrefab, position, firstBelt.transform.rotation);

                beltSegment.GetComponent<ConveyorBeltSegment>().RemoveHologramFlag();
                beltSegment.GetComponent<ConveyorBeltSegment>().SetSpeed(1.0f);
                beltSegment.GetComponent<ConveyorBeltSegment>().length = segmentCount;

                // Set the belt segment's length
                // The belt segment scale is (1,1,1) and origin in the middle of length
                SpriteRenderer spriteRenderer = beltSegment.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) spriteRenderer.size = new Vector2(segmentCount, .75f);

                // Adjust the collider size and offset
                BoxCollider2D collider = beltSegment.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    collider.size = new Vector2(segmentCount - 0.05f, .75f); // Subtract 0.05 to account for 0.025 margin on each end
                    //collider.offset = new Vector2((length - 0.05f) / 2, 0); // Center the collider
                }

                beltSegment.name = BuildingType.belt.ToString();
                beltSegment.layer = beltLayer;

                foreach (var hologram in beltHolograms)
                {
                    Destroy(hologram);
                }

                beltHolograms.Clear();
                isPlacingBelt = false;

                BeltsModified.Invoke();
                return;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            // Cancel belt placement
            isPlacingBelt = false;
            Destroy(firstBelt);
            foreach (var beltHologram in beltHolograms)
            {
                Destroy(beltHologram);
            }
            beltHolograms.Clear();

        }

        if (isPlacingBelt)
        {   
            if(firstBelt == null) {
                isPlacingBelt = !isPlacingBelt;
                return;
            }
            // Dynamically extend the belt following the mouse
            Vector3 currentMousePosition = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
            currentMousePosition.z = -2;
            // Calculate direction based on the initial hologram's rotation

            Vector3 direction = firstBelt.transform.right;
            float length;

            if(firstBelt.transform.rotation.eulerAngles.z == 0)
            {
                length = initialPosition.x - currentMousePosition.x - 0.5f;
                length *= -1;
            }

            else if (firstBelt.transform.rotation.eulerAngles.z == 180)
            {
                length = initialPosition.x - currentMousePosition.x + 0.5f;
            }

            else if (firstBelt.transform.rotation.eulerAngles.z == 90)
            {
                length = initialPosition.y - currentMousePosition.y - 0.5f;
                length *= -1;
            }

            else if (firstBelt.transform.rotation.eulerAngles.z == 270)
            {
                length = initialPosition.y - currentMousePosition.y + 0.5f;
            }

            else
            {
                length = 0;
            }


            segmentCount = (int)Mathf.Ceil(length / grid.cellSize.x);
            if (segmentCount < 1)
            {
                segmentCount = 1;
            }
            canPlaceBelt = true;
            // Clear previous holograms
            for (int i = 1; i < beltHolograms.Count; i++)
            {
                Destroy(beltHolograms[i]);
            }
            beltHolograms.Clear();
            beltHolograms.Add(firstBelt);
            // Create new holograms

            for (int i = 1; i < segmentCount; i++)
            {
                Vector3 segmentPosition = i * grid.cellSize.x * direction + initialPosition;
                GameObject newHologram = Instantiate(beltPrefab, segmentPosition, firstBelt.transform.rotation);
                newHologram.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1, 0.5f);
                newHologram.name = "BeltHologram";
                newHologram.layer = hologramLayer;
                beltHolograms.Add(newHologram);
            }

            foreach (var hologram in beltHolograms)
            {
                if(IsSpaceClear(hologram)) {
                    hologram.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1, 0.5f);
                }
                else {
                    hologram.GetComponent<SpriteRenderer>().color = new Color(1, 0.2f, 0.2f, 0.5f);
                    canPlaceBelt = false;
                }
            }
        }
    }

        /*
        Vector3 beginning = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        beginning.z = -2;
    
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            end.z = -2;
        }
    
        GameObject buildingType = buildingPrefabs[BuildingType.belt];
        GameObject created = Instantiate(buildingType, SnapToGrid(beginning, grid), hologram.transform.rotation);
        created.name = buildingType.ToString();
        Debug.Log("placed");
        ConveyorBeltSegment newSegment = created.GetComponent<ConveyorBeltSegment>();
        if (newSegment != null)
        {
            conveyorBeltManager.PlaceBeltSegment(newSegment, created.transform.position);
        }
        */
    

    void Rotate(GameObject building)
    {
        if(building == null) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            building.transform.Rotate(0, 0, -90, Space.Self);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            building.transform.Rotate(0, 0, 90, Space.Self);
        }
    }
    bool IsSpaceClear(GameObject building)
     {  
        if(building == null)
        {
            return true;
        }

        if(building.GetComponent<Collider2D>().OverlapCollider(filter, new Collider2D[1]) != 0)
        {
            return false;
        }
        return true;
     }

    void SwitchBuildingType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBuildingType = BuildingType.belt;
            Destroy(hologram);
            CreateHologram();
            Debug.Log("Switched to Belt");
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBuildingType = BuildingType.smelter;
            Destroy(hologram);
            CreateHologram();
            Debug.Log("Switched to Smelter");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBuildingType = BuildingType.constructor;
            Destroy(hologram);
            CreateHologram();
            Debug.Log("Switched to Constructor");
        }
        // Add more else if blocks for other building types as needed
    }

    void CreateHologram()
    {
        GameObject newHologram;
        GameObject building = buildingPrefabs[currentBuildingType];
        Vector3 position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
        position.z = -2;

        if(hologram == null || IsSpaceClear(hologram))
        {
            newHologram = Instantiate(building, position, transform.rotation);
            newHologram.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1, 0.5f);
            newHologram.name = "Hologram";
            newHologram.layer = 8;
            hologram = newHologram;
        }
    }
    /*
    GameObject UnderMouse()
    {
        int layerObject = 8;
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, layerObject);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else return null;
    }
    */

    void EnterBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            deleteMode = false;
            buildMode = !buildMode;

            if (!buildMode)
            {
                Destroy(hologram);
            }
            Debug.Log("Build mode: " + buildMode);
        }
        return;
    }

    void EnterDeleteMode()
    {   
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(buildMode)
            {
                buildMode = false;
                Destroy(hologram);
            }

            deleteMode = !deleteMode;
            Debug.Log("Delete mode: " + deleteMode);
        }
    }

    void BuildMode()
    {
        if(hologram == null)
        {
            CreateHologram();
        }

        if (hologram != null)
        {
            if (buildMode && currentBuildingType != BuildingType.belt)
            {
                hologram.transform.position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
                if (Input.GetMouseButtonDown(0))
                {
                    if (IsSpaceClear(hologram))
                    {
                        Build(currentBuildingType);
                        BuildingPlaced.Invoke();
                    }
                }
            }                
        }

        if (!IsSpaceClear(hologram))
        {
            hologram.GetComponent<SpriteRenderer>().color = new Color(1, 0.2f, 0.2f, 0.5f);
        }
        else
        {
            hologram.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1, 0.5f);
        }
    }
}
