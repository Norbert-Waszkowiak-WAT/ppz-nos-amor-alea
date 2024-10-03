using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent BuildingPlaced;

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
    public GameObject hologram = null;

    public GameObject debugObject;

    BuildingType currentBuildingType;
    // Dictionary to map BuildingType to GameObject
    private Dictionary<BuildingType, GameObject> buildingPrefabs;
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
            BuildMode(); 
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
            position.z = -2;
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

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
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

            if (buildMode)
            {
                CreateHologram();
            }

            else
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
            buildMode = false;
            deleteMode = !deleteMode;
            Debug.Log("Delete mode: " + deleteMode);
        }
    }

    void BuildMode()
    {
        if (hologram != null)
        {
            if (buildMode)
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
}
