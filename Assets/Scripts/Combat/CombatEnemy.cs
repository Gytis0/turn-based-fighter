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
        foreach (Action actionTemp in availableActions)
        {
            if (actionTemp.actionName == ActionName.Skip)
            {
                action = actionTemp;
                break;
            }
        }
        path.ClearCorners();

        // Actions if we're fallen
        if (combatStance == CombatStance.Fallen)
        {
            if(Vector3.Distance(transform.position, playerObject.transform.position) <= 2.5f)
            {
                List<Action> agileActions = new List<Action>();
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Agile && tempAction.actionName != ActionName.Get_Up) agileActions.Add(tempAction);
                action = agileActions[Random.Range(0, availableActions.Count)];
            }
            else
            {
                int randomActionNumber = Random.Range(0, 1);
                if(randomActionNumber == 0)
                {
                    action = new Action(ActionName.Get_Up, ActionType.Agile);
                }
                else
                {
                    action = new Action(ActionName.Skip, ActionType.Skip);
                }
            }
        }
        // If player is far, move to it    
        else if (Vector3.Distance(transform.position, playerObject.transform.position) > 2.5f)
        {
            StartCoroutine(FindNextWaypoint());

            Vector3 waypoint = path.corners[1];
            Dictionary<Direction, Vector3> fourDirections = UtilityScripts.GetFourDirections(transform.position);
            List<Direction> availableDirections = GetAvailableDirections(ActionName.Step);
            if(availableDirections.Count > 0)
            {
                action = new Action(ActionName.Step, ActionType.Movement);

                float shortestDistance = 99f;

                foreach (Direction direction in availableDirections)
                {
                    float distance = Vector3.Distance(fourDirections[direction], waypoint);
                    if (distance > shortestDistance) continue;
                    shortestDistance = distance;
                    action.SetDirection(direction);
                }
            }
        }
        else if(playerObject.GetComponent<CombatPlayer>().GetCombatStance() == CombatStance.Fallen)
        {
            ActionName actionName;
            if (equipment.GetEquippedWeaponData().GetWeaponType() == WeaponType.OneHanded)
                actionName = ActionName.Stab;
            else
                actionName = ActionName.Overhead;
            foreach (Action actionTemp in availableActions)
            {
                if (actionTemp.actionName == actionName)
                {
                    action = actionTemp;
                    action.SetDirection(Direction.Backward);
                    break;
                }
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

            if(actionListOfType.Count == 0)
                action = new Action(ActionName.Skip, ActionType.Skip);
            else
                action = actionListOfType[Random.Range(0, actionListOfType.Count)];
        }

        Debug.Log("Bot decided on action: " + action.actionName);
        if(action.directions.Count > 0) action.SetDirection(action.directions[Random.Range(0, action.directions.Count)]);
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
