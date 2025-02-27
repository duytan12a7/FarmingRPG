using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabEvent += InstantiateCropPrefabs;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabEvent -= InstantiateCropPrefabs;
    }

    private void InstantiateCropPrefabs()
    {
        grid = FindObjectOfType<Grid>();

        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode < 1) return;

        GridPropertyDetails gridPropertyDetails;

        gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            gridPropertyDetails = new GridPropertyDetails();

        gridPropertyDetails.daysSinceDug = daysSinceDug;
        gridPropertyDetails.daysSinceWatered = daysSinceWatered;
        gridPropertyDetails.seedItemCode = seedItemCode;
        gridPropertyDetails.growthDays = growthDays;

        GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
    }
}
