using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    private void SetAnimationParameters(
        float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle,
        bool isCarrying, ToolEffect toolEffect,
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleUp, bool idleDown, bool idleRight, bool idleLeft)
    {
        // Set basic movement parameters
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        SetToolTriggers(isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                        isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                        isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown);

        SetPickingTriggers(isPickingRight, isPickingLeft, isPickingUp, isPickingDown);

        SetIdleTriggers(idleUp, idleDown, idleRight, idleLeft);
    }

    // Helper function to set tool usage triggers
    private void SetToolTriggers(
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown)
    {
        if (isUsingToolRight || isLiftingToolRight || isSwingingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);

        if (isUsingToolLeft || isLiftingToolLeft || isSwingingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);

        if (isUsingToolUp || isLiftingToolUp || isSwingingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);

        if (isUsingToolDown || isLiftingToolDown || isSwingingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);
    }

    // Helper function to set picking triggers
    private void SetPickingTriggers(bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown)
    {
        if (isPickingRight) animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft) animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp) animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown) animator.SetTrigger(Settings.isPickingDown);
    }

    // Helper function to set idle triggers
    private void SetIdleTriggers(bool idleUp, bool idleDown, bool idleRight, bool idleLeft)
    {
        if (idleUp) animator.SetTrigger(Settings.idleUp);
        if (idleDown) animator.SetTrigger(Settings.idleDown);
        if (idleRight) animator.SetTrigger(Settings.idleRight);
        if (idleLeft) animator.SetTrigger(Settings.idleLeft);
    }

    private void AnimationEventPlayFootstepSound()
    {
        // Footstep sound logic can be implemented here
    }
}
