using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Sprite emptyBubble;
    [SerializeField] Sprite fullBubble;

    [SerializeField] TextMeshProUGUI availablePointsText;

    [SerializeField] public List<Image> healthBubbles;
    [SerializeField] List<Image> staminaBubbles;
    [SerializeField] List<Image> composureBubbles;
    [SerializeField] List<Image> intelligenceBubbles;

    

    int healthPoints = 3;
    int staminaPoints = 3;
    int composurePoints = 3;
    int intelligencePoints = 3;

    int availablePoints = 0;

    private void Start()
    {
        updateAllPanels();

    }

    public void changeHealthPoints(int change) {
        if (availablePoints == 0 && change > 0) return;

        if (change < 0 && healthPoints > 1) { setAvailaiblePoints(1); }
        if (change > 0 && healthPoints < 5) { setAvailaiblePoints(-1); }

        healthPoints = Mathf.Clamp(healthPoints + change, 1, 5);
        updatePanel(healthPoints, healthBubbles);
    }

    public void changeStaminaPoints(int change) {
        if (availablePoints == 0 && change > 0) return;

        if (change < 0 && staminaPoints > 1) { setAvailaiblePoints(1); }
        if (change > 0 && staminaPoints < 5) { setAvailaiblePoints(-1); }

        staminaPoints = Mathf.Clamp(staminaPoints + change, 1, 5);
        updatePanel(staminaPoints, staminaBubbles);
    }

    public void changeComposurePoints(int change)
    {
        if (availablePoints == 0 && change > 0) return;

        if (change < 0 && composurePoints > 1) { setAvailaiblePoints(1); }
        if (change > 0 && composurePoints < 5) { setAvailaiblePoints(-1); }

        composurePoints = Mathf.Clamp(composurePoints + change, 1, 5);
        updatePanel(composurePoints, composureBubbles);
    }

    public void changeIntelligencePoints(int change)
    {
        if (availablePoints == 0 && change > 0) return;

        if (change < 0 && intelligencePoints > 1) { setAvailaiblePoints(1); }
        if (change > 0 && intelligencePoints < 5) { setAvailaiblePoints(-1); }

        intelligencePoints = Mathf.Clamp(intelligencePoints + change, 1, 5);
        updatePanel(intelligencePoints, intelligenceBubbles);
    }

    void updatePanel(int points, List<Image> images)
    {
        int i = 1;
        foreach (Image image in images)
        {
            if (i <= points) image.sprite = fullBubble;
            else image.sprite = emptyBubble;
            i++;
        }
    }

    void updateAllPanels()
    {
        updatePanel(healthPoints, healthBubbles);
        updatePanel(staminaPoints, staminaBubbles);
        updatePanel(composurePoints, composureBubbles);
        updatePanel(intelligencePoints, intelligenceBubbles);
    }

    void setAvailaiblePoints(int points)
    {
        availablePoints += points;
        availablePointsText.SetText("Points Left: " + availablePoints);
    }

    public void ResetPoints()
    {
        healthPoints = staminaPoints = composurePoints = intelligencePoints = 3;
        availablePoints = 0;
        availablePointsText.SetText("Points Left: " + availablePoints);
        updateAllPanels();
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
