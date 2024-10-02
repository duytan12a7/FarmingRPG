using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehaviour<PlayerController>
{

    private PlayerMovement playerMovement;
    public PlayerMovement PlayerMovement => playerMovement;

    private PlayerCarriedItemHandler playerCarriedItemHandler;
    public PlayerCarriedItemHandler PlayerCarriedItemHandler => playerCarriedItemHandler;

    private PlayerAnimation playerAnimation;
    public PlayerAnimation PlayerAnimation => playerAnimation;

    private GridCursor gridCursor;

    protected override void Awake() 
    {
        base.Awake();

        playerMovement = GetComponent<PlayerMovement>();
        playerCarriedItemHandler = GetComponent<PlayerCarriedItemHandler>();
        playerAnimation = GetComponent<PlayerAnimation>();
        
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
    }

    private void Update()
    {
        if (PlayerMovement.PlayerInputIsDisabled) return;

        PlayerClickInput();
    }

    private void PlayerClickInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (gridCursor.CursorIsEnabled)
            {
                ProcessPlayerClickInput();
            }
        }
    }

    private void ProcessPlayerClickInput()
    {
        PlayerMovement.ResetMovement();
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null) return;

        switch (itemDetails.itemType)
        {
            case ItemType.Seed:
                if (Input.GetMouseButtonDown(0)) ProcessPlayerClickInputSeed(itemDetails);
                break;
            case ItemType.Commodity:
                if (Input.GetMouseButtonDown(0)) ProcessPlayerClickInputCommodity(itemDetails);
                break;
            case ItemType.none:
                break;
            case ItemType.count:
                break;
            default:
                break;
        }
    }

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }
}
