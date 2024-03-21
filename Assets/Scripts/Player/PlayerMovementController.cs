using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerMovementController : HumanoidMovementController
{
    InputController inputActions;

    public delegate void PlayerReachedFollower();
    public static event PlayerReachedFollower onPlayerReachedFollower;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputController();
    }

    public void StartWalking(Vector3 _destination)
    {
        StopFollowing();
        Walk(_destination);
    }

    public void StartRunning(Vector3 _destination)
    {
        StopFollowing();
        Run(_destination);
    }

    void TriggerFollowed()
    {
        if(onPlayerReachedFollower != null)
        {
            onPlayerReachedFollower();
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        OnFollowed += TriggerFollowed;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        OnFollowed -= TriggerFollowed;
    }


}
