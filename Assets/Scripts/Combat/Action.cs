using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Combat/Action")]
public class Action : ScriptableObject
{
    public Action(ActionType actionType, Direction direction = Direction.None)
    {
        this.actionType = actionType;
        this.direction = direction;
    }

    public enum ActionType
    { 
        Movement,
        Agile,
        Offensive,
        Defensive,
        Skip
    }

    public enum State
    {
        Any,
        Standing,
        Walking,
        Running,
        Fallen,
        Dodging
    }
    public enum Direction
    {
        None,
        Forward,
        Backward,
        Left,
        Right
    }
    public string actionName;
    public int baseDuration;
    public int baseStaminaDrain;
    public int baseComposureDrain;

    public ActionType actionType;
    public List<State> availableWhen;
    public Direction direction;
}
