using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Add SaveLoadManager = -80 to Project Settings -> Script Execution Order
/// </summary>
public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    public List<ISaveable> iSaveableObjectList;
    public GameSave gameSave;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }

    public void StoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger store scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.IStoreSceneData(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger restore scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.IRestoreSceneData(SceneManager.GetActiveScene().name);
        }
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter binaryFormatter = new();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            gameSave = (GameSave)binaryFormatter.Deserialize(fileStream);

            for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ILoadData(gameSave);
                }
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            fileStream.Close();
        }
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveData());
        }

        BinaryFormatter binaryFormatter = new();

        FileStream fileStream = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        binaryFormatter.Serialize(fileStream, gameSave);

        fileStream.Close();

        UIManager.Instance.DisablePauseMenu();
    }
}
