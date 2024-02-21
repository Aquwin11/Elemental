using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap,int width,int height)
    {
        Texture2D texture2D = new Texture2D(width, height);
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.SetPixels(colorMap);
        texture2D.Apply();
        return texture2D;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightmap)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightmap[x, y]);
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }
}
