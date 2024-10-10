using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;


    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;

    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void LateUpdate()
    {
        // Switch inventory bar position depending on player position
        SwitchInventoryBarPosition();
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation != InventoryLocation.player || inventorySlots.Length == 0) return;

        ClearInventorySlots();

        int itemCount = Mathf.Min(inventorySlots.Length, inventoryList.Count);
        for (int i = 0; i < itemCount; i++)
        {
            UpdateSlot(inventorySlots[i], inventoryList[i]);
        }
    }

    private void UpdateSlot(UIInventorySlot uIInventorySlot, InventoryItem inventoryItem)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode);

        if (itemDetails == null) return;

        uIInventorySlot.inventorySlotImage.sprite = itemDetails.itemSprite;
        uIInventorySlot.textMeshProUGUI.text = inventoryItem.itemQuantity.ToString();
        uIInventorySlot.itemDetails = itemDetails;
        uIInventorySlot.itemQuantity = inventoryItem.itemQuantity;

        SetHighlightedInventorySlots(uIInventorySlot);
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = PlayerController.Instance.PlayerMovement.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && !IsInventoryBarPositionBottom)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;

        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;

        }
    }

    private void ClearInventorySlots()
    {
        foreach (UIInventorySlot uIInventorySlot in inventorySlots)
        {
            uIInventorySlot.inventorySlotImage.sprite = blank16x16sprite;
            uIInventorySlot.textMeshProUGUI.text = string.Empty;
            uIInventorySlot.itemDetails = null;
            uIInventorySlot.itemQuantity = 0;

            SetHighlightedInventorySlots(uIInventorySlot);
        }
    }

    public void ClearHighlightOnInventorySlots()
    {
        foreach (UIInventorySlot uIInventorySlot in inventorySlots)
        {
            if (uIInventorySlot.isSelected)
            {
                uIInventorySlot.isSelected = false;
                uIInventorySlot.inventorySlotHightlight.color = new Color(0f, 0f, 0f, 0f);
                InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
            }
        }
    }

    public void SetHighlightedInventorySlots()
    {
        foreach (UIInventorySlot uIInventorySlot in inventorySlots)
        {
            SetHighlightedInventorySlots(uIInventorySlot);
        }
    }

    public void SetHighlightedInventorySlots(UIInventorySlot uIInventorySlot)
    {
        if (uIInventorySlot?.itemDetails != null && uIInventorySlot.isSelected)
        {
            uIInventorySlot.inventorySlotHightlight.color = new Color(1f, 1f, 1f, 1f);
            InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, uIInventorySlot.itemDetails.itemCode);
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        foreach (UIInventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.draggedItem != null)
            {
                Destroy(inventorySlot.draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        foreach (UIInventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.draggedItem != null)
            {
                inventorySlot.ClearSelectedItem();
            }
        }
    }
}
