using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHumanoid : MonoBehaviour
{
    public delegate void TurnDone(Action action);
    public event TurnDone onTurnDone;

    protected HumanoidMovementController humanoidMovement;
    protected HumanoidProperties humanoidProperties;
    protected bool inCombat = false;

    protected Queue<Action> actionQueue = new Queue<Action>();
    protected List<ActionCombination> combinations = new List<ActionCombination>();

    protected void Start()
    {
        humanoidMovement = transform.GetComponent<HumanoidMovementController>();
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
            onTurnDone(new Action(Action.ActionType.Skip));
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
}
