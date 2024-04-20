using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPlayer : CombatHumanoid
{
    [SerializeField] List<Button> directionButtons = new();
    [SerializeField] List<Button> movementButtons = new();
    [SerializeField] List<Button> agileButtons = new();
    [SerializeField] List<Button> offenseButtons = new();
    [SerializeField] List<Button> defenseButtons = new();
    [SerializeField] GameObject combatUI;

    void Start()
    {
        base.Start();
        combatUI.SetActive(false);
    }

    public override void EnableCombatMode(bool _enable)
    {
        base.EnableCombatMode(_enable);
        combatUI.SetActive(inCombat);
    }

    void EnableButtons(List<Action> availableActions)
    {
        // Enable buttons based on available actions

    }

    void DisableButtons()
    {
        foreach (Button button in agileButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in offenseButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in defenseButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
}
