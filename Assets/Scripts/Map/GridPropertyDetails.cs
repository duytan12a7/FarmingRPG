using System;

[System.Serializable]
public class GridPropertyDetails
{
    public int gridX;
    public int gridY;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;

    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;

    public GridPropertyDetails()
    {

    }

    public void ClearCropData()
    {
        seedItemCode = -1;
        growthDays = -1;
        daysSinceLastHarvest = -1;
        daysSinceWatered = -1;
    }
}
