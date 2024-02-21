using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawer : MonoBehaviour
{
    public static ObjectSpawer Instance;
    public List<ObjectToSpawn> allObjects;

    public void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(obj: this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        GenerateObjects();
    }
    public void GenerateObjects()
    {
        for (int size = 0; size < allObjects.Count; size++)
        {
            ObjectToSpawn gO = allObjects[size];
            for (int i = 0; i < gO.numberOfObjects; i++)
            {
                GameObject objectCreated = Instantiate(gO.objectPrefab, Vector2.zero, Quaternion.identity, gO.objectParent);
                objectCreated.SetActive(false);
                gO.objectList.Add(objectCreated);
            }
        }
    }

    public GameObject GetObject(ObjectType checkObjectType, Vector3 spawnPosition,Quaternion rotation)
    {
        foreach (var Objectypes in allObjects)
        {
            if (Objectypes.objectType == checkObjectType)
            {
                foreach (var Object in Objectypes.objectList)
                {
                    if (!Object.activeSelf)
                    {
                        Object.transform.position = spawnPosition;
                        Object.transform.rotation = rotation;
                        Object.SetActive(true);
                        return Object;
                    }
                }
                GenerateNewObject(checkObjectType);
                Objectypes.objectList[Objectypes.objectList.Count - 1].transform.position = spawnPosition;
                Objectypes.objectList[Objectypes.objectList.Count - 1].transform.rotation = rotation;
                Objectypes.objectList[Objectypes.objectList.Count - 1].SetActive(true);
                return Objectypes.objectList[Objectypes.objectList.Count - 1];
            }
        }
        return null;
    }

    private void GenerateNewObject(ObjectType checkObjectType)
    {
        foreach (var Objectypes in allObjects)
        {
            if (Objectypes.objectType == checkObjectType)
            {
                GameObject newObject = Instantiate(Objectypes.objectPrefab, Vector2.zero, Quaternion.identity, Objectypes.objectParent);
                newObject.transform.eulerAngles = new Vector3(0, 0, 90);
                newObject.SetActive(false);
                Objectypes.objectList.Add(newObject);
            }
        }
    }

    [System.Serializable]
    public class ObjectToSpawn
    {
        [Header("ObjectSetting")]
        public string objectName;
        public ObjectType objectType;
        public float numberOfObjects;
        public GameObject objectPrefab;
        public Transform objectParent;
        public List<GameObject> objectList = new List<GameObject>();
    }

    public enum ObjectType
    {
        PlayerBullet,
        EnemyBullet,
        JumpEffect,
        ExplosionEffect
    }
}
