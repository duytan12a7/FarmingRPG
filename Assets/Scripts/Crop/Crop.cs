using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crop : MonoBehaviour
{
    private int _harvestActionCount = 0;

    [SerializeField]
    [Tooltip("This should be populated from the child transform gameobject showing harvest effect spawn point")]
    private Transform _harvestActionEffectTransform = null;

    [SerializeField]
    [Tooltip("This should be populated from the child gameobject")]
    private SpriteRenderer _cropHarvestedSpriteRenderer = null;
    [HideInInspector] public Vector2Int cropGridPosition;

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

    public void ProcessToolAction(ItemDetails itemDetails)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null) return;

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null) return;

        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null) return;

        _harvestActionCount += 1;

        int requiredHarvestActions = cropDetails.RequiredHarvestActionForTool(itemDetails.itemCode);
        if (requiredHarvestActions == -1) return;

        if (_harvestActionCount >= requiredHarvestActions)
            HarvestCrop(gridPropertyDetails, cropDetails);
    }

    private void HarvestCrop(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails)
    {
        // Delete crop from grid properties
        gridPropertyDetails.ClearCropData();
        SpawnHarvestedItems(cropDetails);
        Destroy(gameObject);
    }

    // private void SpawnHarvestedItems(CropDetails cropDetails)
    // { 
    //     for(int i = 0; i < cropDetails.cropProducedItemCode.Length; i++){
    //         int cropToProduce;

    //         if(cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i]||
    //         cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i]){
    //             cropToProduce = cropDetails.cropProducedMinQuantity[i];
    //         }
    //         else {
    //             cropToProduce = UnityEngine.Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
    //         }
    //         for(int j = 0; j< cropToProduce; j++){
    //             Vector3 spawnPosition;
    //             if(cropDetails.spawnCropProducedAtPlayerPosition){  
    //                 InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
    //             }
    //             else {
    //                 spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f), transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
    //                 SceneItemsManager.Instance.InstantiateSceneItems(cropDetails.cropProducedItemCode[i], spawnPosition);
    //             }
    //         }
    //     }
    // }
    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            var cropProduced = cropDetails.cropProducedItemCode[i];

            int minAmount = cropDetails.cropProducedMinQuantity[i];
            int maxAmount = Math.Max(minAmount, cropDetails.cropProducedMaxQuantity[i]);
            int amount = Random.Range(minAmount, maxAmount + 1);

            for (int x = 0; x < amount; x++)
            {
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropProduced);
                }
                else
                {
                    var spawnPosition =
                        new Vector3(
                            transform.position.x + Random.Range(-1f, 1f),
                            transform.position.y + Random.Range(-1f, 1f),
                            0f);

                    SceneItemsManager.Instance.InstantiateSceneItems(cropProduced, spawnPosition);
                }
            }
        }
    }
}
