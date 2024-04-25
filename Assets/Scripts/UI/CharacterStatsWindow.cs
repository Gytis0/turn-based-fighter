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
        healthText.SetText("Health: " + health);
        healthDashLayout.SetDashes();

        staminaSlider.maxValue = stamina;
        staminaSlider.value = stamina;
        staminaText.SetText("Stamina: " + stamina);
        staminaDashLayout.SetDashes();

        composureSlider.maxValue = composure;
        composureSlider.value = composure;
        composureText.SetText("Composure: " + composure);
        composureDashLayout.SetDashes();
    }

    public void SetSlidersIntervals(float minHealth, float maxHealth, float minStamina, float maxStamina, float minComposure, float maxComposure)
    {
        healthIntervalSlider.SetInterval(minHealth, maxHealth);
        staminaIntervalSlider.SetInterval(minStamina, maxStamina);
        composureIntervalSlider.SetInterval(minComposure, maxComposure);

        healthText.SetText("Health: " + minHealth + "-" + maxHealth);
        staminaText.SetText("Stamina: " + minStamina + "-" + maxStamina);
        composureText.SetText("Composure: " + minComposure + "-" + maxComposure);
    }
}
