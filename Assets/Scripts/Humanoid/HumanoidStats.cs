using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidStats : MonoBehaviour
{
    HumanoidMovementController humanoidMovement;
    HumanoidProperties humanoidProperties;
    public bool inCombat = false;
    public string currentState = "Idle";
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
        if (inCombat)
        {
            if (currentState == "Walking")
            {
                humanoidProperties.DrainStamina(1);
            }
            else if (currentState == "Running")
            {
                humanoidProperties.DrainStamina(2);
            }
        }
        else
        {
            humanoidProperties.RegenerateStamina(1);
        }
    
    }

    public void TakeDamage(int damage)
    {
        humanoidProperties.TakeDamage(damage);
    }
}
