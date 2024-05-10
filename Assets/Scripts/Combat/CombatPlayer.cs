using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPlayer : CombatHumanoid
{
    public delegate void PlayerTurnEnd(Queue<Action> actions);
    public event PlayerTurnEnd onPlayerTurnEnd;

    List<Action> movementActions, agileActions, offenseActions, defenseActions;
    [SerializeField] List<Button> allButtons = new();
    [SerializeField] List<Button> directionButtons = new();
    [SerializeField] List<Button> movementButtons = new();
    [SerializeField] List<Button> agileButtons = new();
    [SerializeField] List<Button> offenseButtons = new();
    [SerializeField] List<Button> defenseButtons = new();
    [SerializeField] GameObject combatUI;

    [SerializeField] List<TextMeshProUGUI> bubblesTexts;
    [SerializeField] List<Image> bubblesIndicators;
    [SerializeField] Button resetQueueButton;
    [SerializeField] Button endTurnButton;
    [SerializeField] Button skipTurnButton;
    [SerializeField] Button addActionButton;

    Action selectedAction;
    Direction selectedDirection;

    [SerializeField] Color notSelectedButton;
    [SerializeField] Color selectedButton;

    void Start()
    {
        base.Start();
        combatUI.SetActive(false);
        movementActions = new List<Action>();
        agileActions = new List<Action>();
        offenseActions = new List<Action>();
        defenseActions = new List<Action>();
        DisableButtons();
    }

    public override void EnableCombatMode(bool _enable, float gridSpacing, Tuple<Vector2, Vector2> gridBoundaries)
    {
        base.EnableCombatMode(_enable, gridSpacing, gridBoundaries);
        combatUI.SetActive(_enable);
        if (!_enable) DisableButtons();
    }

    public override void ExecuteAction(Action action)
    {
        base.ExecuteAction(action);
    }

    public override void PromptAction(List<Action> availableActions)
    {
        if (actionQueue.Count == 0) EnableButtons(availableActions);
        else EndTurn();
    }

    public override void DenyAction()
    {
        DisableButtons();

    }

    void EnableButtons(List<Action> availableActions)
    {
        ClearActionsLists();
        UpdateBubbles();
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

        resetQueueButton.interactable = true;
        endTurnButton.interactable = true;
        skipTurnButton.interactable = true;
        addActionButton.interactable = true;

    }

    void DisableButtons()
    {
        foreach (Button button in allButtons)
        {
            button.transform.parent.GetComponent<Image>().color = notSelectedButton;
            button.transform.parent.gameObject.SetActive(false);
        }

        resetQueueButton.interactable = false;
        endTurnButton.interactable = false;
        skipTurnButton.interactable = false;
        addActionButton.interactable = false;
        ClearActionsLists();
    }

    void ShowActionDirections()
    {
        foreach (Button button in directionButtons)
        {
            button.transform.parent.GetComponent<Image>().color = notSelectedButton;
            button.transform.parent.gameObject.SetActive(false);
        }

        foreach (Direction direction in selectedAction.directions)
        {
            if (direction == Direction.Forward) directionButtons[0].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Left) directionButtons[1].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Right) directionButtons[2].transform.parent.gameObject.SetActive(true);
            else if (direction == Direction.Backward) directionButtons[3].transform.parent.gameObject.SetActive(true);
        }
    }

    void HideActionDirections()
    {
        foreach (Button button in directionButtons)
        {
            button.transform.parent.GetComponent<Image>().color = notSelectedButton;
            button.transform.parent.gameObject.SetActive(false);
        }
    }

    public void SelectAction(int buttonId)
    {
        DemarkEverything();

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

    public void SelectDirection(int direction)
    {
        foreach (Button button in directionButtons)
        {
            button.transform.parent.GetComponent<Image>().color = notSelectedButton;
        }
        direction++;
        selectedDirection = (Direction)direction;
    }

    void UpdateBubbles()
    {
        List<Action> actions = new List<Action>();
        foreach(Action action in actionQueue)
        {
            actions.Add(action);
        }

        for(int i = 0; i < bubblesTexts.Count; i++)
        {
            if (actions.Count <= i)
            {
                bubblesTexts[i].SetText("");
                bubblesIndicators[i].enabled = false;
                bubblesTexts[i].transform.GetComponentInParent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                bubblesTexts[i].SetText(actions[i].actionName.ToString());
                bubblesIndicators[i].enabled = true;
                if (actions[i].directions[0] == Direction.None) { bubblesIndicators[i].enabled = false; }
                else if (actions[i].directions[0] == Direction.Left) { bubblesIndicators[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
                else if (actions[i].directions[0] == Direction.Backward) { bubblesIndicators[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90)); }
                else if (actions[i].directions[0] == Direction.Right) { bubblesIndicators[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180)); }
                else if (actions[i].directions[0] == Direction.Forward) { bubblesIndicators[i].rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 270)); }
                bubblesTexts[i].transform.GetComponentInParent<Image>().color = new Color(1, 1, 1, 1f);
            }
        }
    }

    void DemarkEverything()
    {
        foreach (Button button in allButtons)
        {
            button.transform.parent.GetComponent<Image>().color = notSelectedButton;
        }
    }
  
    void ClearActionsLists()
    {
        movementActions.Clear();
        agileActions.Clear();
        offenseActions.Clear();
        defenseActions.Clear();
    }

    // Four buttons

    public void AddAction()
    {
        if (selectedAction == null || (selectedAction.directions.Count != 0 && selectedAction.directions[0] != Direction.None && selectedDirection == 0)) return;

        Action newAction = new Action(selectedAction, selectedDirection);
        actionQueue.Enqueue(newAction);

        selectedAction = null;
        selectedDirection = 0;

        HideActionDirections();
        UpdateBubbles();
        DemarkEverything();
    }

    public override void EndTurn()
    {
        selectedAction = null;
        selectedDirection = 0;
        DemarkEverything();

        DisableButtons();

        if (actionQueue.Count == 0)
        {
            SkipTurn();
        }
        else
        {
            Queue<Action> temp = new Queue<Action>(actionQueue);
            actionQueue.Clear();

            onPlayerTurnEnd(temp);
            UpdateBubbles();
        }
    }

    public override void SkipTurn()
    {
        selectedAction = null;
        selectedDirection = 0;
        DemarkEverything();

        DisableButtons();
        actionQueue.Enqueue(skipTurnAction);
        Queue<Action> temp = new Queue<Action>(actionQueue);
        actionQueue.Clear();
        onPlayerTurnEnd(temp);
    }

    public void ResetQueue()
    {
        selectedAction = null;
        selectedDirection = 0;
        DemarkEverything();

        actionQueue.Clear();
        UpdateBubbles();
    }

}
