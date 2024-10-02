using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerCtrl;

    private bool isCarrying = false;

    private bool isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown;
    private bool isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown;
    private bool isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown;
    private bool isPickingRight, isPickingLeft, isPickingUp, isPickingDown;

    private ToolEffect toolEffect = ToolEffect.none;

    [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributes;
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;

    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;

    public bool PlayerToolUseDisabled { get; set; } = false;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        characterAttributes = new List<CharacterAttribute>();
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
    }

    private void Start()
    {
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);

        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
    }

    public void ResetAnimationTriggers()
    {
        isPickingRight = isPickingLeft = isPickingUp = isPickingDown = false;
        isUsingToolRight = isUsingToolLeft = isUsingToolUp = isUsingToolDown = false;
        isLiftingToolRight = isLiftingToolLeft = isLiftingToolUp = isLiftingToolDown = false;
        isSwingingToolRight = isSwingingToolLeft = isSwingingToolUp = isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }

    public void CallMovementEvent()
    {
        EventHandler.CallMovementEvent(playerCtrl.PlayerMovement.XInput(), playerCtrl.PlayerMovement.YInput(),
            playerCtrl.PlayerMovement.IsWalking(), playerCtrl.PlayerMovement.IsRunning(), playerCtrl.PlayerMovement.IsIdle(),
            isCarrying, toolEffect, isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
            false, false, false, false);
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

        if (itemDetails == null) return;

        UpdateEquippedItem(itemDetails.itemSprite, PartVariantType.carry);
        isCarrying = true;
    }

    public void ClearCarriedItem()
    {
        UpdateEquippedItem(null, PartVariantType.none);
        isCarrying = false;
    }

    private void UpdateEquippedItem(Sprite itemSprite, PartVariantType variantType)
    {
        if (equippedItemSpriteRenderer != null)
        {
            equippedItemSpriteRenderer.sprite = itemSprite;
            equippedItemSpriteRenderer.color = itemSprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
        }

        armsCharacterAttribute.variantType = variantType;
        characterAttributes.Clear();
        characterAttributes.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisation(characterAttributes);
    }

    public Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x) return Vector3Int.right;
        if (cursorGridPosition.x < playerGridPosition.x) return Vector3Int.left;
        if (cursorGridPosition.y > playerGridPosition.y) return Vector3Int.up;
        return Vector3Int.down;
    }

    public void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(PerformToolAction(gridPropertyDetails, playerDirection, PartVariantType.hoe, ToolAction.usingTool, ToolEffect.none));
    }

    public void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(PerformToolAction(gridPropertyDetails, playerDirection, PartVariantType.wateringCan, ToolAction.liftingTool, ToolEffect.watering));
    }

    private (WaitForSeconds toolPause, WaitForSeconds afterToolPause) GetToolPauseTimes(ToolAction toolAction)
    {
        return toolAction switch
        {
            ToolAction.usingTool => (useToolAnimationPause, afterUseToolAnimationPause),
            ToolAction.liftingTool => (liftToolAnimationPause, afterLiftToolAnimationPause),
            _ => (new WaitForSeconds(0), new WaitForSeconds(0)),
        };
    }

    private IEnumerator PerformToolAction(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection, PartVariantType toolType, ToolAction toolAction, ToolEffect effect)
    {
        playerCtrl.PlayerMovement.PlayerInputIsDisabled = true;
        PlayerToolUseDisabled = true;
        var (toolPause, afterToolPause) = GetToolPauseTimes(toolAction);

        toolCharacterAttribute.variantType = toolType;
        characterAttributes.Clear();
        characterAttributes.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisation(characterAttributes);

        toolEffect = effect;
        SetToolFlags(playerDirection, toolAction);

        yield return toolPause;

        if (toolType == PartVariantType.hoe)
        {
            gridPropertyDetails.daysSinceDug = gridPropertyDetails.daysSinceDug == -1 ? 0 : gridPropertyDetails.daysSinceDug;
            GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);
        }
        else if (toolType == PartVariantType.wateringCan)
        {
            if (gridPropertyDetails.daysSinceWatered == -1)
            {
                gridPropertyDetails.daysSinceWatered = 0;
            }
            GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        yield return afterToolPause;

        playerCtrl.PlayerMovement.PlayerInputIsDisabled = false;
        PlayerToolUseDisabled = false;
    }


    public void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        playerCtrl.PlayerMovement.PlayerInputIsDisabled = true;
        PlayerToolUseDisabled = true;

        toolCharacterAttribute.variantType = PartVariantType.scythe;
        characterAttributes.Clear();
        characterAttributes.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisation(characterAttributes);

        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        playerCtrl.PlayerMovement.PlayerInputIsDisabled = false;
        PlayerToolUseDisabled = false;

    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (!Input.GetMouseButton(0)) return;

        SetToolFlags(playerDirection, ToolAction.reapingTool);

        Vector2 point = new(
            playerCtrl.GetPlayerCentrePosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)),
            playerCtrl.GetPlayerCentrePosition().y + (playerDirection.y * (equippedItemDetails.itemUseRadius / 2f)));

        Vector2 size = new(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

        // Get Item componenets with 2D collider located in teh house at the center point defined ( 2d colliders tested limited to maxCollidersTo Test Perreap swing)
        Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestReapSwing, point, size, 0f);

        int reapableItemCount = 0;

        // Loop through all items retrieved 
        for (int i = itemArray.Length - 1; i >= 0; i--)
        {
            if (itemArray[i] != null)
            {
                // Destroy item game object if reapable
                if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenery)
                {
                    Vector3 effectPosition = new(
                        itemArray[i].transform.position.x,
                        itemArray[i].transform.position.y + Settings.gridCellSize / 2f,
                        itemArray[i].transform.position.z);

                    itemArray[i].gameObject.SetActive(false);
                    //Destroy(itemArray[i].gameObject);

                    reapableItemCount++;
                    if (reapableItemCount >= Settings.maxTargetComponentsToDestroyReapSwing)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void SetToolFlags(Vector3Int playerDirection, ToolAction toolAction)
    {
        bool isRight = playerDirection == Vector3Int.right;
        bool isLeft = playerDirection == Vector3Int.left;
        bool isUp = playerDirection == Vector3Int.up;
        bool isDown = playerDirection == Vector3Int.down;

        if (toolAction == ToolAction.usingTool)
        {
            isUsingToolRight = isRight;
            isUsingToolLeft = isLeft;
            isUsingToolUp = isUp;
            isUsingToolDown = isDown;
        }
        else if (toolAction == ToolAction.reapingTool)
        {
            isSwingingToolRight = isRight;
            isSwingingToolLeft = isLeft;
            isSwingingToolUp = isUp;
            isSwingingToolDown = isDown;
        }
        else if (toolAction == ToolAction.liftingTool)
        {
            isLiftingToolRight = isRight;
            isLiftingToolLeft = isLeft;
            isLiftingToolUp = isUp;
            isLiftingToolDown = isDown;
        }
        else if (toolAction == ToolAction.pickingTool)
        {
            isPickingRight = isRight;
            isPickingLeft = isLeft;
            isPickingUp = isUp;
            isPickingDown = isDown;
        }
    }
}
