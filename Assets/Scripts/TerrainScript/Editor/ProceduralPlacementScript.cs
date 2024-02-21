using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ProceduralPlacementScript : EditorWindow
{/*
    
    private Texture2D noiseMap;
    private float density;
    private GameObject prefabs;
    private MapDisplay mapDisplay;


    [MenuItem("Tools/Aquwin Custom/Terrain/Placement")]
    public static void ShowWindow()
    {
        GetWindow<ProceduralPlacementScript>("Placement");
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        noiseMap = (Texture2D)EditorGUILayout.ObjectField(" Noise Map Texture", noiseMap, typeof(Texture2D), false);
        if(GUILayout.Button("Generate Noise"))
        {
            noiseMap =  (Texture2D)mapDisplay.textureRenderer.material.mainTexture;
        }
        EditorGUILayout.EndHorizontal();

        density = EditorGUILayout.Slider("Density", density, 0, 10000);
        prefabs = (GameObject)EditorGUILayout.ObjectField("Object prefabs", prefabs, typeof(GameObject), false);


        if(GUILayout.Button("Place Objects"))
        {
            GameObject Terrain = GameObject.Find("TerrainMesh");
            if (Terrain!=null)
            {
                PlaceObject(Terrain, noiseMap, density, prefabs);

            }
        }
    }

    public static void PlaceObject(GameObject meshObject, Texture2D noiseMap, float density, GameObject prefab)
    {
        Transform parent = new GameObject("PlaceObject").transform;
        Mesh mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;

        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Loop through each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            // Get the vertex position
            Vector3 vertexPosition = meshObject.transform.TransformPoint(vertices[i]);

            // Convert the vertex position to local space of the noise map
            Vector2Int noiseMapPosition = new Vector2Int(
                Mathf.RoundToInt(vertexPosition.x),
                Mathf.RoundToInt(vertexPosition.z)
            );

            // Check if the noise map position is within the bounds of the noise map
            if (noiseMapPosition.x >= 0 && noiseMapPosition.x < noiseMap.width &&
                noiseMapPosition.y >= 0 && noiseMapPosition.y < noiseMap.height)
            {
                // Get the noise map value at the position
                float noiseMapValue = noiseMap.GetPixel(noiseMapPosition.x, noiseMapPosition.y).g;

                // Check if the noise map value meets the density threshold
                if (noiseMapValue > 1 - density)
                {
                    // Instantiate the prefab at the vertex position
                    GameObject obj = Instantiate(prefab, vertexPosition, Quaternion.identity, parent);
                    // Set the prefab's position to align with the mesh vertex
                    obj.transform.position = vertexPosition;
                }
            }
        }
    }


    private void OnEnable()
    {
        noiseMap = LoadTextureFromFile("Assets/Custom/noisetexture.png");
    }
    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
            return texture;
        else
            return null;
    }*/
}
