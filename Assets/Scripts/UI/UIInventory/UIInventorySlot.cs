using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHightlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SceneLoaded;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Drops the item at the current mouse position if selected.
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

            // Create the item from prefab at mouse position
            GameObject itemGameObject = Instantiate(itemPrefab, worldPos, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            // Remove item from player's inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

            // Clear selected item if it was removed from the inventory
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            PlayerController.Instance.PlayerMovement.DisablePlayerInputAndResetMovement();

            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    // Destroy the dragged item placeholder
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            // Check if the drag ended over another inventory slot
            UIInventorySlot targetSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<UIInventorySlot>();
            if (targetSlot != null)
            {
                int toSlotNumber = targetSlot.slotNumber;

                // Swap inventory items
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();
                ClearSelectedItem();
            }
            else if (itemDetails.canBeDropped)
            {
                // Drop the item if it can be dropped
                DropSelectedItemAtMousePosition();
            }

            // Re-enable player input
            PlayerController.Instance.PlayerMovement.EnablePlayerInput();
        }
    }

    // Show item details when the pointer enters the slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            // Instantiate the inventory text box
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            // Populate the text box with item details
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // Set text box position based on inventory bar's position
            RectTransform textBoxTransform = inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>();
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                textBoxTransform.pivot = new Vector2(0.5f, 0f);
                textBoxTransform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                textBoxTransform.pivot = new Vector2(0.5f, 1f);
                textBoxTransform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    public void DestroyInventoryTextBox()
    {
        // Destroy the text box game object if it exists
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                ClearSelectedItem();
            }
            else if (itemQuantity > 0)
            {
                SetSelectedItem();
            }
        }
    }

    // Sets this inventory slot item to be selected.
    public void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        // Show carried item or clear it based on item's properties
        if (itemDetails.canBeCarried)
        {
            PlayerController.Instance.PlayerCarriedItemHandler.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            PlayerController.Instance.PlayerCarriedItemHandler.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        // Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = false;

        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
        PlayerController.Instance.PlayerCarriedItemHandler.ClearCarriedItem();
    }

    public void SceneLoaded()
    {
        // Find the parent item transform after the scene is loaded
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}
