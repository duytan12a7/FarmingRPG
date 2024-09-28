using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridProperties;
    [SerializeField] private SO_GridProperties[] gridPropertiesScriptableObjects = null;

    public string ISaveableUniqueID { get; set; }
    public GameObjectSave GameObjectSave { get; set; }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        IRegisterSaveable();
        EventHandler.AfterSceneLoadEvent += OnSceneLoaded;
    }

    private void OnDisable()
    {
        IDeregisterSaveable();
        EventHandler.AfterSceneLoadEvent -= OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        grid = FindObjectOfType<Grid>();
    }

    private void Start()
    {
        InitializeGridProperties();
    }

    public void InitializeGridProperties()
    {
        foreach (var so_GridProperties in gridPropertiesScriptableObjects)
        {
            var gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            foreach (var gridProperty in so_GridProperties.gridProperties)
            {
                var key = $"x{gridProperty.gridCoordinate.x}y{gridProperty.gridCoordinate.y}";

                var gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary)
                                          ?? new GridPropertyDetails();

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            var sceneSave = new SceneSave { gridPropertyDetailsDictionary = gridPropertyDictionary };

            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridProperties = gridPropertyDictionary;
            }

            GameObjectSave.sceneData[so_GridProperties.sceneName.ToString()] = sceneSave;
        }
    }

    public void IRegisterSaveable()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void IDeregisterSaveable()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void IRestoreSceneData(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out var sceneSave))
        {
            gridProperties = sceneSave.gridPropertyDetailsDictionary;
        }
    }

    public void IStoreSceneData(string sceneName)
    {
        GameObjectSave.sceneData[sceneName] = new SceneSave { gridPropertyDetailsDictionary = gridProperties };
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        var key = $"x{gridX}y{gridY}";
        gridPropertyDictionary.TryGetValue(key, out var gridPropertyDetails);
        return gridPropertyDetails;
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridProperties);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        var key = $"x{gridX}y{gridY}";
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridProperties);
    }
}
