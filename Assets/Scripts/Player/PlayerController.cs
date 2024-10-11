using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : SingletonMonobehaviour<PlayerController>, ISaveable
{
    public PlayerMovement PlayerMovement { get; private set; }

    public PlayerAnimation PlayerAnimation { get; private set; }

    public PlayerInputHandler PlayerInput { get; private set; }

    public string ISaveableUniqueID { get; set; }

    public GameObjectSave GameObjectSave { get; set; }

    protected override void Awake()
    {
        base.Awake();

        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        PlayerInput = GetComponent<PlayerInputHandler>();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        IRegisterSaveable();
    }

    private void OnDisable()
    {
        IDeregisterSaveable();
    }

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }

    public void IRegisterSaveable()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void IDeregisterSaveable()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void IStoreSceneData(string sceneName)
    {
        //TODO
    }

    public void IRestoreSceneData(string sceneName)
    {
        //TODO
    }

    //TODO: Don't teleport Scene
    public GameObjectSave ISaveData()
    {
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        SceneSave sceneSave = new();

        Vector3Serializable vector3Serializable = new(transform.position.x, transform.position.y, transform.position.z);

        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>(){
            {"playerPosition", vector3Serializable}
        };

        sceneSave.stringDictionary = new Dictionary<string, string>(){
            {"currentScene", SceneManager.GetActiveScene().name},
            {"playerDirection", PlayerInput.playerDirection.ToString()}
        };

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        return GameObjectSave;
    }


    //TODO: Don't teleport Scene
    public void ILoadData(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                if (sceneSave.stringDictionary == null) return;

                if (sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                {
                    SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                }

                if (sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                {
                    bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                    if (!playerDirFound) return;

                    PlayerInput.playerDirection = direction;
                    SetPlayerDirection(PlayerInput.playerDirection);
                }
            }
        }
    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case (Direction.up):
                EventHandler.CallMovementEvent(idleUp: true);
                break;
            case (Direction.down):
                EventHandler.CallMovementEvent(idleDown: true);
                break;
            case (Direction.left):
                EventHandler.CallMovementEvent(idleLeft: true);
                break;
            case (Direction.right):
                EventHandler.CallMovementEvent(idleRight: true);
                break;
            default:
                EventHandler.CallMovementEvent();
                break;
        }
    }
}
