using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenScript : MonoBehaviour,IDataPersistence
{

    [SerializeField] private string id;
    [SerializeField] private string tokenName;
    [SerializeField] private MeshRenderer visual;
    [SerializeField] private SphereCollider tokenCollider;

    public bool isCollected;
    [ContextMenu(" Generate GUID for ID")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!isCollected)
        {
            isCollected = true;
            visualChanges(false);
            if (GameManager.gameManagerInstance != null)
                GameManager.gameManagerInstance.coinsCollected++;
        }
    }
    public void visualChanges(bool state)
    {
        visual.enabled = state;
        tokenCollider.enabled = state;
    }
    public void LoadData(GameData gameData)
    {
        gameData.tokenCollected.TryGetValue(id, out isCollected);
        if(isCollected)
        {
            visualChanges(false);
            if (GameManager.gameManagerInstance != null)
                GameManager.gameManagerInstance.IncreamentCoinVal();
        }
    }

    public void SavaData(GameData gameData)
    {
        if(gameData.tokenCollected.ContainsKey(id))
        {
            gameData.tokenCollected.Remove(id);
        }
        gameData.tokenCollected.Add(id,isCollected);
    }
}
