using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    public bool CursorIsEnabled { get; set; } = true;

    public bool CursorPositionIsValid { get; set; } = false;

    public ItemType SelectedItemType { get; set; }

    public float ItemUseRadius { get; set; } = 0f;

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

    // Display the cursor based on its current state
    private void DisplayCursor()
    {
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        // Check if the cursor position is valid based on player and item conditions
        SetCursorValidity(cursorWorldPosition, PlayerController.Instance.GetPlayerCentrePosition());

        // Update the cursor's position in the UI
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    // Validate cursor's position based on player position and item usage
    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        // Check if the cursor is out of bounds or if the selected item is invalid
        if (IsCursorOutOfUseRadius(cursorPosition, playerPosition) || !IsSelectedItemValid(cursorPosition))
        {
            SetCursorState(false); // Set cursor to invalid
        }
        else
        {
            SetCursorState(true); // Set cursor to valid
        }
    }

    // Check if the cursor is outside the valid item use radius
    private bool IsCursorOutOfUseRadius(Vector3 cursorPosition, Vector3 playerPosition)
    {
        float deltaX = Mathf.Abs(cursorPosition.x - playerPosition.x);
        float deltaY = Mathf.Abs(cursorPosition.y - playerPosition.y);

        // Return true if the cursor exceeds the allowed radius
        return (deltaX > ItemUseRadius || deltaY > ItemUseRadius);
    }

    // Check if the selected item is valid at the current cursor position
    private bool IsSelectedItemValid(Vector3 cursorPosition)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if (itemDetails == null) return false; // If no item is selected, return false

        // Check validity based on the item type (currently only reaping tools)
        return itemDetails.itemType switch
        {
            ItemType.Reaping_tool => IsReapingToolValid(cursorPosition),
            _ => false,
        };
    }

    // Check if the reaping tool is valid at the cursor position
    private bool IsReapingToolValid(Vector3 cursorPosition)
    {
        List<Item> itemList = new();

        // Get components at the cursor location and check if they are valid reaping targets
        if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            foreach (Item item in itemList)
            {
                if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenery)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Set the cursor to either valid (green) or invalid (transparent) state
    private void SetCursorState(bool isValid)
    {
        cursorImage.sprite = isValid ? greenCursorSprite : transparentCursorSprite; // Change cursor sprite based on validity
        CursorPositionIsValid = isValid; // Update cursor position validity

        // Enable or disable the grid cursor based on the cursor's validity
        if (isValid)
        {
            gridCursor.DisableCursor();
        }
        else
        {
            gridCursor.EnableCursor();
        }
    }

    // Enable the cursor by making it visible
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f); // Set the cursor's color to full opacity
        CursorIsEnabled = true;
    }

    // Disable the cursor by making it invisible
    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f); // Set the cursor's color to transparent
        CursorIsEnabled = false;
    }

    // Get the cursor's position in the game world from the screen position
    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new(Input.mousePosition.x, Input.mousePosition.y, 0f);
        return mainCamera.ScreenToWorldPoint(screenPosition); // Convert screen position to world position
    }

    // Get the cursor's position in the UI using RectTransform
    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas); // Adjust for UI scaling
    }
}
