using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DataPersistence:MonoBehaviour
{
    [Header("File Strage Configure")]
    [SerializeField] private string fileName;
    public static DataPersistence dataPersistenceInstance { get; private set; }
    private List<IDataPersistence> dataPersistences;
    private GameData gameData;
    private FileDataHandler dataHandler;
    public void Awake()
    {
        if(dataPersistenceInstance!=null)
        {
            Destroy(obj: this);
        }
        dataPersistenceInstance = this;
    }
    private void Start()
    {
    }
    private List<IDataPersistence> FindAllDataPersistence()
    {
        IEnumerable<IDataPersistence> dataPersistences = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        Debug.Log("datapersistence " + dataPersistences.Count());
        return new List<IDataPersistence>(dataPersistences);
    }
    public void FindAllDatapersisence()
    {
        this.dataPersistences = FindAllDataPersistence();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
        Debug.Log(" Check if new data ");
    }
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if(this.gameData==null)
        {
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistences)
        {
            dataPersistenceObj.LoadData(gameData);
            Debug.Log(" new Loaded data " + gameData.meshGeneratorSeed);
        }
      
    }
    public void SaveData()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistences)
        {
            dataPersistenceObj.SavaData(gameData);
        }
        dataHandler.Save(gameData);
        Debug.Log("Saved data " + gameData.meshGeneratorSeed);
    }

    public bool CheckForLoadFile()
    {
        if (this.gameData == null)
        {
            return false;
        }
        return true;
    }

    public void OnStartGame()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        FindAllDatapersisence();
        LoadGame();
    }

    
}
