using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidProperties : MonoBehaviour
{
    protected EntityOverhead entityOverhead;

    protected int maxHealth = 100;
    protected int maxStamina = 100;
    protected int maxComposure = 100;
    protected int intelligence = 5;

    protected int health;
    protected int stamina;
    protected int composure;

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

    public void SetStats(int[] points)
    {
        maxHealth = points[0] * 20;
        maxStamina = points[1] * 20;
        maxComposure = points[2] * 20;
        intelligence = points[3];
    }
}
