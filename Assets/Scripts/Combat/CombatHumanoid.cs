using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHumanoid : MonoBehaviour
{
    public delegate void TurnDone(Action action);
    public event TurnDone onTurnDone;

    protected Equipment equipment;
    protected HumanoidProperties humanoidProperties;
    protected bool inCombat = false;

    protected Queue<Action> actionQueue = new Queue<Action>();
    protected List<ActionCombination> combinations = new List<ActionCombination>();

    protected List<Action> availableActions = new List<Action>();

    protected CombatState combatState = CombatState.Standing;

    protected void Start()
    {
        equipment = transform.GetComponent<Equipment>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
    }

    public void SetCombinations(List<ActionCombination> combinations)
    {
        this.combinations = combinations;
    }

    public void EndTurn()
    {
        if(actionQueue.Count == 0)
        {
            onTurnDone(new Action(ActionType.Skip));
        }
        else
        {
            onTurnDone(actionQueue.Dequeue());
        }
    }

    public virtual void EnableCombatMode(bool _enable)
    {
        inCombat = _enable;
    }

    public void ExecuteAction(Action action)
    {
        
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
