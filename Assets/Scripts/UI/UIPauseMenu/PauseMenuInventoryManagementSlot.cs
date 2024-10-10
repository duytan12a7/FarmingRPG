using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image inventoryManagementSlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject greyedOutImageGO;

    [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    public GameObject draggedItem;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemQuantity == 0) return;
        // Instantiate gameobject as dragged item
        draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab, inventoryManagement.transform);
        // Get image for dragged item
        var draggedItemImage = draggedItem.GetComponentInChildren<Image>();
        draggedItemImage.sprite = inventoryManagementSlotImage.sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem == null) return;
        draggedItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem == null) return;
        Destroy(draggedItem);

        var destSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<PauseMenuInventoryManagementSlot>();
        if (destSlot == null) return;

        InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, destSlot.slotNumber);
        inventoryManagement.DestroyInventoryTextBoxGameObject();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity == 0)
            return;
        inventoryManagement.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
        inventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);
        var inventoryTextBox = inventoryManagement.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();
        string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);
        inventoryTextBox.SetTextboxText(
            itemDetails.itemDescription,
            itemTypeDescription,
            "",
            itemDetails.itemLongDescription,
            "",
            "");
        if (slotNumber > 23)
        {
            inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 50f,
                transform.position.z);
        }
        else
        {
            inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 50f,
                transform.position.z);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManagement.DestroyInventoryTextBoxGameObject();
    }
}
