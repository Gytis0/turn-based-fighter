using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsOverhead : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    EvenDashLayout healthDashLayout;
    IntervalSlider healthIntervalSlider;

    [SerializeField] Slider staminaSlider;
    EvenDashLayout staminaDashLayout;
    IntervalSlider staminaIntervalSlider;

    [SerializeField] Slider composureSlider;
    EvenDashLayout composureDashLayout;
    IntervalSlider composureIntervalSlider;


    private void Awake()
    {
        healthDashLayout = healthSlider.GetComponentInChildren<EvenDashLayout>();
        staminaDashLayout = staminaSlider.GetComponentInChildren<EvenDashLayout>();
        composureDashLayout = composureSlider.GetComponentInChildren<EvenDashLayout>();

        healthIntervalSlider = healthSlider.GetComponent<IntervalSlider>();
        staminaIntervalSlider = staminaSlider.GetComponent<IntervalSlider>();
        composureIntervalSlider = composureSlider.GetComponent<IntervalSlider>();
    }
    public void UpdateHealthSlider(float newHealth)
    {
        healthSlider.value = newHealth;
    }

    public void UpdateStaminaSlider(float newStamina)
    {
        staminaSlider.value = newStamina;
    }

    public void UpdateComposureSlider(float newComposure)
    {
        composureSlider.value = newComposure;
    }

    public void UpdateHealthInterval(float change)
    {
        healthIntervalSlider.AlterInterval(change);
    }

    public void UpdateStaminaInterval(float change)
    {
        staminaIntervalSlider.AlterInterval(change);
    }

    public void UpdateComposureInterval(float change)
    {
        composureIntervalSlider.AlterInterval(change);
    }

    public void SetSlidersValues(float health, float stamina, float composure)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthDashLayout.SetDashes();

        staminaSlider.maxValue = stamina;
        staminaSlider.value = stamina;
        staminaDashLayout.SetDashes();

        composureSlider.maxValue = composure;
        composureSlider.value = composure;
        composureDashLayout.SetDashes();
    }

    public void SetSlidersIntervals(float[] stats)
    {
        healthIntervalSlider.SetInterval(stats[0], stats[1]);
        staminaIntervalSlider.SetInterval(stats[2], stats[3]);
        composureIntervalSlider.SetInterval(stats[4], stats[5]);
    }
}
