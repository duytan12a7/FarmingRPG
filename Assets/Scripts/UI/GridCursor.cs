using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    [SerializeField] private SO_CropDetailsList sO_CropDetailsList;

    public bool CursorPositionIsValid { get; private set; } = false;
    public int ItemUseGridRadius { get; set; } = 0;
    public ItemType SelectedItemType { get; set; }
    public bool CursorIsEnabled { get; set; } = true;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoad;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid == null) return Vector3Int.zero;

        Vector3Int gridPosition = GetGridPositionForCursor();
        Vector3Int playerGridPosition = GetGridPositionForPlayer();

        SetCursorValidity(gridPosition, playerGridPosition);
        cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

        return gridPosition;
    }

    private void SceneLoad()
    {
        grid = FindObjectOfType<Grid>();
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        if (gridPropertyDetails == null)
        {
            SetCursorToInvalid();
            return;
        }
        switch (itemDetails.itemType)
        {
            case ItemType.Seed:
                if (!IsCursorValidForSeed(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.Commodity:
                if (!IsCursorValidForCommodity(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.Watering_tool:
            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Reaping_tool:
            case ItemType.Collecting_tool:
                if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.none:
                break;
            case ItemType.count:
                break;
            default:
                break;
        }

        SetCursorToValid();
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    Vector3 cursorWorldPosition = new(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    List<Item> items = new();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out items, cursorWorldPosition, Settings.cursorSize, 0f);

                    bool foundReapable = false;

                    foreach (Item item in items)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenery)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    return false;
                }

            case ItemType.Watering_tool:
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.Collecting_tool:
                if (gridPropertyDetails.seedItemCode == -1)
                    return false;

                CropDetails cropDetails = sO_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                if (cropDetails == null)
                    return false;

                return gridPropertyDetails.growthDays >= cropDetails.totalGrowthDays &&
                       cropDetails.CanUseToolHarvestCrop(itemDetails.itemCode);

            default:
                return false;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = Color.white;
        CursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        return grid.WorldToCell(worldPos);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(PlayerController.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);

        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
}
