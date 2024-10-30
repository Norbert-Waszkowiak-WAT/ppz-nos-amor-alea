using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Biome Preset", menuName = "New Biome Preset")]
public class BiomePreset : ScriptableObject
{

    public Tile[] tiles;
    public float minHeight;
    public float minMoisture;
    public float minHeat;

    public Tile GetTile ()
    {
        return tiles[Random.Range(0, tiles.Length)];
    }

    public bool MatchCondition (float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
