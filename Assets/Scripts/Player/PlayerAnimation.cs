using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerCtrl;

    protected bool isCarrying = false;

    protected bool isLiftingToolRight;
    protected bool isLiftingToolLeft;
    protected bool isLiftingToolUp;
    protected bool isLiftingToolDown;
    protected bool isUsingToolRight;
    protected bool isUsingToolLeft;
    protected bool isUsingToolUp;
    protected bool isUsingToolDown;
    protected bool isSwingingToolRight;
    protected bool isSwingingToolLeft;
    protected bool isSwingingToolUp;
    protected bool isSwingingToolDown;
    protected bool isPickingRight;
    protected bool isPickingLeft;
    protected bool isPickingUp;
    protected bool isPickingDown;

    protected ToolEffect toolEffect = ToolEffect.none;

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
        if (itemDetails != null)
        {
            UpdateEquippedItem(itemDetails.itemSprite, PartVariantType.carry);
            isCarrying = true;
        }
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
        StartCoroutine(HoeGroundAtCursorRoutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator HoeGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        playerCtrl.PlayerMovement.PlayerInputIsDisabled = true;
        PlayerToolUseDisabled = true;

        toolCharacterAttribute.variantType = PartVariantType.hoe;
        characterAttributes.Clear();
        characterAttributes.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisation(characterAttributes);

        SetToolUsingFlags(playerDirection);

        yield return useToolAnimationPause;

        gridPropertyDetails.daysSinceDug = gridPropertyDetails.daysSinceDug == -1 ? 0 : gridPropertyDetails.daysSinceDug;
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        playerCtrl.PlayerMovement.PlayerInputIsDisabled = false;
        PlayerToolUseDisabled = false;
    }

    public void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator WaterGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        playerCtrl.PlayerMovement.PlayerInputIsDisabled = true;
        PlayerToolUseDisabled = true;

        // Set tool animation to watering can in override aniamtion
        toolCharacterAttribute.variantType = PartVariantType.wateringCan;
        characterAttributes.Clear();
        characterAttributes.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisation(characterAttributes);

        // TODO if there is water in teh watering can
        toolEffect = ToolEffect.watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        // Set Grid Property details for watering ground
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        // Set Grid property to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // After animation pause
        yield return afterLiftToolAnimationPause;

        // Allow player input
        playerCtrl.PlayerMovement.PlayerInputIsDisabled = false;
        PlayerToolUseDisabled = false;


    }

    private void SetToolUsingFlags(Vector3Int playerDirection)
    {
        isUsingToolRight = playerDirection == Vector3Int.right;
        isUsingToolLeft = playerDirection == Vector3Int.left;
        isUsingToolUp = playerDirection == Vector3Int.up;
        isUsingToolDown = playerDirection == Vector3Int.down;
    }

    private void SetToolLiftingFlags(Vector3Int playerDirection)
    {
        isLiftingToolRight = playerDirection == Vector3Int.right;
        isLiftingToolLeft = playerDirection == Vector3Int.left;
        isLiftingToolUp = playerDirection == Vector3Int.up;
        isLiftingToolDown = playerDirection == Vector3Int.down;
    }
}
