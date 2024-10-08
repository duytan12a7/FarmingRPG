using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    private int[] selectedInventoryItem;
    public List<InventoryItem>[] inventoryLists;
    [HideInInspector] public int[] inventoryListCapacityIntArray;
    [SerializeField] private SO_ItemList itemListSO = null;

    protected override void Awake()
    {
        base.Awake();
        CreateInventoryLists();
        CreateItemsDetailsDictionary();
        InitializeSelectedInventoryItems();
    }

    private void CreateInventoryLists()
    {
        int inventoryCount = (int)InventoryLocation.count;
        inventoryLists = new List<InventoryItem>[inventoryCount];
        inventoryListCapacityIntArray = new int[inventoryCount];

        for (int i = 0; i < inventoryCount; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    private void InitializeSelectedInventoryItems()
    {
        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }
    }

    private void CreateItemsDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails itemDetails in itemListSO.itemDetails)
        {
            itemDetailsDictionary[itemDetails.itemCode] = itemDetails;
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);
        Destroy(gameObjectToDelete);
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item) => AddItem(inventoryLocation, item.ItemCode);

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            UpdateItemQuantity(inventoryItems, itemCode, itemPosition, 1);
        }
        else
        {
            inventoryItems.Add(new InventoryItem { itemCode = itemCode, itemQuantity = 1 });
        }
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryItems);
    }

    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            UpdateItemQuantity(inventoryList, itemCode, itemPosition, -1);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryList);
    }

    private void UpdateItemQuantity(List<InventoryItem> inventoryList, int itemCode, int position, int quantityChange)
    {
        InventoryItem inventoryItem = inventoryList[position];
        inventoryItem.itemQuantity += quantityChange;

        if (inventoryItem.itemQuantity > 0)
        {
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        itemDetailsDictionary.TryGetValue(itemCode, out ItemDetails itemDetails);
        return itemDetails;
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1) return null;

        return GetItemDetails(itemCode);
    }

    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        if (IsValidSwap(inventoryLocation, fromItem, toItem))
        {
            (inventoryList[fromItem], inventoryList[toItem]) = (inventoryList[toItem], inventoryList[fromItem]);
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }

    private bool IsValidSwap(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        return fromItem >= 0 && toItem >= 0 &&
               fromItem < inventoryLists[(int)inventoryLocation].Count &&
               toItem < inventoryLists[(int)inventoryLocation].Count &&
               fromItem != toItem;
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Breaking_tool => Settings.BreakingTool,
            ItemType.Chopping_tool => Settings.ChoppingTool,
            ItemType.Hoeing_tool => Settings.HoeingTool,
            ItemType.Reaping_tool => Settings.ReapingTool,
            ItemType.Watering_tool => Settings.WateringTool,
            ItemType.Collecting_tool => Settings.CollectingTool,
            _ => itemType.ToString(),
        };
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }
}
