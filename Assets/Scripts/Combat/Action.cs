using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Actions/Action")]
public class Action : ScriptableObject
{
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
    public string actionName;
    public int baseDuration;
    public int baseStaminaDrain;
    public ActionType actionType;
    public List<State> availableWhen;
}
