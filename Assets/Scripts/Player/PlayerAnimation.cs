using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerController;

    protected bool isLiftingToolRight;
    protected bool isLiftingToolLeft;
    protected bool isLiftingToolUp;
    protected bool isLiftingToolDown;
    protected bool isUsingToolRight;
    protected bool isUsingToolLeft;
    protected bool isUsingToolUp;
    protected bool isUsingToolDown;
    protected bool isSwingingToolRight;
    protected bool isSwingingToolLeft;
    protected bool isSwingingToolUp;
    protected bool isSwingingToolDown;
    protected bool isPickingRight;
    protected bool isPickingLeft;
    protected bool isPickingUp;
    protected bool isPickingDown;

    protected ToolEffect toolEffect = ToolEffect.none;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }

    public void CallMovementEvent()
    {
        EventHandler.CallMovementEvent(playerController.PlayerMovement.XInput(), playerController.PlayerMovement.YInput(),
            playerController.PlayerMovement.IsWalking(), playerController.PlayerMovement.IsRunning(), playerController.PlayerMovement.IsIdle(),
            playerController.PlayerCarriedItemHandler.IsCarrying(), toolEffect, isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
            false, false, false, false);
    }
}
