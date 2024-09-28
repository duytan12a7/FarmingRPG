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
        if (gridPropertyDetails == null || !IsCursorValidForItem(itemDetails.itemType, gridPropertyDetails))
        {
            SetCursorToInvalid();
            return;
        }

        SetCursorToValid();
    }

    private bool IsCursorValidForItem(ItemType itemType, GridPropertyDetails gridPropertyDetails)
    {
        // Ch? ki?m tra m?t ?i?u ki?n cho c? Seed v? Commodity
        return itemType switch
        {
            ItemType.Seed or ItemType.Commodity => gridPropertyDetails.canDropItem,
            _ => true
        };
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
}
