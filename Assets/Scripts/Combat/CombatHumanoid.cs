using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHumanoid : MonoBehaviour
{
    protected Equipment equipment;
    protected HumanoidProperties humanoidProperties;
    protected HumanoidAnimationController humanoidAnimationController;
    protected HumanoidMovementController humanoidMovementController;

    protected bool inCombat = false;
    protected bool isOneHanded = false;

    protected Queue<Action> actionQueue = new Queue<Action>();
    protected List<ActionCombination> combinations = new List<ActionCombination>();

    protected List<Action> availableActions = new List<Action>();

    protected CombatState combatState = CombatState.Standing;

    [SerializeField] protected float dodgeSpeed = 5f;
    [SerializeField] protected float runSpeed = 3f;
    [SerializeField] protected float walkSpeed = 1f;

    protected float gridSpacing;

    protected void Start()
    {
        equipment = transform.GetComponent<Equipment>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
        humanoidAnimationController = transform.GetComponent<HumanoidAnimationController>();
        humanoidMovementController = transform.GetComponent<HumanoidMovementController>();
    }

    public void SetCombinations(List<ActionCombination> combinations)
    {
        this.combinations = combinations;
    }

    public virtual void EndTurn()
    {
        
    }

    public virtual void EnableCombatMode(bool _enable, float gridSpacing)
    {
        inCombat = _enable;
        humanoidAnimationController.SetAnimationModes(equipment.IsTwoHanded(), equipment.IsLeftHanded(), inCombat);
        this.gridSpacing = gridSpacing;
        humanoidAnimationController.SetState(AnimationStates.IDLE);
    }

    public virtual void ExecuteAction(Action action)
    {
        if (action.actionName == ActionName.Block)
        {
            humanoidAnimationController.SetState(AnimationStates.BLOCKING);
        }
        else if (action.actionName == ActionName.Dodge)
        {
            if (action.directions[0] == Direction.Forward)
            {
                humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing, dodgeSpeed, AnimationStates.DODGING_FORWARD, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_FORWARD);
            }
            else if (action.directions[0] == Direction.Backward)
            {
                humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing, dodgeSpeed, AnimationStates.DODGING_BACKWARD, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_BACKWARD);
            }
            else if (action.directions[0] == Direction.Left)
            {
                humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing, dodgeSpeed, AnimationStates.DODGING_LEFT, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_LEFT);
            }
            else if (action.directions[0] == Direction.Right)
            {
                humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing, dodgeSpeed, AnimationStates.DODGING_RIGHT, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_RIGHT);
            }
        }
        else if (action.actionName == ActionName.Duck)
        {
            humanoidAnimationController.SetState(AnimationStates.DUCKING);
        }
        else if (action.actionName == ActionName.Get_Up)
        {
            humanoidAnimationController.SetState(AnimationStates.GETTING_UP);
        }
        else if (action.actionName == ActionName.Overhead)
        {
            humanoidAnimationController.SetState(AnimationStates.OVERHEAD);
        }
        else if (action.actionName == ActionName.Kick)
        {
            humanoidAnimationController.SetState(AnimationStates.KICKING);
        }
        else if (action.actionName == ActionName.Roll)
        {
            if (action.directions[0] == Direction.Left) humanoidAnimationController.SetState(AnimationStates.ROLLING_LEFT);
            else if (action.directions[0] == Direction.Right) humanoidAnimationController.SetState(AnimationStates.ROLLING_RIGHT);
        }
        else if (action.actionName == ActionName.Run)
        {
            if (action.directions[0] == Direction.Forward) humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Backward) humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Left) humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Right) humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);

            humanoidAnimationController.SetState(AnimationStates.RUNNING);
        }
        else if (action.actionName == ActionName.Stab)
        {
            humanoidAnimationController.SetState(AnimationStates.STABBING);
        }
        else if (action.actionName == ActionName.Stand_Ground)
        {
            humanoidAnimationController.SetState(AnimationStates.STAND_GROUND);
        }
        else if (action.actionName == ActionName.Step)
        {
            if (action.directions[0] == Direction.Forward) humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Backward) humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Left) humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Right) humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing, walkSpeed, AnimationStates.WALKING);

            humanoidAnimationController.SetState(AnimationStates.WALKING);
        }
        else if (action.actionName == ActionName.Swing)
        {
            if (action.directions[0] == Direction.Left) humanoidAnimationController.SetState(AnimationStates.SWINGING_LEFT);
            else if (action.directions[0] == Direction.Right) humanoidAnimationController.SetState(AnimationStates.SWINGING_RIGHT);
        }
        else if(action.actionName == ActionName.Throw)
        {
            humanoidAnimationController.SetState(AnimationStates.THROWING);
        }
    }

    void AddActionsToQueue(List<Action> actions)
    {

    }

    public float GetStamina()
    {
        return humanoidProperties.GetStamina();
    }

    public CombatState GetCombatState() { return combatState; }

    public float GetWeaponWeight()
    {
        return equipment.GetWeaponWeight();
    }

    public float GetShieldWeight()
    {
        return equipment.GetShieldWeight();
    }

    public float GetAllWeight()
    {
        return equipment.GetAllWeight();
    }

    public List<ActionName> GetShieldActions()
    {
        return equipment.GetShieldActions();
    }

    public List<ActionName> GetWeaponActions()
    {
        return equipment.GetWeaponActions();
    }
}
