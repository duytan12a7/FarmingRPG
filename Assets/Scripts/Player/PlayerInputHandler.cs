using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController playerCtrl;

    private GridCursor gridCursor;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();

        gridCursor = FindObjectOfType<GridCursor>();
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

        if (gridCursor.CursorIsEnabled)
        {
            ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
        }
    }

    //private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    //{
    //    playerCtrl.PlayerMovement.ResetMovement();

    //    Vector3Int playerDirection = playerCtrl.PlayerAnimation.GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
    //    GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
    //    ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

    //    if (itemDetails == null) return;

    //    if (Input.GetMouseButtonDown(0) && gridCursor.CursorPositionIsValid)
    //    {
    //        switch (itemDetails.itemType)
    //        {
    //            case ItemType.Seed:
    //            case ItemType.Commodity:
    //                if (itemDetails.canBeDropped)
    //                    EventHandler.CallDropSelectedItemEvent();
    //                break;

    //            case ItemType.Hoeing_tool:
    //                ProcessPlayerClickInputTool(gridPropertyDetails, playerDirection);
    //                break;
    //        }
    //    }
    //}

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        playerCtrl.PlayerMovement.ResetMovement();

        Vector3Int playerDirection = playerCtrl.PlayerAnimation.GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null) return;

        if (Input.GetMouseButtonDown(0) && gridCursor.CursorPositionIsValid)
        {
            HandleItemInteraction(itemDetails, gridPropertyDetails, playerDirection);
        }
    }

    private void HandleItemInteraction(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Seed:
            case ItemType.Commodity:
                if (itemDetails.canBeDropped)
                {
                    EventHandler.CallDropSelectedItemEvent();
                }
                break;

            case ItemType.Hoeing_tool:
                ProcessPlayerClickInputTool(gridPropertyDetails, playerDirection);
                break;
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        if (gridCursor.CursorPositionIsValid)
        {
            playerCtrl.PlayerAnimation.HoeGroundAtCursor(gridPropertyDetails, playerDirection);
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
