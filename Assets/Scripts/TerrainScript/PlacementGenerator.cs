using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
public class PlacementGenerator : MonoBehaviour
{

    public bool autoUpdate;
    
    public List<GenerateObjects> generateObjects;
    public GameObject playerGameObject;


    [Header("Enemy Placement")]
    public GameObject enemyPrefabs;
    public int enemyDensity;
    public GameObject enemyParent;
    [Space]
    public Vector3 enemyPositionsOffsetVec;
    public float enemyMinHeight;
    public float enemyMaxHeight;
    public Vector2 enemyXRange;
    public Vector2 enemyZRange;

    [Range(0, 1)] public float rotateTowardsNormal;
    public Vector2 rotationRange;
    public void Generate()
    {
        Clear();
        for (int size = 0; size < generateObjects.Count; size++)
        {
            GenerateObjects gO = generateObjects[size];
            for (int i = 0; i < gO.density; i++)
            {
                float sampleX = Random.Range(gO.xRange.x, gO.xRange.y);
                float sampleY = Random.Range(gO.zRange.x, gO.zRange.y);
                Vector3 rayStart = new Vector3(sampleX, gO.maxHeight, sampleY);
                if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                    continue;
                if (hit.point.y < gO.minHeight)
                    continue;
#if UNITY_EDITOR
                GameObject instantiatePrefabs = (GameObject)PrefabUtility.InstantiatePrefab(gO.prefabs[Random.Range(0, gO.prefabs.Count)], transform);
#else
                GameObject instantiatePrefabs = Instantiate(gO.prefabs[Random.Range(0, gO.prefabs.Count)], transform);
#endif
                instantiatePrefabs.transform.position = hit.point+gO.positionsOffsetVec;
                instantiatePrefabs.transform.Rotate(Vector3.up, Random.Range
                    (gO.rotationRange.x, gO.rotationRange.y), Space.Self);
                instantiatePrefabs.transform.rotation = Quaternion.Lerp(transform.rotation,
                    transform.rotation * Quaternion.FromToRotation
                    (instantiatePrefabs.transform.up, hit.normal), gO.rotateTowardsNormal);
                instantiatePrefabs.transform.localScale = new Vector3(Random.Range(gO.minScale.x, gO.maxScale.x),
                    Random.Range(gO.minScale.y, gO.maxScale.y),
                    Random.Range(gO.minScale.z, gO.maxScale.z));
            }
        }
        EnablePlayerObject();
    }

    public void EnablePlayerObject()
    {
        if(playerGameObject!=null)
        {
            playerGameObject.SetActive(true);
        }
        //GenerateEnemy();
    }
    public void GenerateEnemy()
    {
       
    }
    public void ClearEnemy()
    {
        for (int i = 0; i < enemyParent.transform.childCount; i++)
        {
            DestroyImmediate(enemyParent.transform.GetChild(i).gameObject);
        }
    }

    public void Clear()
    {
        while(transform.childCount!=0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
    }
    [System.Serializable]
    public class GenerateObjects
    {
        public string objectType;
        public List<GameObject> prefabs;

        [Header("Raycast Settings")]
        public int density;

        [Space]
        public Vector3 positionsOffsetVec;
        public float minHeight;
        public float maxHeight;
        public Vector2 xRange;
        public Vector2 zRange;

        [Header("Prefabs Variation Settings")]
        [Range(0, 1)] public float rotateTowardsNormal;
        public Vector2 rotationRange;
        public Vector3 minScale;
        public Vector3 maxScale;
    }
    
}
