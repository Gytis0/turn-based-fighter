using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidProperties : EntityProperties
{
    int baseStamina = 1000;
    [SerializeField]
    int baseStaminaDrain = 5;
    [SerializeField]
    int baseStaminaRegen = 5;
    [SerializeField]
    int composure = 100;

    float stamina;
    int staminaDrain;
    int staminaRegen;

    private void Start()
    {
        stamina = baseStamina;
        staminaDrain = baseStaminaDrain;
        staminaRegen = baseStaminaRegen;
    }

    public void DrainStamina(float _multiplier)
    {
        stamina -= staminaDrain * _multiplier;
        if (stamina < 0)
        {
            stamina = 0;
        }
        entityOverhead.UpdateStaminaSlider(stamina);
    }

    public void RegenerateStamina(float _multiplier)
    {
        stamina += staminaRegen * _multiplier;
        if (stamina > 1000)
        {
            stamina = 1000;
        }
        entityOverhead.UpdateStaminaSlider(stamina);
    }

    public float GetStamina()
    {
        return stamina;
    }

    public void TakeDamage(int damage)
    {
        AlterHealth(-damage);
    }
}
