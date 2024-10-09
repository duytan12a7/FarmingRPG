using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode;
    public int[] growthDays;
    public int totalGrowthDays => growthDays.Sum();
    public GameObject[] growthPrefab;
    public Sprite[] growthSprites;
    public Season[] seasons;
    public Sprite harvestedSprite;

    [ItemCodeDescription]
    public int harvestedTransfromItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;

    public bool isHarvestedAnimation;
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;
    public int[] requiredHarvestActions;

    [ItemCodeDescription]
    public int[] cropProducedItemCode;
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;

    private Dictionary<int, int> harvestToolActionMap;

    private void InitializeToolActionMap()
    {
        if (harvestToolActionMap != null) return;

        harvestToolActionMap = new Dictionary<int, int>();

        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            harvestToolActionMap[harvestToolItemCode[i]] = requiredHarvestActions[i];
        }
    }

    public bool CanUseToolHarvestCrop(int toolItemCode)
    {
        return RequiredHarvestActionForTool(toolItemCode) != -1;
    }

    public int RequiredHarvestActionForTool(int toolItemCode)
    {
        InitializeToolActionMap();

        return harvestToolActionMap.TryGetValue(toolItemCode, out int requiredActions) ? requiredActions : -1;
    }

    public int GetGrowthStageForDays(int days)
    {
        int daysCounter = totalGrowthDays;
        for (int x = growthDays.Length - 1; x >= 0; x--)
        {
            if (days >= daysCounter)
            {
                return x;
            }
            daysCounter -= growthDays[x];
        }

        throw new Exception($"Unexpected error calculating growth stage for days '{days}'");
    }
}
