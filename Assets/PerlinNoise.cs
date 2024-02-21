using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;

    public float scale = 20;
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color color = CalculateColor(i,j);
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply(); 
        return texture;
    }

    private Color CalculateColor(int i,int j)
    {
        float xCoord = (float)i / width * scale;
        float yCoord = (float)j / height * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
