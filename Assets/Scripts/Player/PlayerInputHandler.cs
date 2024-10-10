using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController playerCtrl;

    private GridCursor gridCursor;

    private Cursor cursor;

    public Direction playerDirection;

    private void Start()
    {
        playerCtrl = GetComponent<PlayerController>();

        gridCursor = FindObjectOfType<GridCursor>();

        cursor = FindObjectOfType<Cursor>();
    }

    private void Update()
    {
        if (!playerCtrl.PlayerMovement.PlayerInputIsDisabled)
        {
            HandlePlayerInput();
            HandlePlayerAnimations();
            HandleDebugInput();
        }
    }

    private void HandlePlayerInput()
    {
        float yInput = Input.GetAxisRaw("Vertical");
        float xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput *= Settings.DiagonalMovementFactor;
            yInput *= Settings.DiagonalMovementFactor;
        }

        playerCtrl.PlayerMovement.UpdatePlayerState(xInput, yInput);
        playerCtrl.PlayerMovement.UpdatePlayerDirection(xInput, yInput);
    }

    private void HandlePlayerAnimations()
    {
        playerCtrl.PlayerAnimation.ResetAnimationTriggers();

        HandlePlayerClickInput();

        playerCtrl.PlayerAnimation.CallMovementEvent();
    }

    public void HandlePlayerClickInput()
    {
        if (playerCtrl.PlayerAnimation.PlayerToolUseDisabled || !Input.GetMouseButton(0)) return;

        Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
        Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

        if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
        {
            ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        playerCtrl.PlayerMovement.ResetMovement();

        Vector3Int playerDirection = playerCtrl.PlayerAnimation.GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleItemInteraction(itemDetails, gridPropertyDetails, playerDirection);
        }
    }

    private void HandleItemInteraction(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Seed:
                ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                break;

            case ItemType.Commodity:
                ProcessPlayerClickInputCommodity(itemDetails);
                break;

            case ItemType.Hoeing_tool:
            case ItemType.Watering_tool:
            case ItemType.Reaping_tool:
            case ItemType.Collecting_tool:
            case ItemType.Chopping_tool:
                ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                break;
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                    playerCtrl.PlayerAnimation.HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                break;
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                    playerCtrl.PlayerAnimation.WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                break;
            case ItemType.Chopping_tool:
                if (gridCursor.CursorPositionIsValid)
                    playerCtrl.PlayerAnimation.ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                break;
            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                    playerCtrl.PlayerAnimation.CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                break;
            case ItemType.Reaping_tool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), playerCtrl.GetPlayerCentrePosition());
                    playerCtrl.PlayerAnimation.ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        gridPropertyDetails.seedItemCode = itemDetails.itemCode;
        gridPropertyDetails.growthDays = 0;

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

        EventHandler.CallRemoveSelectedItemFromInventoryEvent();
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)

        )
        {
            return Vector3Int.right;
        }
        else if (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
        )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void HandleDebugInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameTime();
        }
        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
        if (Input.GetKey(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }
}
