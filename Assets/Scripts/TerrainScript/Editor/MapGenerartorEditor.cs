#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor (typeof(MapGenerator))]
public class MapGenerartorEditor : Editor
{/*
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if(DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {

                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generator"))
        {
            mapGen.GenerateMap();
        }
    }
    public void DeleteCustomFile()
    {
        string filePath = "Assets/Custom/noisetexture.png"; // Replace with the path to your custom file

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Delete the file
            File.Delete(filePath);
            Debug.Log("Custom file deleted: " + filePath);
        }
        else
        {
            Debug.LogWarning("Custom file not found: " + filePath);
        }
    }*/
}
#endif
