using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidStats : MonoBehaviour
{
    HumanoidMovementController humanoidMovement;
    HumanoidProperties humanoidProperties;
    public bool inCombat = false;
    public AnimationStates currentState = AnimationStates.IDLE;
    public Transform followingObj;
    public bool following = false;

    void Start()
    {
        humanoidMovement = transform.GetComponent<HumanoidMovementController>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
    }

    public void EnableCombatMode(bool _enable)
    {
        inCombat = _enable;
    }

    private void Update()
    {

    }
}
