using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Combat/Action")]
public class Action : ScriptableObject
{
    public Action(ActionType actionType)
    {
        this.actionType = actionType;
    }

    public Action(Action action)
    {
        actionName = action.actionName;
        baseDuration = action.baseDuration;
        baseStaminaDrain = action.baseStaminaDrain;
        baseComposureDrain = action.baseComposureDrain;
        actionType = action.actionType;
        availableWhen = action.availableWhen;
        directions = action.directions;
    }

    public Action(Action action, Direction direction)
    {
        actionName = action.actionName;
        baseDuration = action.baseDuration;
        baseStaminaDrain = action.baseStaminaDrain;
        baseComposureDrain = action.baseComposureDrain;
        actionType = action.actionType;
        availableWhen = action.availableWhen;
        directions = new List<Direction>() { direction };
    }



    public ActionName actionName;
    public int baseDuration;
    public int baseStaminaDrain;
    public int baseComposureDrain;

    public ActionType actionType;
    public List<CombatState> availableWhen;
    public List<Direction> directions;

    public void AddDirection(Direction direction)
    {
        directions.Add(direction);
        directions.Remove(Direction.None);
    }
}
