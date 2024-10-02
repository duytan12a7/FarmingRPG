using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Add SaveLoadManager = -80 to Project Settings -> Script Execution Order
/// </summary>
public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    public List<ISaveable> iSaveableObjectList;

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
}
