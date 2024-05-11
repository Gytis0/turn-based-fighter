using UnityEngine;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;
    AnimationStates animationState;
    float animationDuration = 0f;

    float tempTimer = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    
    public void SetState(AnimationStates newState, float duration = 0f)
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
    }

    public void SetAnimationModes(bool isTwoHanded, bool isLeftHanded, bool inCombat)
    {
        animator.SetBool("Two Handed", isTwoHanded);
        animator.SetBool("Left Handed", isLeftHanded);
        animator.SetBool("In Combat", inCombat);
    }

    public string GetCurrentAnimationName()
    {
        if (animator == null || animator.GetCurrentAnimatorClipInfoCount(0) == 0) return "";
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
        if (animator == null) return;

        if(animationDuration == 0f)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", GetCurrentAnimationLength() / animationDuration);
        }
    }
}
