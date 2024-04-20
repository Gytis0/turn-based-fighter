using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Humanoid : MonoBehaviour
{
    [SerializeField] List<Button> movementButtons = new();
    [SerializeField] List<Button> agileButtons = new();
    [SerializeField] List<Button> offenseButtons = new();
    [SerializeField] List<Button> defenseButtons = new();

    HumanoidMovementController humanoidMovement;
    HumanoidProperties humanoidProperties;
    public bool inCombat = false;

    Queue<Action> actionQueue = new Queue<Action>();

    void Start()
    {
        humanoidMovement = transform.GetComponent<HumanoidMovementController>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
    }

    public void EnableCombatMode(bool _enable)
    {
        inCombat = _enable;
    }

    private void Update()
    {

    }

    public void ExecuteAction(Action action)
    {
        
    }

    void AddActionsToQueue(List<Action> actions)
    {

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
