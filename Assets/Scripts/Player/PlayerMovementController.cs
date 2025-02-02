using UnityEngine;

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
        StopReaching();
        Walk(_destination);
    }

    public void StartRunning(Vector3 _destination)
    {
        StopReaching();
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
