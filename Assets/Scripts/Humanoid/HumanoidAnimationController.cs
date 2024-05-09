using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static log4net.Appender.RollingFileAppender;
using UnityEditor.UIElements;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;
    AnimationStates animationState;
    float animationDuration = 1f;

    float tempTimer = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    
    public void SetState(AnimationStates newState, float duration = 1f)
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

        animationDuration = duration;
        if(animationState == AnimationStates.IDLE)
        {
            Debug.Log(tempTimer);
        }
        else if(animationState == AnimationStates.ROLLING_LEFT || animationState == AnimationStates.ROLLING_RIGHT || animationState == AnimationStates.FALLING) 
        {
            tempTimer = 0f;
        }
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

    float GetCurrentAnimationLength()
    {
        if (animator.GetCurrentAnimatorClipInfoCount(0) == 0) return 0f;
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }

    private void Update()
    {
        animator.SetFloat("Speed", GetCurrentAnimationLength() / animationDuration);
        tempTimer += Time.deltaTime;
    }
}
