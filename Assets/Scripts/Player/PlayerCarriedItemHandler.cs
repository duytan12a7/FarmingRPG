using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarriedItemHandler : MonoBehaviour
{
    private bool isCarrying = false;

    [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private AnimationOverrides animationOverrides;

    private List<CharacterAttribute> characterAttributes;
    private CharacterAttribute armsCharacterAttribute;

    private void Awake()
    {
        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        characterAttributes = new List<CharacterAttribute>();
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
        equippedItemSpriteRenderer.sprite = itemSprite;
        equippedItemSpriteRenderer.color = itemSprite != null ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);

        armsCharacterAttribute.variantType = variantType;

        characterAttributes.Clear();
        characterAttributes.Add(armsCharacterAttribute);

        animationOverrides.ApplyCharacterCustomisation(characterAttributes);
    }

    public bool IsCarrying()
    {
        return isCarrying;
    }
}
