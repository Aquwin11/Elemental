 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool gamePause;
    [SerializeField] int CurrentSceneIndex=0;
    public float coinsCollected = 0;
    [Header("SceneTransitionOrb")]
    [SerializeField] private Transform TrasitionOrb;
    public static GameManager gameManagerInstance;
    void Awake()
    {
        if (gameManagerInstance != null && gameManagerInstance != this)
        {
            Destroy(obj: this);
        }
        else
        {
            gameManagerInstance = this;
            DontDestroyOnLoad(this);
        }
        CurrentSceneIndex = 0;
    }

    public void ScaleUpOrb()
    {
        TrasitionOrb.DOScale(Vector3.one * (60f), 1.25f);
    }

    public void ScaleDownOrb()
    {
        TrasitionOrb.DOScale(Vector3.zero, 1.25f);
    }


    public void NewLoadNextLevel()
    {
        StopAllCoroutines();
        StartCoroutine(SCeneTransitionNewGame());
        ResetCoinVal();
    }
    public void PreviousLoadNextLevel()
    {
        StopAllCoroutines();
        StartCoroutine(SCeneTransitionLoadGame());
        ResetCoinVal();
    }

    public void ReturntoMianMenuLevel()
    {
        StopAllCoroutines();
        StartCoroutine(GoBackToMainMenu());
    }

    IEnumerator GoBackToMainMenu()
    {
        DataPersistence.dataPersistenceInstance.SaveData();
        ScaleUpOrb();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        CurrentSceneIndex = 0;
        yield return new WaitForSeconds(2f);
        ScaleDownOrb();
    }

    public void LoadPreviousLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        ScaleUpOrb();
    }


    IEnumerator SCeneTransitionNewGame()
    {
        gamePause = false;
        ScaleUpOrb();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(0.25f); 
        DataPersistence.dataPersistenceInstance.OnStartGame();
        GameObject.FindObjectOfType<MapGenerator>().NewGameMapGenerator();
        //ScaleDownOrb();
        CurrentSceneIndex = 1;
        yield return new WaitForSeconds(3f);
        ScaleDownOrb();
    }

    IEnumerator SCeneTransitionLoadGame()
    {
        gamePause = false;
        ScaleUpOrb();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(0.25f);
        DataPersistence.dataPersistenceInstance.OnStartGame();
        GameObject.FindObjectOfType<MapGenerator>().LoadGameMapGenerator();
        CurrentSceneIndex = 1;
        yield return new WaitForSeconds(3f);
        ScaleDownOrb();
    }

    public void IncreamentCoinVal()
    {
        coinsCollected += 1;
    }
    public void ResetCoinVal()
    {
        coinsCollected = 0;
    }

    public void DeleteLoadFile()
    {
        StartCoroutine(DeleteFile());
    }
    IEnumerator DeleteFile()
    {
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Check if file exsist");
        if (File.Exists(Application.persistentDataPath + "/ElementalData.game"))
        {
            Debug.Log("Yes file exsist");
            File.Delete(Application.persistentDataPath + "/ElementalData.game");
        }
        else
        {
            Debug.Log("Delete file doesn't ");
        }
    }

    public void OnApplicationQuit()
    {
        if(CurrentSceneIndex==1)
        {
            DataPersistence.dataPersistenceInstance.SaveData();
        }
    }
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public Color32 TestCOlor;
    public void ChangeColor()
    {
        TrasitionOrb.GetComponent<Image>().color = TestCOlor;
    }

}
