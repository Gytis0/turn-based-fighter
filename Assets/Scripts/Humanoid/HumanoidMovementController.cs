using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class HumanoidMovementController : MonoBehaviour
{
    protected NavMeshAgent agent;

    HumanoidAnimationController animationController;
    protected HumanoidProperties humanoidProperties;
    protected HumanoidStats humanoidStats;

    public delegate void ChangedState(AnimationStates newState);
    public event ChangedState onChangedState;

    [SerializeField]
    float walkingSpeed = 1f;
    [SerializeField]
    float runningSpeed = 10f;

    protected delegate void Followed();
    protected event Followed OnFollowed;

    protected virtual void Awake()
    {
        animationController = transform.GetComponent<HumanoidAnimationController>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
        humanoidStats = transform.GetComponent<HumanoidStats>();
        agent = transform.GetComponent<NavMeshAgent>();
    }

    protected virtual void Walk(Vector3 _destination)
    {
        agent.isStopped = false;
        agent.SetDestination(_destination);
        agent.speed = walkingSpeed;

        humanoidStats.currentState = AnimationStates.WALKING;
        if (onChangedState != null) onChangedState(humanoidStats.currentState);
    }

    protected virtual void Run(Vector3 _destination)
    {
        agent.isStopped = false;
        agent.SetDestination(_destination);
        agent.speed = runningSpeed;

        humanoidStats.currentState = AnimationStates.RUNNING;
        if (onChangedState != null) onChangedState(humanoidStats.currentState);
    }

    public void Idle()
    {
        humanoidStats.currentState = AnimationStates.IDLE;
        if (onChangedState != null) onChangedState(humanoidStats.currentState);
    }

    public void Stop()
    {
        if (humanoidStats.following)
        {
            FinishedFollowing();
        }
        agent.isStopped = true;
        Idle();
        StopFollowing();
    }

    public void Reach(Vector3 destination, float _radius = 0f)
    {
        humanoidStats.destination = destination;
        agent.stoppingDistance = _radius;
        humanoidStats.following = true;
    }

    protected virtual void StopFollowing()
    {
        humanoidStats.destination = Vector3.zero;
        agent.stoppingDistance = 0f;
        humanoidStats.following = false;
    }

    void FinishedFollowing()
    {
        if (OnFollowed != null)
        {
            OnFollowed();
        }
    }

    protected virtual void Update()
    {
        
        
        if (humanoidStats.following)
        {
            Run(humanoidStats.destination);
        }

        // Stop moving if there is no path generating
        // Stop moving if we reached the destination
        if ((!agent.hasPath && !agent.pathPending) || (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending))
        {
            Stop();
        }
    }
}
