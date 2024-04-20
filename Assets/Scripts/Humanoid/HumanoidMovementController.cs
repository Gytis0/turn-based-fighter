using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class HumanoidMovementController : MonoBehaviour
{
    protected NavMeshAgent agent;

    protected HumanoidProperties humanoidProperties;

    public delegate void ChangedState(AnimationStates newState);
    public event ChangedState onChangedState;

    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] float runningSpeed = 10f;

    public AnimationStates animationState = AnimationStates.IDLE;
    public Vector3 destination;
    public bool isFollowing = false;

    protected delegate void Followed();
    protected event Followed OnFollowed;

    protected virtual void Awake()
    {
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
        agent = transform.GetComponent<NavMeshAgent>();
    }

    protected virtual void Walk(Vector3 _destination)
    {
        agent.isStopped = false;
        agent.SetDestination(_destination);
        agent.speed = walkingSpeed;

        animationState = AnimationStates.WALKING;
        if (onChangedState != null) onChangedState(animationState);
    }

    protected virtual void Run(Vector3 _destination)
    {
        agent.isStopped = false;
        agent.SetDestination(_destination);
        agent.speed = runningSpeed;

        animationState = AnimationStates.RUNNING;
        if (onChangedState != null) onChangedState(animationState);
    }

    public void Idle()
    {
        animationState = AnimationStates.IDLE;
        if (onChangedState != null) onChangedState(animationState);
    }

    public void Stop()
    {
        if (isFollowing)
        {
            FinishedFollowing();
        }
        agent.isStopped = true;
        Idle();
        StopFollowing();
    }

    public void Reach(Vector3 destination, float _radius = 0f)
    {
        destination = destination;
        agent.stoppingDistance = _radius;
        isFollowing = true;
    }

    protected virtual void StopFollowing()
    {
        destination = Vector3.zero;
        agent.stoppingDistance = 0f;
        isFollowing = false;
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
        if (isFollowing)
        {
            Run(destination);
        }

        // Stop moving if there is no path generating
        // Stop moving if we reached the destination
        if ((!agent.hasPath && !agent.pathPending) || (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending))
        {
            Stop();
        }
    }
}
