using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorTrail : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int dept = 20;
    public int scale = 20;
    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, dept, height);
        terrainData.SetHeights(0, 0, GenerateHeight());
        return terrainData;
    }

    private float[,] GenerateHeight()
    {
        float[,] Heights = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Heights[i, j] = CalCulateHeight(i, j);
            }
        }
        return Heights;
    }

    private float CalCulateHeight(int i, int j)
    {
            float xCoord = (float)i / width * scale;
            float yCoord = (float)j / height * scale;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return sample;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
