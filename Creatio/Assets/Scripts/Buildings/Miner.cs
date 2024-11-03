using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Miner : MonoBehaviour
{
    public int extractionRate;
    public int outputBuffer;
    public int resourceType;
    public int purity;

    public Tilemap resourceTilemap;


    // Start is called before the first frame update
    void Start()
    {
        CheckResourceTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParams(ResourceTile resourceTile) {
        resourceTile.GetParams(out purity, out resourceType);
    }

    void CheckResourceTile() {
        Vector3Int minerPosition = resourceTilemap.WorldToCell(transform.position);
        TileBase tile = (ResourceTile) resourceTilemap.GetTile(minerPosition);

        if (tile is ResourceTile resourceTile)
        {
            SetParams(resourceTile);
            Debug.Log($"Tile found under miner: {resourceTile.name}, Purity: {purity}, ResourceType: {resourceType}");
        }
        else
        {
            Debug.LogWarning("No resource tile found under miner.");
        }
    }
}
