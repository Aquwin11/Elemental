using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour , IDataPersistence
{
	public enum DrawMode
    {
		NoiseMap,
		Mesh,
		FallOffMap,
		ColorMap
    }
	public DrawMode drawMode;
	const int mapChunkSize=241;
	[Range(0,6)]
	public int levelofDetail;
	/*public int mapChunkSize;
	public int mapChunkSize;*/
	public float noiseScale;

	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;

	
	[SerializeField]private int seed;
	public Vector2 offset;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool autoUpdate;
	public bool testScene;
	public TerrainType[] regions;

	float[,] falloffMap;

	[Header("Fallout Settings")]
	public bool useFallOffMap;
	public bool EnableFallEndPoints;
	[Range(0,1)]
	public float fallOutStart;
	[Range(0, 1)]
	public float fallOutEnd;


	public GameObject storyCanvasObject;
    private void Awake()
    {
		falloffMap = FallOffMap.GenerateFallOfMap(mapChunkSize,fallOutStart,fallOutEnd,EnableFallEndPoints);
    }
    private void Start()
    {
        if(testScene)
        {
			LoadGameMapGenerator();
		}

    }

    public void NewGameMapGenerator()
    {
		seed = GenerateRandomSeed();
		GenerateMap();
		
		if (gameObject.GetComponent<MapDisplay>() != null)
		{
			gameObject.GetComponent<MapDisplay>().AddCollision();

		}
		if (GameManager.gameManagerInstance != null)
			GameManager.gameManagerInstance.gamePause = true;

		storyCanvasObject.SetActive(true);
	}
	public void LoadGameMapGenerator()
	{
		GenerateMap();
		if (gameObject.GetComponent<MapDisplay>() != null)
		{
			gameObject.GetComponent<MapDisplay>().AddCollision();
		}
	}
	public int GenerateRandomSeed()
    {
		int minSeed = 1;
		int maxSeed = 100;
		int finalSeed = Random.Range(minSeed, maxSeed);
		return finalSeed;
    }
	public void LoadData(GameData gameData)
	{
		this.seed = gameData.meshGeneratorSeed;
		Debug.Log(seed + "seedValue");
	}

	/*IEnumerator EnableCollider()
    {
		yield return new WaitForSeconds(0.15f);
		display
    }*/
	public void GenerateMap()
	{
		
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
		Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
				if(useFallOffMap)
                {
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
				float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
					if(currentHeight <= regions[i].height)
                    {
						colorMap[y * mapChunkSize + x] = regions[i].color;
						break;
                    }
                }
            }
        }
		Debug.Log("New Seed" + seed);
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
		falloffMap = FallOffMap.GenerateFallOfMap(mapChunkSize, fallOutStart, fallOutEnd,EnableFallEndPoints);
	}


    public void SavaData(GameData gameData)
    {
		Debug.Log(gameData.meshGeneratorSeed + "seedValue save");
		gameData.meshGeneratorSeed = this.seed;
		Debug.Log(seed + "seedValue save");
	}
}
[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}