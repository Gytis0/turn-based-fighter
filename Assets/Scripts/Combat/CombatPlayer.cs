using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPlayer : CombatHumanoid
{
    List<Action> movementActions, agileActions, offenseActions, defenseActions;
    [SerializeField] List<Button> directionButtons = new();
    [SerializeField] List<Button> movementButtons = new();
    [SerializeField] List<Button> agileButtons = new();
    [SerializeField] List<Button> offenseButtons = new();
    [SerializeField] List<Button> defenseButtons = new();
    [SerializeField] GameObject combatUI;

    [SerializeField] List<TextMeshProUGUI> bubblesTexts;
    [SerializeField] Button resetQueue;
    [SerializeField] Button endTurn;
    [SerializeField] Button skipTurn;

    Action selectedAction;
    Direction selectedDirection;

    

    void Start()
    {
        base.Start();
        combatUI.SetActive(false);
        movementActions = new List<Action>();
        agileActions = new List<Action>();
        offenseActions = new List<Action>();
        defenseActions = new List<Action>();
    }

    public override void EnableCombatMode(bool _enable)
    {
        base.EnableCombatMode(_enable);
        combatUI.SetActive(inCombat);
        DisableButtons();
    }

    public void EnableButtons(List<Action> availableActions)
    {
        this.availableActions = availableActions;
        int index;
        // Enable buttons based on available actions
        for(int i = 0; i < availableActions.Count; i++)
        {
            if (availableActions[i].actionType == ActionType.Movement)
            {
                index = movementActions.Count;
                movementButtons[index].transform.parent.gameObject.SetActive(true);
                movementButtons[index].GetComponentInChildren<TextMeshProUGUI>().SetText(availableActions[i].actionName.ToString());
                movementActions.Add(availableActions[i]);
            }
            else if (availableActions[i].actionType == ActionType.Agile)
            {
                index = agileActions.Count;
                agileButtons[index].transform.parent.gameObject.SetActive(true);
                agileButtons[index].GetComponentInChildren<TextMeshProUGUI>().SetText(availableActions[i].actionName.ToString());
                agileActions.Add(availableActions[i]);
            }
            else if (availableActions[i].actionType == ActionType.Defensive)
            {
                index = defenseActions.Count;
                defenseButtons[index].transform.parent.gameObject.SetActive(true);
                defenseButtons[index].GetComponentInChildren<TextMeshProUGUI>().SetText(availableActions[i].actionName.ToString());
                defenseActions.Add(availableActions[i]);
            }
            else if (availableActions[i].actionType == ActionType.Offensive)
            {
                index = offenseActions.Count;
                offenseButtons[index].transform.parent.gameObject.SetActive(true);
                offenseButtons[index].GetComponentInChildren<TextMeshProUGUI>().SetText(availableActions[i].actionName.ToString());
                offenseActions.Add(availableActions[i]);
            }
        }
    }

    void DisableButtons()
    {
        foreach (Button button in agileButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
        foreach (Button button in offenseButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
        foreach (Button button in defenseButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
        foreach (Button button in directionButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
        foreach (Button button in movementButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
    }

    void ShowActionDirections()
    {
        foreach(Direction direction in selectedAction.directions)
        {
            if (direction == Direction.Forward) directionButtons[0].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Left) directionButtons[1].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Right) directionButtons[2].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Backward) directionButtons[3].transform.parent.gameObject.SetActive(true);
        }
    }

    public void SelectAction(int buttonId)
    {
        foreach (Button button in directionButtons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }

        if (buttonId == 0) selectedAction = movementActions[0];
        else if(buttonId == 1) selectedAction = movementActions[1];
        else if(buttonId == 2) selectedAction = movementActions[2];
        else if(buttonId == 3) selectedAction = movementActions[3];
        else if(buttonId == 10) selectedAction = agileActions[0];
        else if(buttonId == 11) selectedAction = agileActions[1];
        else if(buttonId == 12) selectedAction = agileActions[2];
        else if(buttonId == 13) selectedAction = agileActions[3];
        else if(buttonId == 20) selectedAction = offenseActions[0];
        else if(buttonId == 21) selectedAction = offenseActions[1];
        else if(buttonId == 22) selectedAction = offenseActions[2];
        else if(buttonId == 23) selectedAction = offenseActions[3];
        else if(buttonId == 30) selectedAction = defenseActions[0];
        else if(buttonId == 31) selectedAction = defenseActions[1];
        else if(buttonId == 32) selectedAction = defenseActions[2];
        else if(buttonId == 33) selectedAction = defenseActions[3];

        ShowActionDirections();
    }
}
