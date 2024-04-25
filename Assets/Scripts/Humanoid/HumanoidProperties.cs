using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidProperties : MonoBehaviour
{
    protected CharacterStatsUI entityOverhead;

    protected int maxHealth = 100;
    protected int maxStamina = 100;
    protected int maxComposure = 100;
    protected int intelligence = 5;

    protected float health;
    protected float stamina;
    protected float composure;

    protected void Start()
    {
        entityOverhead = GetComponentInChildren<CharacterStatsUI>();

        health = maxHealth;
        stamina = maxStamina;
        composure = maxComposure;
    }

    

    public void AlterHealth(int change)
    {
        health += change;
        entityOverhead.UpdateHealthSlider(health);
    }
    public void AlterStamina(float change)
    {
        stamina += change;
        entityOverhead.UpdateStaminaSlider(stamina);
    }
    public void AlterComposure(float change)
    {
        composure += change;
        entityOverhead.UpdateComposureSlider(composure);
    }

    public float GetStamina() { return stamina; }
    public float GetHealth() { return health; }
    public float GetComposure() { return composure; }
    public int GetMaxComposure() { return maxComposure; }
    public int GetIntelligence() { return intelligence; }

    public void SetStats(int[] points)
    {
        maxHealth = points[0] * 20;
        maxStamina = points[1] * 20;
        maxComposure = points[2] * 20;
        intelligence = points[3];

        health = maxHealth;
        stamina = maxStamina;
        composure = maxComposure;
    }
}
