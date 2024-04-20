using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combination", menuName = "Combat/Combination")]
public class ActionCombination : ScriptableObject
{
    public Action upgradedAction;
    public Action[] previousActions = new Action[3];

    public string combinationName;
    public float durationMultiplier;
    public float staminaMultiplier;
    public float composureMultiplier;
}
