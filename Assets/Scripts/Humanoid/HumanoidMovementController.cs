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

    public delegate void ChangedState(string currentState);
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

        humanoidStats.currentState = "Walking";
        if (onChangedState != null) onChangedState(humanoidStats.currentState);
    }

    protected virtual void Run(Vector3 _destination)
    {
        agent.isStopped = false;
        agent.SetDestination(_destination);
        agent.speed = runningSpeed;

        humanoidStats.currentState = "Running";
        if (onChangedState != null) onChangedState(humanoidStats.currentState);
    }

    public void Idle()
    {
        humanoidStats.currentState = "Idle";
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

    public void StartFollowing(Transform _obj, float _radius = 0f)
    {
        humanoidStats.followingObj = _obj;
        agent.stoppingDistance = _radius;
        humanoidStats.following = true;
    }

    protected virtual void StopFollowing()
    {
        humanoidStats.followingObj = null;
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
        if(humanoidStats.following)
        {
            Run(humanoidStats.followingObj.position);
        }

        if(humanoidProperties.GetStamina() <= 0 && humanoidStats.inCombat)
        {
            Stop();
        }

        if(agent.remainingDistance < agent.stoppingDistance + 0.1f && agent.remainingDistance > agent.stoppingDistance)
        {
            Stop();
        }
    }
}
