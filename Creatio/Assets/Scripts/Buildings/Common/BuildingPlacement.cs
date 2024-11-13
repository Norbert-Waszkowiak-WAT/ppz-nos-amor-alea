using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacement : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent BuildingPlaced;
    //public UnityEngine.Events.UnityEvent BeltsModified;

    public bool buildMode;
    public bool deleteMode;
    public Grid grid;
    public Tilemap resourceTilemap;

    

    //public GameObject beltPrefab;
    public List<GameObject> buildingPrefabs;
    [SerializeField] BuildingType currentBuildingType;



    //GameObject hologram = null;
    ContactFilter2D filter;

    [SerializeField] bool isPlacingBelt = false;

    Vector3 initialPosition;
    int beltLength;
    GameObject hologram = null;

    // List<GameObject> holograms = new List<GameObject>();
    private int hologramLayer;

    enum BuildingType {
        belt,
        miner,
        smelter,
        constructor,
        assembler,
        producer,
        refinery
    }


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
        buildMode = false;
        deleteMode = false;

        filter.layerMask = LayerMask.GetMask("ConveyorBelts") | LayerMask.GetMask("Buildings") | LayerMask.GetMask("Large Flora");
        filter.useLayerMask = true;
        

        currentBuildingType = BuildingType.belt;
        hologram = Instantiate(buildingPrefabs[(int)currentBuildingType]);
        hologram.GetComponent<SpriteRenderer>().sortingOrder = 1;
        RefreshHologram();
        hologram.SetActive(false);
        Debug.Log("Hologram created, Inactive");
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
                BuildBelt();
            }
            else {
                UpdateHologramPosition(); 
                BuildMode();
            }
        }  

        // if(Input.GetKeyDown(KeyCode.G)) {
            // Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // position.z = -3;
            // Instantiate(debugObject, SnapToGrid(position, grid), transform.rotation);
            // Debug.Log("Debug Object placed");
        // }     
    }

    Vector3 SnapToGrid(Vector3 Position, Grid grid)
    {
        Vector3 gridSize = grid.cellSize;
        Position.x = Mathf.FloorToInt(Position.x / gridSize.x) + gridSize.x/2;
        Position.y = Mathf.FloorToInt(Position.y / gridSize.y) + gridSize.y/2;
        Position.z = -4;
        return Position;
    }

    void Build(GameObject buildingType)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = -4;
        GameObject created = Instantiate(buildingType, SnapToGrid(position, grid), hologram.transform.rotation);
        created.name = "building " + buildingType.ToString();
        created.GetComponent<Building>().SetManager(this);
        created.layer = LayerMask.NameToLayer("Buildings");
        created.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        created.SetActive(true);

        if(created.GetComponent<Miner>() != null) {
            Vector3Int minerPosition = resourceTilemap.WorldToCell(created.transform.position);
            minerPosition.z = 0;

            created.GetComponent<Miner>().SetParams(resourceTilemap.GetTile(minerPosition) as ResourceTile);
            Debug.Log(resourceTilemap.GetTile(minerPosition));
            Debug.Log("placed" + created.name + " at " + minerPosition);
        }
    }

    void CreateBelt() {
        Vector3 position = hologram.transform.position;
        position.z = -2;
        GameObject createdBelt = Instantiate(buildingPrefabs[(int)BuildingType.belt], position, hologram.transform.rotation);
        
        createdBelt.GetComponent<SpriteRenderer>().size = new Vector2(beltLength, 0.75f);
        createdBelt.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        createdBelt.GetComponent<BoxCollider2D>().size = new Vector2(beltLength - 0.05f, 0.75f);

        createdBelt.name = BuildingType.belt.ToString();
        createdBelt.layer = LayerMask.NameToLayer("ConveyorBelts");

        createdBelt.GetComponent<Building>().SetManager(this);
        createdBelt.GetComponent<ConveyorBeltSegment>().Initialize(this, beltLength);
    }

    void BuildBelt() 
    {           
        if (!isPlacingBelt)
        {
            Vector3 position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
            position.z = -2;
            hologram.transform.position = position;
            //Debug.Log("Hologram moved to mouse position");
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(!isPlacingBelt) {
                // First click: create the first hologram
                initialPosition = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
                hologram.transform.position = initialPosition;
                isPlacingBelt = true;
                Debug.Log("First click");
                return;
            } 
            
            else
            {
                // Second click: place the belt segment
                CreateBelt();
                beltLength = 1;
                isPlacingBelt = false;
                UpdateHologramPosition();
                RefreshHologram();
                BuildingPlaced.Invoke();
                return;
            } 
        }

        if (Input.GetMouseButtonDown(1)) {
            // Cancel belt placement
            isPlacingBelt = false;
            beltLength = 1;
            RefreshHologram();
            Debug.Log("Belt placement cancelled");
            return;
        }

        if (isPlacingBelt)
        {   
            Vector3 finalPosition = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
            finalPosition = SnapToAxis(initialPosition, finalPosition);
            PlaceBeltHologram(initialPosition, finalPosition);
            //Debug.Log("is placing belt");
            IsSpaceClear(hologram);
        } 
    }

    private Vector3 SnapToAxis(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            end.y = start.y;
        }
        else
        {
            end.x = start.x;
        }
        return end;
    }

    private void PlaceBeltHologram(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        beltLength = 1;
        beltLength += (int)Vector3.Distance(start, end);

        hologram.transform.position = start + direction * 0.5f;

        //hologram.transform.position = start + direction * ((beltLength - 1) / 2);
        //Debug.Log(direction * ((beltLength - 1) / 2));
        hologram.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        hologram.transform.rotation = Quaternion.Euler(0, 0, hologram.transform.rotation.eulerAngles.z + 90);
        hologram.GetComponent<SpriteRenderer>().size = new Vector2(beltLength, 0.75f);
    }

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
        if(building.GetComponent<Collider2D>().OverlapCollider(filter, new Collider2D[1]) == 0)
        {
            if(currentBuildingType != BuildingType.miner) {
                //Debug.Log("Space clear by collider check");
                return true;
            }

            else {
                Vector3Int minerPosition = resourceTilemap.WorldToCell(building.transform.position);
                minerPosition.z = 0;
                if(resourceTilemap.GetTile(minerPosition) is ResourceTile) 
                {
                    //Debug.Log("Resource tile found under miner");
                    return true;
                }
            //Debug.Log("Resource tile not found under miner: " + minerPosition);
            }
        }

        //Debug.Log("Space not clear by default statement");
        return false;
    }

    void SwitchBuildingType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBuildingType = BuildingType.belt;
            RefreshHologram();
            Debug.Log("Switched to Belt");
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBuildingType = BuildingType.miner;
            RefreshHologram();
            Debug.Log("Switched to Miner");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBuildingType = BuildingType.smelter;
            RefreshHologram();
            Debug.Log("Switched to Smelter");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentBuildingType = BuildingType.constructor;
            RefreshHologram();
            Debug.Log("Switched to Constructor");
        }
        // Add more else if blocks for other building types as needed
    }

    void RefreshHologram()
    {
        SpriteRenderer prefabRenderer = buildingPrefabs[(int)currentBuildingType].GetComponent<SpriteRenderer>();
        SpriteRenderer hologramRenderer = hologram.GetComponent<SpriteRenderer>();

        hologramRenderer.sprite = prefabRenderer.sprite;
        hologramRenderer.size = prefabRenderer.size;
        hologramRenderer.drawMode = prefabRenderer.drawMode;
        hologramRenderer.color = new Color(0.2f, 0.2f, 1, 0.5f);

        hologram.transform.rotation = Quaternion.identity;
        hologram.transform.localScale = Vector3.one;

        hologram.GetComponent<BoxCollider2D>().size = buildingPrefabs[(int)currentBuildingType].GetComponent<BoxCollider2D>().size;

        hologram.name = "Hologram";
        hologram.layer = LayerMask.NameToLayer("Hologram");
        Debug.Log("Hologram refreshed");

        if(hologram.GetComponent<ConveyorBeltSegment>() != null) hologram.GetComponent<ConveyorBeltSegment>().enabled = false;

    }

    void UpdateHologramPosition() {
        Vector3 position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
        position.z = -4;
        hologram.transform.position = position;
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

            if(buildMode) {
                isPlacingBelt = false;
                beltLength = 1;
                hologram.SetActive(true);
                RefreshHologram();
                Debug.Log("Build mode entered");
                return;
            }

            else {
                hologram.SetActive(false);
                isPlacingBelt = false;
                beltLength = 1;
                Debug.Log("Build mode exited");
                return;
            }
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
                hologram.SetActive(false);
                Debug.Log("Build mode exited");
            }

            deleteMode = !deleteMode;
            Debug.Log("Delete mode: " + deleteMode);
        }
    }

    void BuildMode()
    {
        Vector3 position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
        position.z = -4;
        hologram.transform.position = position;
        if (Input.GetMouseButtonDown(0))
        {
            if (IsSpaceClear(hologram))
            {
                Build(buildingPrefabs[(int)currentBuildingType]);
                BuildingPlaced.Invoke();
            }
        }    

        if (IsSpaceClear(hologram))
        {
            hologram.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 1f, 0.5f);
            //Debug.Log("Space clear");
        }
        else
        {
            hologram.GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 0.2f, 0.5f);
        }
    }
}
