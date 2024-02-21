using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffMap
{
    public static float[,] GenerateFallOfMap(int size, float startPoint, float endPoint, bool enableFallEndPoints)
    {
        float[,] map = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xPos = x / (float)size * 2 - 1;
                float yPos = y / (float)size * 2 - 1;
                Vector2 position = new Vector2(xPos, yPos);
                float t = Mathf.Max(Mathf.Abs(position.x), Mathf.Abs(position.y));

                if (enableFallEndPoints)
                {
                    if (t < startPoint)
                    {
                        map[x, y] = 1;
                    }
                    else if (t > endPoint)
                    {
                        map[x, y] = 0;
                    }
                    else
                    {
                        map[x, y] = Mathf.SmoothStep(1, 0, Mathf.InverseLerp(startPoint, endPoint, t));
                    }
                }
                else
                {
                    map[x, y] = Evaluate(t);
                }
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }


}
