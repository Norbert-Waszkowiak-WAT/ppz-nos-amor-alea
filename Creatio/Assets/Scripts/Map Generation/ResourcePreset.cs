using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Resource Preset", menuName = "New Resource Preset")]
public class ResourcePreset : ScriptableObject
{

    public Tile tile;
    public float minHeight;
    private float maxHeight;
    public float minMoisture;
    private float maxMoisture;
    public float minHeat;
    private float maxHeat;

    public Tile GetTile ()
    {
        return tile;
    }

    public bool MatchCondition (float height, float moisture, float heat)
    {
        return height >= minHeight && height <= maxHeight && moisture >= minMoisture && moisture <= maxMoisture && heat >= minHeat && heat <= maxHeat;
    }

    
    // Start is called before the first frame update
    void Start()
    {  
        maxHeight = minHeight + 0.05f;
        maxMoisture = minMoisture + 0.05f;
        maxHeat = minHeat + 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
