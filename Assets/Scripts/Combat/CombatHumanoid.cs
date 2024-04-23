using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHumanoid : MonoBehaviour
{
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

    public virtual void EndTurn()
    {
        
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
