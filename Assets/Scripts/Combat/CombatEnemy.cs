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

    // Values to determine bot style
    float baseDefensiveFactor, baseRetreatFactor, baseWildcardFactor;

    private void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        playerObject = GameObject.FindGameObjectWithTag("Player");

        baseDefensiveFactor = Random.Range(0.1f, 0.3f);
        baseRetreatFactor = Random.Range(0.2f, 0.4f);
        baseWildcardFactor = Random.Range(0.01f, 0.1f);
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
        Action action = new Action(ActionName.Skip, ActionType.Skip);
        path.ClearCorners();

        // If player is far, move to it    
        if (Vector3.Distance(transform.position, playerObject.transform.position) > 2.5f)
        {
            action = new Action(ActionName.Step, ActionType.Movement);
            StartCoroutine(FindNextWaypoint());

            Vector3 waypoint = path.corners[1];
            Dictionary<Direction, Vector3> fourDirections = GetFourDirections();
            List<Direction> availableDirections = GetAvailableDirections(ActionName.Step);
            float shortestDistance = 99f;

            foreach (Direction direction in availableDirections)
            {
                float distance = Vector3.Distance(fourDirections[direction], waypoint);
                if (distance > shortestDistance) continue;
                else shortestDistance = distance;
                action.SetDirection(direction);
            }
        }
        // Else decide whether to be defensive or offensive or retreat
        else
        {
            float healthPercentage = humanoidProperties.GetHealth() / humanoidProperties.GetMaxHealth();
            float defensiveFactor = baseDefensiveFactor;
            if(healthPercentage < 0.5f || humanoidProperties.GetHealth() <= 20)
            {
                defensiveFactor *= 1.5f;
            }

            float composurePercentage = humanoidProperties.GetComposure() / humanoidProperties.GetMaxComposure();
            float retreatFactor = baseRetreatFactor;
            if (composurePercentage < 0.4f || humanoidProperties.GetComposure() <= fallOverThreshold)
            {
                retreatFactor *= 2f;
            }

            bool isDefensiveAction = Random.Range(0f, 1f) < defensiveFactor;
            bool isRetreatAction = Random.Range(0f, 1f)  < retreatFactor;

            Debug.Log("(isDefensiveAction / isRetreatAction): " + isDefensiveAction + " / " + isRetreatAction);

            List<Action> actionListOfType = new List<Action>();
            if (isDefensiveAction && isRetreatAction)
            {
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Movement || tempAction.actionType == ActionType.Agile) actionListOfType.Add(tempAction);
            }
            else if(isDefensiveAction)
            {
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Defensive) actionListOfType.Add(tempAction);
            }
            else
            {
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Offensive) actionListOfType.Add(tempAction);
            }

            action = actionListOfType[Random.Range(0, actionListOfType.Count)];
            action.SetDirection(action.directions[Random.Range(0, action.directions.Count)]);
        }

        actionQueue.Enqueue(action);
        EndTurn();
    }

    IEnumerator FindNextWaypoint()
    {
        agent.stoppingDistance = gridSpacing;
        agent.CalculatePath(playerObject.transform.position, path);
        while(path.status != NavMeshPathStatus.PathComplete)
        {
            yield return new WaitForSeconds(1);
        }
    }

    Dictionary<Direction, Vector3> GetFourDirections()
    {
        return new Dictionary<Direction, Vector3>
        {
            { Direction.Forward, transform.position + Vector3.forward },
            { Direction.Backward, transform.position + Vector3.back },
            { Direction.Left, transform.position + Vector3.left },
            { Direction.Right, transform.position + Vector3.right }
        };
    }

    List<Direction> GetAvailableDirections(ActionName actionName)
    {
        List<Direction> result = new List<Direction>();
        foreach(Action action in availableActions)
        {
            if(action.actionName == actionName)
            {
                foreach(Direction direction in action.directions)
                {
                    result.Add(direction);
                }
                break;
            }
        }
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
