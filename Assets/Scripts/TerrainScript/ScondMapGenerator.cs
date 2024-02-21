using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScondMapGenerator : MonoBehaviour
{
	public enum DrawMode
	{
		NoiseMap,
		Mesh,
		FallOffMap,
		ColorMap
	}
	public DrawMode drawMode;
	const int mapChunkSize = 241;
	[Range(0, 6)]
	public int levelofDetail;
	/*public int mapChunkSize;
	public int mapChunkSize;*/
	public float noiseScale;

	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;


	public int seed;
	public Vector2 offset;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool autoUpdate;
	public TerrainType[] regions;

	float[,] falloffMap;

	[Header("Fallout Settings")]
	public bool useFallOffMap;
	public bool EnableFallEndPoints;
	[Range(1, 0)]
	public float fallOutStart;
	[Range(1, 0)]
	public float fallOutEnd;

	private void Awake()
	{
		falloffMap = FallOffMap.GenerateFallOfMap(mapChunkSize, fallOutStart, fallOutEnd, EnableFallEndPoints);
	}

	private void Start()
	{
		Invoke("GenerateMap", 0.15f);
		MapDisplay display = FindObjectOfType<MapDisplay>();
		display.AddCollision();
	}
	public void GenerateMap()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
		Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++)
		{
			for (int x = 0; x < mapChunkSize; x++)
			{
				if (useFallOffMap)
				{
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
				{
					if (currentHeight <= regions[i].height)
					{
						colorMap[y * mapChunkSize + x] = regions[i].color;
						break;
					}
				}
			}
		}
		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MesHGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelofDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.FallOffMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffMap.GenerateFallOfMap(mapChunkSize, fallOutStart, fallOutEnd, EnableFallEndPoints)));
		}
	}
	public void DisplayChange()
    {
		levelofDetail = 1;
    }
	void OnValidate()
	{
		/*if (mapChunkSize < 1)
		{
			mapChunkSize = 1;
		}
		if (mapChunkSize < 1)
		{
			mapChunkSize = 1;
		}*/
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 0)
		{
			octaves = 0;
		}
		falloffMap = FallOffMap.GenerateFallOfMap(mapChunkSize, fallOutStart, fallOutEnd, EnableFallEndPoints);
	}

}
