using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidProperties : MonoBehaviour
{
    [SerializeField] bool isPropertiesPrecise = true;
    [SerializeField] GameObject floatingNumber;
    protected CharacterStatsOverhead characterStats;

    protected int maxHealth = 100;
    protected int maxStamina = 100;
    protected int maxComposure = 100;
    protected int intelligence = 5;

    protected float health;
    protected float stamina;
    protected float composure;

    Color blue = new Color(0f, 0.5f, 1f);
    Color yellow = new Color(1f, 0.64f, 0f);

    protected void Awake()
    {
        characterStats = GetComponentInChildren<CharacterStatsOverhead>();
    }
    protected void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        composure = maxComposure;
    }

    public void AlterHealth(float change)
    {
        health += change;
        if(isPropertiesPrecise) characterStats.UpdateHealthSlider(health);
        else characterStats.UpdateHealthInterval(change);
        FloatingNumber floatingNumber = Instantiate(this.floatingNumber, characterStats.transform).GetComponent<FloatingNumber>();
        floatingNumber.SetNumber(change, Color.red);
    }
    public void AlterStamina(float change)
    {
        stamina = Mathf.Clamp(stamina + change, 0, maxStamina);
        if (isPropertiesPrecise) characterStats.UpdateStaminaSlider(stamina);
        else characterStats.UpdateStaminaInterval(change);
        FloatingNumber floatingNumber = Instantiate(this.floatingNumber, characterStats.transform).GetComponent<FloatingNumber>();
        floatingNumber.SetNumber(change, blue);
    }
    public void AlterComposure(float change)
    {
        composure = Mathf.Clamp(composure + change, 0, maxComposure);
        if (isPropertiesPrecise) characterStats.UpdateComposureSlider(composure);
        else characterStats.UpdateComposureInterval(change);
        FloatingNumber floatingNumber = Instantiate(this.floatingNumber, characterStats.transform).GetComponent<FloatingNumber>();
        floatingNumber.SetNumber(change, yellow);
    }

    public float GetStamina() { return stamina; }
    public float GetHealth() { return health; }
    public float GetComposure() { return composure; }
    public int GetMaxHealth() { return maxHealth; }
    public int GetMaxStamina() { return maxStamina; }
    public int GetMaxComposure() { return maxComposure; }
    public int GetIntelligence() { return intelligence; }

    public void SetStats(int[] points, float[] intervals = null)
    {
        maxHealth = points[0] * 20;
        maxStamina = points[1] * 20;
        maxComposure = points[2] * 20;
        intelligence = points[3];

        health = maxHealth;
        stamina = maxStamina;
        composure = maxComposure;

        if (intervals == null) characterStats.SetSlidersValues(health, stamina, composure);
        else characterStats.SetSlidersIntervals(intervals);
    }
    public void EnableCharacterStatsOverhead(bool enable)
    {
        characterStats.gameObject.GetComponent<Canvas>().enabled = enable;
    }
}
