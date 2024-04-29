using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class HumanoidMovementController : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected HumanoidProperties humanoidProperties;
    protected HumanoidAnimationController animationController;

    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] float runningSpeed = 10f;

    public Vector3 destination;
    public bool isFollowing = false;
    bool isFallen = false;
    public bool inCombat = false;
    bool moving = false;

    protected delegate void Followed();
    protected event Followed OnFollowed;

    protected virtual void Awake()
    {
        humanoidProperties = GetComponent<HumanoidProperties>();
        animationController = GetComponent<HumanoidAnimationController>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Walk(Vector3 _destination)
    {
        moving = false;
        agent.SetDestination(_destination);
        agent.speed = walkingSpeed;

        animationController.SetState(AnimationStates.WALKING);
    }

    protected virtual void Run(Vector3 _destination)
    {
        moving = false;
        agent.SetDestination(_destination);
        agent.speed = runningSpeed;

        animationController.SetState(AnimationStates.RUNNING);
    }

    public void FaceEnemy()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            if (transform.tag == "Enemy") enemy = GameObject.FindGameObjectWithTag("Player");
        }

        Quaternion _lookRotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
        _lookRotation.Set(0f, _lookRotation.y, 0f, _lookRotation.w);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 5);
    }

    public void Idle()
    {
        animationController.SetState(AnimationStates.IDLE);
    }

    public void Stop()
    {
        if (isFollowing)
        {
            FinishedFollowing();
        }
        moving = false;
        Idle();
        StopFollowing();
    }

    public void Reach(Vector3 destination, float _radius = 0f)
    {
        this.destination = destination;
        agent.stoppingDistance = _radius;
        moving = true;
        isFollowing = true;
    }

    public void Move(Vector3 destination, float speed, AnimationStates animationState, float angularSpeed = 800f, bool isFallen = false)
    {
        animationController.SetState(animationState);
        moving = true;
        agent.SetDestination(destination);
        agent.speed = speed;
        agent.stoppingDistance = 0;
        agent.angularSpeed = angularSpeed;
        this.isFallen = isFallen;
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

    protected void Update()
    {
        // Face the enemy whenever we're idling
        if (inCombat && !moving)
        {
            FaceEnemy();
        }

        if (animationController.GetCurrentAnimationName().ToLower().Contains("fallen_idle")) moving = false;

        if (isFallen)
        {
            return;
        }

        if (isFollowing && animationController.GetCurrentAnimationState() != AnimationStates.RUNNING)
        {
            Run(destination);
        }

        // Stop moving if there is no path generating
        // Stop moving if we reached the destination
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && moving)
        {
            Stop();
        }

        
    }
}
