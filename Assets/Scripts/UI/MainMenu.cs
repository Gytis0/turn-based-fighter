using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] TextMeshProUGUI matchesPlayedText;
    [SerializeField] TextMeshProUGUI matchesWonText;
    [SerializeField] TextMeshProUGUI averageMatchTimeText;
    [SerializeField] TextMeshProUGUI actionsTakenText;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] TextMeshProUGUI averageHealthChoiceText;
    [SerializeField] TextMeshProUGUI averageStaminaChoiceText;
    [SerializeField] TextMeshProUGUI averageComposureChoiceText;
    [SerializeField] TextMeshProUGUI averageIntelligenceChoiceText;
    [SerializeField] TextMeshProUGUI oneHandedWeaponChoicesText;

    public delegate void StartGame();
    public static event StartGame onGameStart;

    int healthPoints = 3;
    int staminaPoints = 3;
    int composurePoints = 3;
    int intelligencePoints = 3;

    int availablePoints = 0;

    public int[] GetPoints()
    {
        return new int[] { healthPoints, staminaPoints, composurePoints, intelligencePoints };
    }

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

    public void UpdateStats()
    {
        matchesPlayedText.SetText("Matches played: " + PlayerPrefs.GetInt("matchesPlayed"));
        
        actionsTakenText.SetText("Actions taken: " + PlayerPrefs.GetInt("actionsTaken"));
        bestScoreText.SetText("Best score: " + PlayerPrefs.GetInt("bestScore"));
        

        int hours, minutes, seconds;
        hours = minutes = seconds = 0;
        try
        {
            float averageTotalTime = PlayerPrefs.GetInt("totalTime") / PlayerPrefs.GetInt("countTime");
            if(!float.IsNaN(averageTotalTime))
            {
                hours = (int)(averageTotalTime / 3600);
                averageTotalTime -= hours * 3600;
                minutes = (int)(averageTotalTime / 60);
                averageTotalTime -= minutes * 60;
                seconds = (int)averageTotalTime;
            }
        }
        catch (DivideByZeroException e) { Debug.LogError("Divided by 0");    }
        
        try { averageMatchTimeText.SetText("Average match time: " + hours + ":" + minutes + ":" + seconds); } catch (Exception) { }
        try { matchesWonText.SetText("Matches won: " + PlayerPrefs.GetInt("matchesWon") / PlayerPrefs.GetInt("matchesPlayed") * 100 + "%"); } catch (Exception) { }
        try { averageHealthChoiceText.SetText("Average health choice: " + (PlayerPrefs.GetInt("totalHealth") / PlayerPrefs.GetInt("countHealth"))); } catch (Exception) { }
        try { averageStaminaChoiceText.SetText("Average stamina choice: " + (PlayerPrefs.GetInt("totalStamina") / PlayerPrefs.GetInt("countStamina"))); } catch (Exception) { }
        try { averageComposureChoiceText.SetText("Average composure choice: " + (PlayerPrefs.GetInt("totalComposure") / PlayerPrefs.GetInt("countComposure"))); } catch (Exception) { }
        try { averageIntelligenceChoiceText.SetText("Average intelligence choice: " + (PlayerPrefs.GetInt("totalIntelligence") / PlayerPrefs.GetInt("countIntelligence"))); } catch (Exception) { }
        try { oneHandedWeaponChoicesText.SetText("One handed weapon choice: " + (PlayerPrefs.GetInt("countOneHanded") / PlayerPrefs.GetInt("matchesPlayed") * 100) + "%"); } catch (Exception) { }
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

    public void StartGameButton()
    {
        if(onGameStart != null) onGameStart();
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
