using System.Collections;
using System.Collections.Generic;
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

    private void SetAnimationParameters(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle,
   bool isCarrying, ToolEffect toolEffect, bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
   bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
   bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
   bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
   bool idleUp, bool idleDown, bool idleRight, bool idleLeft)
    {
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        // Gọi hàm SetTrigger cho từng hành động
        SetTriggers(new (bool, int)[] {
            (isUsingToolRight, Settings.isUsingToolRight),
            (isUsingToolLeft, Settings.isUsingToolLeft),
            (isUsingToolUp, Settings.isUsingToolUp),
            (isUsingToolDown, Settings.isUsingToolDown),
            (isLiftingToolRight, Settings.isLiftingToolRight),
            (isLiftingToolLeft, Settings.isLiftingToolLeft),
            (isLiftingToolUp, Settings.isLiftingToolUp),
            (isLiftingToolDown, Settings.isLiftingToolDown),
            (isSwingingToolRight, Settings.isSwingingToolRight),
            (isSwingingToolLeft, Settings.isSwingingToolLeft),
            (isSwingingToolUp, Settings.isSwingingToolUp),
            (isSwingingToolDown, Settings.isSwingingToolDown),
            (isPickingRight, Settings.isPickingRight),
            (isPickingLeft, Settings.isPickingLeft),
            (isPickingUp, Settings.isPickingUp),
            (isPickingDown, Settings.isPickingDown),
            (idleUp, Settings.idleUp),
            (idleDown, Settings.idleDown),
            (idleRight, Settings.idleRight),
            (idleLeft, Settings.idleLeft),
        });
    }

    private void SetTriggers((bool condition, int trigger)[] triggers)
    {
        foreach (var (condition, triggerValue) in triggers)
        {
            if (condition)
            {
                animator.SetTrigger(triggerValue);
            }
        }
    }

    private void AnimationEventPlayFootstepSound()
    {
        // Code for playing footstep sound can be implemented here
    }
}
