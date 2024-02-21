using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinParent : MonoBehaviour
{
    [SerializeField] MainMenuManager menuManager;
    public int maxCoins;
    public Text meshPro;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameManagerInstance != null)
        {
            meshPro.text = "" + GameManager.gameManagerInstance.coinsCollected + "/" + maxCoins.ToString();
            if (GameManager.gameManagerInstance.coinsCollected == maxCoins && !GameManager.gameManagerInstance.gamePause)
            {
                menuManager.onWin();
            }
        }

    }
}
