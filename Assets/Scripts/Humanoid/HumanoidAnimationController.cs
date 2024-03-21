using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationController : MonoBehaviour
{
    Animator animator;
    public HumanoidMovementController movementController;

    string path;
    class PlayerStates
    {
        public List<string> playerStates = new List<string>();
    }

    PlayerStates states = new PlayerStates();
    private void Awake()
    {
        path = Application.dataPath + "/Scripts/Humanoid/States.json";
        animator = GetComponentInChildren<Animator>();
        movementController = transform.GetComponent<HumanoidMovementController>();

        LoadPlayerStates();
    }
    
    void LoadPlayerStates()
    {
        string json = File.ReadAllText(path);
        states = JsonUtility.FromJson<PlayerStates>(json);
    }
    void UpdateStates(string _currentState)
    {
        foreach(string state in states.playerStates)
        {
            if(state == _currentState)
            {
                animator.SetBool(state, true);
            }
            else
            {
                animator.SetBool(state, false);
            }
        }
    }

    private void OnEnable()
    {
        movementController.onChangedState += UpdateStates;
    }

    private void OnDisable()
    {
        movementController.onChangedState -= UpdateStates;
    }
}
