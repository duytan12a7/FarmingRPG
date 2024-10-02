using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridProperties;
    [SerializeField] private SO_GridProperties[] gridPropertiesScriptableObjects = null;
    [SerializeField] private Tile[] dugGround = null;

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

        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
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

        if(gridProperties.Count > 0)
        {
            ClearDisplayGridPropertyDetails();
            DisplayGridPropertyDetails();
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

    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        Vector3Int[] directions = {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0)
    };

        foreach (var dir in directions)
        {
            GridPropertyDetails adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
            {
                Tile dugTile = SetDugTile(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y);
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y, 0), dugTile);
            }
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tile (up,down, left, right) are dug or not
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        #region Set appropiate tile based on whether surrounding tiles are dug or not
        // no surround
        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        // down & right
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        // down, right, left
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        // down left
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        // down
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        // up down right
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        // up down right left
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        // up down left
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        // up down
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[8];
        }
        // up right
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        // up right left
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        // up left
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        // up
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        // right
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        // right left
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        // left
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;
        #endregion Set appropiate tile based on whether surrounding tiles are dug or not
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridProperties)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
        }
    }
}
