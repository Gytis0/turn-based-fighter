using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : CombatHumanoid
{
    public delegate void EnemyTurnEnd(Action action);
    public event EnemyTurnEnd onEnemyTurnEnd;

    NavMeshAgent agent;
    NavMeshPath path;
    GameObject playerObject;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public override void ExecuteAction(Action action)
    {
        base.ExecuteAction(action);

    }

    public override void PromptAction(List<Action> availableActions)
    {
        this.availableActions = availableActions;
        if (actionQueue.Count == 0) GenerateAction();
        else EndTurn();
    }

    void GenerateAction()
    {
        Action action;
        path.ClearCorners();

        // If player is far, move to it    
        if (Vector3.Distance(transform.position, playerObject.transform.position) > 2.5f)
        {
           FindNextWaypoint();

            Vector3 waypoint = path.corners[0];
            Vector3[] directions = GetDirections();
            float shortestDistance = 99f;

            foreach (Vector3 direction in directions)
            {

            }
        }

        actionQueue.Enqueue(action);
        EndTurn();
    }

    IEnumerator FindNextWaypoint()
    {
        agent.CalculatePath(playerObject.transform.position, path);

        while(path.status != NavMeshPathStatus.PathComplete)
        {
            yield return new WaitForSeconds(1);
        }
    }

    Vector3[] GetDirections()
    {
        Vector3[] result = new Vector3[4];
        result[0] = transform.position + Vector3.forward;
        result[1] = transform.position + Vector3.back;
        result[2] = transform.position + Vector3.left;
        result[3] = transform.position + Vector3.right;
        return result;
    }

    public override void EndTurn()
    {
        if (actionQueue.Count == 0) SkipTurn();
        else onEnemyTurnEnd(actionQueue.Dequeue());
    }

    public override void SkipTurn()
    {
        onEnemyTurnEnd(skipTurnAction);
    }
}
