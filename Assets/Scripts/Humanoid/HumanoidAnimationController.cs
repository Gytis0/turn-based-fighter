using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;
    public HumanoidMovementController movementController;

    string path;
    AnimationStates animationStates;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        movementController = transform.GetComponent<HumanoidMovementController>();
    }
    
    void UpdateStates(AnimationStates newState)
    {
        foreach (AnimatorControllerParameter state in animator.parameters)
        {
            if(state.name.ToLower() == newState.ToString().ToLower())
            {
                animator.SetBool(state.name, true);
            }
            else
            {
                animator.SetBool(state.name, false);
            }
        }
    }

    private void OnEnable()
    {
        movementController.onChangedState += UpdateStates;
    }

    private void OnDisable()
    {
        movementController.onChangedState -= UpdateStates;
    }
}
