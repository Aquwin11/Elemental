using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public PlacementGenerator placementGenerator;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        //textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void AddCollision()
    {
        if (meshRenderer.gameObject.GetComponent<MeshCollider>() == null)
        {
            //Debug.Log(meshRenderer.gameObject.name + "meshRenderer.gameObject");
            meshRenderer.gameObject.AddComponent<MeshCollider>();
            Invoke("placeObjects", 0.5f);
        }
    }

    public void SaveTextureToFile(string filePath, Texture2D texture)
    {
        // Check if the texture exists
        if (texture == null)
        {
            Debug.LogError("No texture assigned.");
            return;
        }

        // Encode the texture to PNG format
        byte[] textureData = texture.EncodeToPNG();

        // Create the directory if it doesn't exist
        string directoryPath = Path.GetDirectoryName(filePath);
        Directory.CreateDirectory(directoryPath);

        // Write the texture data to the file
        File.WriteAllBytes(filePath, textureData);

        Debug.Log("Texture saved to: " + filePath);
    }
    private bool IsDirectoryEmpty(string path)
    {
        string[] files = Directory.GetFiles(path);
        return files.Length == 0;
    }

    public void placeObjects()
    {
        placementGenerator.Generate();
    }
}
