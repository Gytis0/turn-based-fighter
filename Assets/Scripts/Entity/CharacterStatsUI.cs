using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsUI : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider staminaSlider;
    [SerializeField] Slider composureSlider;

    public void UpdateHealthSlider(int newHealth)
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
}
