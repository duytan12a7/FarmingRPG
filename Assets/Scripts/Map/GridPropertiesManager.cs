using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridProperties;
    [SerializeField] private SO_GridProperties[] gridPropertiesScriptableObjects = null;
    [SerializeField] private SO_CropDetailsList sO_CropDetailsList = null;

    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] waterGround = null;

    public string ISaveableUniqueID { get; set; }
    public GameObjectSave GameObjectSave { get; set; }

    private bool isFirstTimeSceneLoaded = true;

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        IRegisterSaveable();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        IDeregisterSaveable();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void AfterSceneLoaded()
    {
        GameObject cropParent = GameObject.FindGameObjectWithTag(Global.Tags.CropsParentTransform);
        cropParentTransform = cropParent != null ? cropParent.transform : null;

        grid = FindObjectOfType<Grid>();

        groundDecoration1 = GameObject.FindGameObjectWithTag(Global.Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Global.Tags.GroundDecoration2).GetComponent<Tilemap>();
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

                GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary)
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

            SceneSave sceneSave = new() { gridPropertyDetailsDictionary = gridPropertyDictionary };

            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                gridProperties = gridPropertyDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool> { { nameof(isFirstTimeSceneLoaded), true } };

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
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            gridProperties = sceneSave.gridPropertyDetailsDictionary;
        }

        if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue(nameof(isFirstTimeSceneLoaded), out bool storedIsFirstTimeSceneLoaded))
        {
            isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
        }

        if (isFirstTimeSceneLoaded)
            EventHandler.CallInstantiateCropPrefabEvent();

        if (gridProperties.Count > 0)
        {
            ClearDisplayGridPropertyDetails();
            DisplayGridPropertyDetails();
        }

        if (isFirstTimeSceneLoaded) isFirstTimeSceneLoaded = false;
    }

    public void IStoreSceneData(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        var sceneSave = new SceneSave(gridProperties)
        {
            boolDictionary = new Dictionary<string, bool> { { nameof(isFirstTimeSceneLoaded), isFirstTimeSceneLoaded } }
        };

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        if (gridPropertyDictionary.TryGetValue(GridPropertyDetails.Key(gridX, gridY), out var gridPropertyDetails))
        {
            return gridPropertyDetails;
        }
        return null;
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridProperties);
    }

    public GridPropertyDetails GetGridPropertyDetails(Vector2Int pos)
    {
        return GetGridPropertyDetails(pos.x, pos.y);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDictionary[gridPropertyDetails.Key()] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridProperties);
    }

    public void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Clear Display all Grid Property Details
        ClearDisplayGridPropertyDetails();

        // Loop through all scenes - by looping through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in gridPropertiesScriptableObjects)
        {
            // get gridPropertdetail dictionary scene

            if (!GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out var sceneSave))
                continue;

            if (sceneSave?.gridPropertyDetailsDictionary == null)
                continue;

            for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
            {
                KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                GridPropertyDetails gridPropertyDetails = item.Value;

                #region Update all grid properties to reflect the advance in day

                if (gridPropertyDetails.growthDays >= 0)
                    gridPropertyDetails.growthDays++;
                // if ground is watered, then clear water
                if (gridPropertyDetails.daysSinceWatered > -1)
                {
                    gridPropertyDetails.daysSinceWatered = -1;
                }

                // set gridproperty details
                SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                #endregion Update all grid properties to reflect the advance in day
            }
        }

        // Display grid property details to relfect changed value
        DisplayGridPropertyDetails();

    }


    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();
        ClearDisplayAllPantedCrops();
    }

    private void ClearDisplayAllPantedCrops()
    {
        Crop[] crops;
        crops = FindObjectsOfType<Crop>();

        foreach (Crop crop in crops)
        {
            Destroy(crop.gameObject);
        }
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectGround(gridPropertyDetails, SetDugTile, groundDecoration1);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectGround(gridPropertyDetails, SetWateredTile, groundDecoration2);
        }
    }

    private void ConnectGround(GridPropertyDetails gridPropertyDetails, System.Func<int, int, Tile> setTileFunc, Tilemap groundDecoration)
    {
        Tile tile0 = setTileFunc(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), tile0);

        Vector3Int[] directions = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.right,
        Vector3Int.left
    };

        foreach (var dir in directions)
        {
            GridPropertyDetails adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y);

            if (adjacentGridPropertyDetails != null && (setTileFunc == SetDugTile ? adjacentGridPropertyDetails.daysSinceDug > -1 : adjacentGridPropertyDetails.daysSinceWatered > -1))
            {
                Tile tile = setTileFunc(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y);
                groundDecoration.SetTile(new Vector3Int(gridPropertyDetails.gridX + dir.x, gridPropertyDetails.gridY + dir.y, 0), tile);
            }
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        return SetTile(xGrid, yGrid, true);
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        return SetTile(xGrid, yGrid, false);
    }

    private Tile SetTile(int xGrid, int yGrid, bool isDug)
    {
        bool up = isDug ? IsGridSquareDug(xGrid, yGrid + 1) : IsGridSquareWatered(xGrid, yGrid + 1);
        bool down = isDug ? IsGridSquareDug(xGrid, yGrid - 1) : IsGridSquareWatered(xGrid, yGrid - 1);
        bool right = isDug ? IsGridSquareDug(xGrid + 1, yGrid) : IsGridSquareWatered(xGrid + 1, yGrid);
        bool left = isDug ? IsGridSquareDug(xGrid - 1, yGrid) : IsGridSquareWatered(xGrid - 1, yGrid);

        Tile[] tileArray = isDug ? dugGround : waterGround;

        if (!up && !down && !right && !left) return tileArray[0];
        if (!up && down && right && !left) return tileArray[1];
        if (!up && down && right && left) return tileArray[2];
        if (!up && down && !right && left) return tileArray[3];
        if (!up && down && !right && !left) return tileArray[4];
        if (up && down && right && !left) return tileArray[5];
        if (up && down && right && left) return tileArray[6];
        if (up && down && !right && left) return tileArray[7];
        if (up && down && right && !left) return tileArray[8];
        if (up && !down && right && !left) return tileArray[9];
        if (up && !down && right && left) return tileArray[10];
        if (up && !down && !right && left) return tileArray[11];
        if (up && !down && !right && !left) return tileArray[12];
        if (!up && !down && right && !left) return tileArray[13];
        if (!up && !down && right && left) return tileArray[14];
        if (!up && !down && !right && left) return tileArray[15];

        return null;
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        return IsGridSquareCondition(xGrid, yGrid, details => details.daysSinceDug);
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        return IsGridSquareCondition(xGrid, yGrid, details => details.daysSinceWatered);
    }

    private bool IsGridSquareCondition(int xGrid, int yGrid, Func<GridPropertyDetails, int> condition)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }
        return condition(gridPropertyDetails) > -1;
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridProperties)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        CropDetails cropDetails = sO_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
        if (cropDetails == null) return;

        // Find the current growsth stage
        int currentStage = cropDetails.GetGrowthStageForDays(gridPropertyDetails.growthDays);

        Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

        Crop.Create(
            cropDetails.growthPrefab[currentStage],
            worldPosition,
            cropDetails.growthSprites[currentStage],
            cropParentTransform,
            gridPropertyDetails.gridX,
            gridPropertyDetails.gridY
        );
    }

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;

        for (int i = 0; i < collider2Ds.Length; i++)
        {
            crop = collider2Ds[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            crop = collider2Ds[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode) => sO_CropDetailsList.GetCropDetails(seedItemCode);
}
