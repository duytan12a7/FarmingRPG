using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    // Reference to parent transform for items
    private Transform parentItem;

    // Prefab to instantiate items from
    [SerializeField] private GameObject itemPrefab = null;

    // Unique ID for saving
    public string ISaveableUniqueID { get; set; }

    // Data structure for saving the game object state
    public GameObjectSave GameObjectSave { get; set; }

    // Called after the scene has been loaded to assign parent transform for items
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = gameObject.AddComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    // Destroys all items currently in the scene
    private void DestroySceneItems()
    {
        // Use foreach for readability and maintainability
        foreach (var item in FindObjectsOfType<Item>())
        {
            Destroy(item.gameObject);
        }
    }

    // Instantiates a single item in the scene at a given position
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        itemGameObject.GetComponent<Item>().Init(itemCode);
    }

    // Instantiates multiple items based on a list of SceneItems
    public void InstantiateSceneItems(List<SceneItem> sceneItemsList)
    {
        foreach (var sceneItem in sceneItemsList)
        {
            Vector3 position = new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z);
            GameObject itemGameObject = Instantiate(itemPrefab, position, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    // Register to save system and subscribe to scene load event
    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    // Deregister from save system and unsubscribe from scene load event
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    // Save the current state of the scene items
    public void ISaveableStoreScene(string sceneName)
    {
        // Clear previous save data for this scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create a new list to store the current items in the scene
        List<SceneItem> sceneItemList = new();
        foreach (var item in FindObjectsOfType<Item>())
        {
            SceneItem sceneItem = new()
            {
                itemCode = item.ItemCode,
                position = new Vector3Serializable(item.transform.position),
                itemName = item.name
            };
            sceneItemList.Add(sceneItem);
        }

        // Create a new SceneSave instance and store the list of items
        SceneSave sceneSave = new()
        {
            listSceneItemDictionary = new Dictionary<string, List<SceneItem>>()
            {
                { "sceneItemList", sceneItemList }
            }
        };

        // Add scene save to GameObjectSave
        GameObjectSave.sceneData[sceneName] = sceneSave;
    }

    // Restore the saved state of the scene items
    public void ISaveableRestoreScene(string sceneName)
    {
        // Check if there is saved data for the given scene
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItemDictionary == null) return;
            if (sceneSave.listSceneItemDictionary.TryGetValue("sceneItemList", out List<SceneItem> sceneItemList))
            {
                // Destroy current items and instantiate saved ones
                DestroySceneItems();
                InstantiateSceneItems(sceneItemList);
            }
        }
    }
}
