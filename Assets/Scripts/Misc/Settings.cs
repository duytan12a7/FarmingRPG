using UnityEngine;

public static class Settings
{
    // Fade Settings
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;

    //Tilemap
    public const float gridCellSize = 1f;
    public static Vector2 cursorSize = Vector2.one;

    // Player
    public static float playerCentreYOffset = 0.875f;

    // Reaping
    public const int maxCollidersToTestReapSwing = 15;
    public const int maxTargetComponentsToDestroyReapSwing = 2;

    // Player Movement
    public const float DiagonalMovementFactor = 0.71f;
    public const float runningSpeed = 5.333f;
    public const float walkingSpeed = 2.666f;

    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;

    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.2f;

    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;

    // Inventory
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaxmumInventoryCapacity = 48;


    // Player Animations Parameters
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;

    // Shared Animation Parameters

    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    // Tools
    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    // Time System
    public const float secondsPerGameSecond = 0.012f;





    // static constructor
    static Settings()
    {


        // Player Animation Parameters
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");

        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");

        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");

        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");

        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        // Shared Animation parameters
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");


    }

}
