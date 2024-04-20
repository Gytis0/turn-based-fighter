using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Actions/Combination")]
public class ActionCombination : ScriptableObject
{
    public Action[] actions = new Action[3];
    public float staminaMultiplier;
    public float composureMultiplier;
    
}
