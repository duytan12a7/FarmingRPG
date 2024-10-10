using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlots = null;

    public GameObject inventoryManagementDraggedItemPrefab = null;

    [SerializeField] private Sprite transparent16x16 = null;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;
        if (InventoryManager.Instance == null)
            return;
        PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);
    }
    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameObject();
    }
    public void DestroyInventoryTextBoxGameObject()
    {
        Destroy(inventoryTextBoxGameObject);
    }
    public void DestroyCurrentlyDraggedItems()
    {
        foreach (var slot in inventoryManagementSlots)
        {
            Destroy(slot.draggedItem);
        }
    }
    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            InitializeInventoryManagementSlot();
            for (int i = 0; i < playerInventoryList.Count; i++)
            {
                var inventoryItem = playerInventoryList[i];

                // Get inventory item details
                inventoryManagementSlots[i].itemDetails = InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode);
                inventoryManagementSlots[i].itemQuantity = inventoryItem.itemQuantity;
                if (inventoryManagementSlots[i].itemDetails != null)
                {
                    // update inventory management slot with image and quality
                    inventoryManagementSlots[i].inventoryManagementSlotImage.sprite = inventoryManagementSlots[i].itemDetails.itemSprite;
                    inventoryManagementSlots[i].textMeshProUGUI.text = inventoryManagementSlots[i].itemQuantity.ToString();
                }
            }
        }
    }
    private void InitializeInventoryManagementSlot()
    {
        int currentMaxCapacity = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player];
        for (int x = 0; x < Settings.playerMaximumInventoryCapacity; x++)
        {
            if (x < currentMaxCapacity)
            {
                inventoryManagementSlots[x].itemDetails = null;
                inventoryManagementSlots[x].itemQuantity = 0;
                inventoryManagementSlots[x].inventoryManagementSlotImage.sprite = transparent16x16;
                inventoryManagementSlots[x].textMeshProUGUI.text = "";
            }
            inventoryManagementSlots[x].greyedOutImageGO.SetActive(x >= currentMaxCapacity);
        }
    }
}
