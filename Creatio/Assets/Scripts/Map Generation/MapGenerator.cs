using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    public BiomePreset[] biomes;
    public Tilemap tilemap;
    private Tile tile;

    public ResourcePreset[] resources;
    public Tilemap resourceTilemap;
    public int maxResources;

    //public GameObject tilePrefab;

    [Header("Dimensions")]
    public int width;
    public int height;
    public float scale = 1.0f;
    float frequency;

    public int edgeBuffer;


    public Vector2 offset;


    [Header("Height Map")]
    public Wave[] heightWaves;
    public float[,] heightMap;
    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    private float[,] moistureMap;
    [Header("Heat Map")]
    public Wave[] heatWaves;
    private float[,] heatMap;


    void GenerateMap ()
    {
        RandomizeWaves(heightWaves);
        RandomizeWaves(moistureWaves);
        RandomizeWaves(heatWaves);

        // height map
        heightMap = NoiseGenerator.Generate(width, height, scale, heightWaves, offset);
        // moisture map
        moistureMap = NoiseGenerator.Generate(width, height, scale, moistureWaves, offset);
        // heat map
        heatMap = NoiseGenerator.Generate(width, height, scale, heatWaves, offset);

        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).GetTile();
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);      
                //Debug.Log("Tile: " + tile.name + " at " + x + ", " + y);    
            }
        }
    }

    void generateResources(int maxResources) {

        if(maxResources == 0) {
            return;
        }

        int currentResources = 0;

        for(int x = edgeBuffer; x < width - edgeBuffer; ++x)
        {
            for(int y = edgeBuffer; y < height - edgeBuffer; ++y)
            {   
                if(currentResources >= maxResources || Random.Range(0f, 100f) > 0.05f) {
                    continue;
                }

                ResourcePreset resource = GetResource();

                if(resource != null) {
                    resourceTilemap.SetTile(new Vector3Int(x, y, 0), resource.GetTile());
                    currentResources++;
                    Debug.Log("Resource: " + resource.name + " at " + x + ", " + y);
                }
            }
        }
        Debug.Log("Resources generated: " + currentResources);
    }

    void RandomizeWaves (Wave[] waves)
    {
        foreach(Wave wave in waves)
        {
            wave.Randomize();
        }
    }

    BiomePreset GetBiome (float height, float moisture, float heat)
    {
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();
        foreach(BiomePreset biome in biomes)
        {
            if(biome.MatchCondition(height, moisture, heat))
            {
                biomeTemp.Add(new BiomeTempData(biome));                
            }
        }

        float curVal = 0.0f;
        BiomePreset biomeToReturn = null;
        foreach(BiomeTempData biome in biomeTemp)
        {
            if(biomeToReturn == null)
            {
                biomeToReturn = biome.biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
            else
            {
                if(biome.GetDiffValue(height, moisture, heat) < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat);
                }
            }
        }
        if(biomeToReturn == null)
            biomeToReturn = biomes[0];
        return biomeToReturn;

    }

    ResourcePreset GetResource ()
    {
        return resources[Random.Range(0, resources.Length)];
    }


    // Start is called before the first frame update
    void Start()
    {

        tilemap.size = new Vector3Int(width, height, 0);

        // tilemap.layoutGrid.transform.localScale = new Vector3(scale, scale, 1);
        // Debug.Log(tilemap.size + " " + tilemap.layoutGrid.transform.localScale);

    
        GenerateMap();
        generateResources(maxResources + Random.Range(-2, 2));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class BiomeTempData
{
    public BiomePreset biome;
    public BiomeTempData (BiomePreset preset)
    {
        biome = preset;
    }
        
    public float GetDiffValue (float height, float moisture, float heat)
    {
        return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
    }
}