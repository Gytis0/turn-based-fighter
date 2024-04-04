using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidProperties : MonoBehaviour
{
    protected EntityOverhead entityOverhead;

    protected int maxHealth = 100;
    protected int maxStamina = 100;
    protected int maxComposure = 100;

    protected int health;
    protected int stamina;
    protected int composure;
    protected int intelligence = 5;

    protected void Start()
    {
        entityOverhead = GetComponentInChildren<EntityOverhead>();

        health = maxHealth;
        stamina = maxStamina;
        composure = maxComposure;
    }

    public void DrainStamina(int change)
    {
        stamina += change;
        entityOverhead.UpdateStaminaSlider(stamina);
    }

    public void AlterHealth(int change)
    {
        health += change;
        entityOverhead.UpdateHealthSlider(health);
    }

    public float GetStamina()
    {
        return stamina;
    }
}
