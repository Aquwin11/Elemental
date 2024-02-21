using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public SerializableDictionary<string, bool> tokenCollected;
    public int meshGeneratorSeed;
    public GameData()
    {
        this.meshGeneratorSeed = 0;
        this.tokenCollected = new SerializableDictionary<string, bool>(); 
    }
}
