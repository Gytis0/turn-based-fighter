using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;

    AnimationStates animationState;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    
    public void SetState(AnimationStates newState)
    {
        foreach (AnimatorControllerParameter state in animator.parameters)
        {
            if(state.name.ToLower() == newState.ToString().ToLower())
            {
                animator.SetTrigger(state.name);
                break;
            }
        }
        animationState = newState;
    }

    public void SetAnimationModes(bool isTwoHanded, bool isLeftHanded, bool inCombat)
    {
        animator.SetBool("Two Handed", isTwoHanded);
        animator.SetBool("Left Handed", isLeftHanded);
        animator.SetBool("In Combat", inCombat);
    }

    public string GetCurrentAnimationName()
    {
        if (animator.GetCurrentAnimatorClipInfoCount(0) == 0) return "";
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.ToString();
    }
    public AnimationStates GetCurrentAnimationState() { return animationState; }

    private void Update()
    {
        //if (GetCurrentAnimationName().ToLower().Contains("idle")) animationState = AnimationStates.IDLE;
    }
}
