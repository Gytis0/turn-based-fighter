using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsWindow : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    EvenDashLayout healthDashLayout;
    IntervalSlider healthIntervalSlider;
    TextMeshProUGUI healthText;

    [SerializeField] Slider staminaSlider;
    EvenDashLayout staminaDashLayout;
    IntervalSlider staminaIntervalSlider;
    TextMeshProUGUI staminaText;

    [SerializeField] Slider composureSlider;
    EvenDashLayout composureDashLayout;
    IntervalSlider composureIntervalSlider;
    TextMeshProUGUI composureText;

    [SerializeField] TextMeshProUGUI title;

    private void Awake()
    {
        healthDashLayout = healthSlider.GetComponentInChildren<EvenDashLayout>();
        staminaDashLayout = staminaSlider.GetComponentInChildren<EvenDashLayout>();
        composureDashLayout = composureSlider.GetComponentInChildren<EvenDashLayout>();

        healthIntervalSlider = healthSlider.GetComponent<IntervalSlider>();
        staminaIntervalSlider = staminaSlider.GetComponent<IntervalSlider>();
        composureIntervalSlider = composureSlider.GetComponent<IntervalSlider>();

        healthText = healthSlider.transform.GetComponentInChildren<TextMeshProUGUI>();
        staminaText = staminaSlider.transform.GetComponentInChildren<TextMeshProUGUI>();
        composureText = composureSlider.transform.GetComponentInChildren<TextMeshProUGUI>();
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

    public void SetTitle(string title)
    {
        this.title.SetText(title);
    }

    public void SetSlidersValues(float health, float stamina, float composure)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthText.SetText("Health: " + health.ToString("0.00"));
        healthDashLayout.SetDashes();

        staminaSlider.maxValue = stamina;
        staminaSlider.value = stamina;
        staminaText.SetText("Stamina: " + stamina.ToString("0.00"));
        staminaDashLayout.SetDashes();

        composureSlider.maxValue = composure;
        composureSlider.value = composure;
        composureText.SetText("Composure: " + composure.ToString("0.00"));
        composureDashLayout.SetDashes();
    }
    public void SetSlidersValues(float health, float maxHealth, float stamina, float maxStamina, float composure, float maxComposure)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        healthText.SetText("Health: " + health.ToString("0.00"));
        healthDashLayout.SetDashes();

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        staminaText.SetText("Stamina: " + stamina.ToString("0.00"));
        staminaDashLayout.SetDashes();

        composureSlider.maxValue = maxComposure;
        composureSlider.value = composure;
        composureText.SetText("Composure: " + composure.ToString("0.00"));
        composureDashLayout.SetDashes();
    }


    public void SetSlidersIntervals(float[] stats)
    {
        healthIntervalSlider.SetInterval(stats[0], stats[1]);
        staminaIntervalSlider.SetInterval(stats[2], stats[3]);
        composureIntervalSlider.SetInterval(stats[4], stats[5]);

        healthText.SetText("Health: " + stats[0] + "-" + stats[1]);
        staminaText.SetText("Stamina: " + stats[2] + "-" + stats[3]);
        composureText.SetText("Composure: " + stats[4] + "-" + stats[5]);
    }
}
