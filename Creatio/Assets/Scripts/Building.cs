using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{

    bool buildMode = false;
    public Grid grid;
    public enum BuildingType
    {
        belt,
        smelter,
        constructor
    }

    public GameObject beltPrefab;
    public GameObject smelterPrefab;
    public GameObject constructorPrefab;
    private GameObject hologram;
    private SpriteRenderer hologramSprite;
    BuildingType currentBuildingType = BuildingType.belt;
    // Dictionary to map BuildingType to GameObject
    private Dictionary<BuildingType, GameObject> buildingPrefabs;
    private Dictionary<BuildingType, Vector2> buildingSizes;
    // Start is called before the first frame update
    void Start()
    {
           // Initialize the dictionary with your building prefabs
        buildingPrefabs = new Dictionary<BuildingType, GameObject>
        {
            { BuildingType.belt, beltPrefab },
            { BuildingType.smelter, smelterPrefab },
            { BuildingType.constructor, constructorPrefab }
            // Add other building types and their prefabs here
        };

        buildingSizes = new Dictionary<BuildingType, Vector2>
        {
            { BuildingType.belt, new Vector2(1, 1) },
            { BuildingType.smelter, new Vector2(2, 2) },
            { BuildingType.constructor, new Vector2(1, 1) }
            // Add other building types and their sizes here
        };

        hologram = null;
    }

    // Update is called once per frame
    void Update()
    {        
        Rotate();
        SwitchBuildingType();

        if(Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            Debug.Log("Build mode: " + buildMode);
        }   

        if (buildMode)
        {         
            if (hologram == null)
            {
                hologram = Hologram(currentBuildingType);
            }
            else
            {
                hologram.transform.position = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition), grid);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if(IsSpaceClear(hologram)) {
                    Destroy(hologram);
                    Build(currentBuildingType);
                }
            }
        }

        else
        {
            if (hologram != null)
            {
                Destroy(hologram);
            }
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
            Instantiate(buildingPrefab, SnapToGrid(position, grid), transform.rotation);
            Debug.Log("placed");
        }
        else
        {
            Debug.LogError("Building type not found in dictionary");
        }
    }

    void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(0, 0, -90, Space.Self);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, 0, 90, Space.Self);
        }
    }
    bool IsSpaceClear(GameObject building)
     {  
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        if(building.GetComponent<Collider2D>().OverlapCollider(filter, new Collider2D[1]) != 0)
        {
            Debug.Log("Colliding");
            return false;
        }
        return true;
     }

    void SwitchBuildingType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBuildingType = BuildingType.belt;
            Debug.Log("Switched to Belt");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBuildingType = BuildingType.smelter;
            Debug.Log("Switched to Smelter");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBuildingType = BuildingType.constructor;
            Debug.Log("Switched to Constructor");
        }
        // Add more else if blocks for other building types as needed
    }   

    GameObject Hologram(BuildingType buildingType)
    {
        if(IsSpaceClear(hologram))
        {
            if (buildingPrefabs.TryGetValue(buildingType, out GameObject buildingPrefab))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = -2;
                GameObject hologram = Instantiate(buildingPrefab, SnapToGrid(position, grid), transform.rotation);
            }
            else
            {
                Debug.LogError("Building type not found in dictionary");
                return null;
            }

            hologram.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0.7f);
            return hologram;
        }
        else return null;
    }
}
