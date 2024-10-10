using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crop : MonoBehaviour
{
    private int _harvestActionCount = 0;

    [Tooltip("This should be populated from the child transform gameobject showing harvest effect spawn point")]
    [SerializeField] private Transform _harvestActionEffectTransform = null;

    [Tooltip("This should be populated from the child gameobject")]
    [SerializeField] private SpriteRenderer _cropHarvestedSpriteRenderer = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;

    public static GameObject Create(
        GameObject prefab,
        Vector3 position,
        Sprite sprite,
        Transform parentTransform,
        int gridPosX,
        int gridPosY)
    {
        var cropInstance = Instantiate(prefab, position, Quaternion.identity);
        cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        cropInstance.transform.SetParent(parentTransform);
        cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPosX, gridPosY);
        return cropInstance;
    }

    public void ProcessToolAction(ItemDetails itemDetails, bool isToolRight, bool isToolLeft, bool isToolUp, bool isToolDown)
    {
        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(this.cropGridPosition);
        if (gridPropertyDetails == null)
            return;

        var seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
            return;

        var cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
            return;

        // Get animator for crop, if present
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger(Global.Animations.Triggers.UseToolRight);
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger(Global.Animations.Triggers.UseToolLeft);
            }
        }

        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(_harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }

        int requiredHarvestActions = cropDetails.RequiredHarvestActionForTool(itemDetails.itemCode);
        if (requiredHarvestActions < 0)
            return;

        _harvestActionCount++;

        if (_harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(gridPropertyDetails, cropDetails, animator, isToolRight || isToolUp);
        }
    }

    private void HarvestCrop(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails, Animator animator, bool usingToolRight)
    {
        // Handle animation if exists
        bool runningAnimation = false;

        if (cropDetails.isHarvestedAnimation && (animator != null))
        {
            if (cropDetails.harvestedSprite != null)
            {
                _cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
            }

            animator.SetTrigger(usingToolRight ? Global.Animations.Triggers.HarvestRight : Global.Animations.Triggers.HarvestLeft);
            runningAnimation = true;
        }

        // Delete crop from grid properties
        gridPropertyDetails.ClearCropData();

        // Should the crop be hidden before the harvest animation
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        StartCoroutine(ProcessHarvestActionsAfterAnimation(gridPropertyDetails, cropDetails, runningAnimation, animator));
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails, bool runningAnimation, Animator animator)
    {
        if (runningAnimation)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
            {
                yield return null;
            }
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if (cropDetails.harvestedTransfromItemCode > 0)
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);

        Destroy(gameObject);
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransfromItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            var cropProducedItem = cropDetails.cropProducedItemCode[i];
            int minAmount = cropDetails.cropProducedMinQuantity[i];
            int maxAmount = Math.Max(minAmount, cropDetails.cropProducedMaxQuantity[i]);
            int amount = Random.Range(minAmount, maxAmount + 1);

            for (int x = 0; x < amount; x++)
            {
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropProducedItem);
                }
                else
                {
                    var spawnPosition = new Vector3(
                        transform.position.x + Random.Range(-1f, 1f),
                        transform.position.y + Random.Range(-1f, 1f),
                        0f);

                    SceneItemsManager.Instance.InstantiateSceneItems(cropProducedItem, spawnPosition);
                }
            }
        }
    }

}