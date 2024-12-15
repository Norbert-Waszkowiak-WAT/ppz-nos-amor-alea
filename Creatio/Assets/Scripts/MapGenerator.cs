using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public static float[,] Generate (int width, int height, float scale, Wave[] waves, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; < height; y++) { 
                float samplePosX = (float)x * scale + offset.x;
                float samplePosY = (float)y * scale + offset.y;

                float normalization = 0.0f;
                foreach(Wave wave in waves)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(samplePosX * wave.frequency + wave.seed, samplePosY * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;

            }

        }
        return noiseMap;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }
}
