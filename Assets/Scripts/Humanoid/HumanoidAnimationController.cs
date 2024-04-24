using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;
    HumanoidMovementController movementController;

    AnimationStates animationState;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        movementController = transform.GetComponent<HumanoidMovementController>();
    }
    
    public void SetState(AnimationStates newState)
    {
        animationState = newState;
        foreach (AnimatorControllerParameter state in animator.parameters)
        {
            if(state.name.ToLower() == newState.ToString().ToLower())
            {
                animator.SetTrigger(state.name);
                break;
            }
        }
    }

    public void SetAnimationModes(bool isTwoHanded, bool isLeftHanded, bool inCombat)
    {
        animator.SetBool("Two Handed", isTwoHanded);
        animator.SetBool("Left Handed", isLeftHanded);
        animator.SetBool("In Combat", inCombat);
    }

    private void OnEnable()
    {
        movementController.onChangedState += SetState;
    }

    private void OnDisable()
    {
        movementController.onChangedState -= SetState;
    }
}
