using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    bool buildMode = false;
    public GameObject building;
    public Grid[,] grid;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {        
        if(Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            Debug.Log("Build mode: " + buildMode);
        }   

        if (buildMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spawnPosition.z = -2;
                Instantiate(building, SnapToGrid(spawnPosition, grid), Quaternion.identity);
                Debug.Log("placed");
            }
        }
    }

    Vector3 SnapToGrid(Vector3 Position, Grid[,] grid)
    {
        Vector3 gridSize = grid.cellSize;
        Position.x = Mathf.Round(Position.x / gridSize.x) * gridSize.x + (gridSize.x / 2);
        Position.y = Mathf.Round(Position.y / gridSize.y) * gridSize.y + (gridSize.y / 2);
        return Position;
    }
}
