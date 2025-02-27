
public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count,

}

public enum CharacterPartAnimator
{
    none,
    body,
    arms,
    hair,
    tool,
    hat,
    count,
}

public enum PartVariantColour
{
    none,
    count,
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count,
}

public enum ToolAction
{
    usingTool,
    liftingTool,
    reapingTool,
    pickingTool,
    choppingTool,
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum InventoryLocation
{
    player,
    chest,
    count,

}

public enum ToolEffect
{
    none,
    watering,
}

public enum HarvestActionEffect
{
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none,

}

public enum ItemType
{
    Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenery,
    Furniture,
    none,
    count,

}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin,

}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
}


