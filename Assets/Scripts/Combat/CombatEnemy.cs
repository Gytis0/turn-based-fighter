using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : CombatHumanoid
{
    public delegate void EnemyTurnEnd(Queue<Action> actions);
    public event EnemyTurnEnd onEnemyTurnEnd;

    NavMeshAgent agent;
    NavMeshPath path;
    GameObject playerObject;
    CombatPlayer player;

    Coroutine thinkingProcess;

    // Values to determine bot style
    float baseDefensiveFactor, baseRetreatFactor;
    float thinkingSpeed;

    private void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<CombatPlayer>();

        baseDefensiveFactor = Random.Range(0.1f, 0.2f);
        baseRetreatFactor = Random.Range(0.3f, 0.5f);
        thinkingSpeed = Random.Range(1.5f, 2.5f);
    }

    public override void ExecuteAction(Action action)
    {
        base.ExecuteAction(action);

    }

    public override void PromptAction(List<Action> availableActions)
    {
        this.availableActions = availableActions;
        if (actionQueue.Count == 0) thinkingProcess = StartCoroutine(GenerateAction());
        else EndTurn();
    }

    IEnumerator GenerateAction()
    {
        float composurePercentage = Mathf.Clamp((GetComposure() / GetMaxComposure()) * 2, 0, 1);
        float randomWaitTime = thinkingSpeed * Random.Range(1f, 1.4f) / composurePercentage;
        yield return new WaitForSeconds(randomWaitTime);

        Action action = GetAction(ActionName.Skip, availableActions);
        CombatState newState = CombatState.None;
        path.ClearCorners();

        // Actions if we're fallen
        if (combatStance == CombatStance.Fallen)
        {
            if(Vector3.Distance(transform.position, playerObject.transform.position) <= 2.5f)
            {
                List<Action> agileActions = new List<Action>();
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Agile && tempAction.actionName != ActionName.Get_Up) agileActions.Add(tempAction);
                if(agileActions.Count > 0) action = agileActions[Random.Range(0, availableActions.Count)];
            }
            else
            {
                int randomActionNumber = Random.Range(0, 1);
                if(randomActionNumber == 0 && IsActionAvailable(ActionName.Get_Up, availableActions))
                {
                    action = GetAction(ActionName.Get_Up, availableActions);
                }
            }
        }
        // If the player is far, move to it    
        else if (Vector3.Distance(transform.position, playerObject.transform.position) > 2.5f)
        {
            StartCoroutine(FindNextWaypoint());

            Vector3 waypoint = path.corners[1];
            Dictionary<Direction, Vector3> fourDirections = UtilityScripts.GetFourDirections(transform.position);
            List<Direction> availableDirections = new List<Direction>();
            if (Vector3.Distance(transform.position, playerObject.transform.position) > 10f)
            {
                availableDirections = GetAvailableDirections(ActionName.Run);
                if (availableDirections.Count > 0)
                {
                    action = GetAction(ActionName.Run, availableActions);
                }
                else
                {
                    availableDirections = GetAvailableDirections(ActionName.Step);
                    action = GetAction(ActionName.Step, availableActions);
                }
            }
            else
            {
                availableDirections = GetAvailableDirections(ActionName.Step);
                action = GetAction(ActionName.Step, availableActions);
            }

            if (availableDirections.Count > 0)
            {
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

            action = GetAction(actionName, availableActions);
            action.SetDirection(Direction.Backward);
        }
        // Else decide whether to be defensive or offensive or retreat
        else
        {
            
            float healthPercentage = humanoidProperties.GetHealth() / humanoidProperties.GetMaxHealth();
            float defensiveFactor = baseDefensiveFactor;
            float retreatFactor = baseRetreatFactor;
            if (GetShieldActions() != null)
            {
                AnimationStates playerCurrentAnimation = player.GetCurrentAnimationState();

                if (playerCurrentAnimation == AnimationStates.OVERHEAD || playerCurrentAnimation == AnimationStates.STABBING)
                {
                    newState = CombatState.Blocking;
                    defensiveFactor *= 4f;
                }
                else if (playerCurrentAnimation == AnimationStates.SWINGING_LEFT)
                {
                    newState = CombatState.Blocking_Right;
                    defensiveFactor *= 4f;
                }
                else if (playerCurrentAnimation == AnimationStates.SWINGING_RIGHT)
                {
                    newState = CombatState.Blocking_Left;
                    defensiveFactor *= 4f;
                }
            }
            else
            {
                AnimationStates playerCurrentAnimation = player.GetCurrentAnimationState();

                if (playerCurrentAnimation == AnimationStates.OVERHEAD || playerCurrentAnimation == AnimationStates.STABBING || playerCurrentAnimation == AnimationStates.SWINGING_LEFT || playerCurrentAnimation == AnimationStates.SWINGING_RIGHT)
                {
                    defensiveFactor *= 4f;
                    retreatFactor *= 3f;
                }
            }
            
            if (healthPercentage < 0.5f || humanoidProperties.GetHealth() <= 20)
            {
                defensiveFactor *= 1.2f;
            }

            composurePercentage = GetComposure() / GetMaxComposure();
            if (composurePercentage < 0.4f || humanoidProperties.GetComposure() <= fallOverThreshold + 0.2f)
            {
                retreatFactor *= 2f;
            }

            bool isDefensiveAction = Random.Range(0f, 1f) < defensiveFactor;
            bool isRetreatAction = Random.Range(0f, 1f)  < retreatFactor;

            List<Action> actionListOfType = new List<Action>();
            if (isDefensiveAction && isRetreatAction)
            {
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Movement || tempAction.actionType == ActionType.Agile) actionListOfType.Add(tempAction);
            }
            else if(isDefensiveAction)
            {
                if(newState != CombatState.None)
                {
                    actionListOfType.Add(GetAction(ActionName.Block, availableActions));
                }
                else
                {
                    foreach (Action tempAction in availableActions)
                        if (tempAction.actionType == ActionType.Defensive) actionListOfType.Add(tempAction);
                }
            }
            else
            {
                newState = CombatState.None;
                foreach (Action tempAction in availableActions)
                    if (tempAction.actionType == ActionType.Offensive) actionListOfType.Add(tempAction);
            }

            if(actionListOfType.Count == 0)
                action = GetAction(ActionName.Skip, availableActions);
            else
                action = actionListOfType[Random.Range(0, actionListOfType.Count)];
        }

        if(newState == CombatState.Blocking_Right) action.SetDirection(Direction.Right);
        else if(newState == CombatState.Blocking_Left) action.SetDirection(Direction.Left);
        else if(newState == CombatState.Blocking) action.SetDirection(Direction.Forward);
        else if(action.directions.Count > 1) action.SetDirection(action.directions[Random.Range(0, action.directions.Count)]);
        actionQueue.Enqueue(action);

        
        EndTurn();
    }

    IEnumerator FindNextWaypoint()
    {
        agent.stoppingDistance = gridSpacing;
        agent.CalculatePath(playerObject.transform.position, path);
        while (path.status != NavMeshPathStatus.PathComplete)
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
        else
        {
            Queue<Action> temp = new Queue<Action>(actionQueue);
            actionQueue.Clear();
            onEnemyTurnEnd(temp);
        }

    }

    public override void SkipTurn()
    {
        StopCoroutine(thinkingProcess);
        actionQueue.Enqueue(skipTurnAction);
        Queue<Action> temp = new Queue<Action>(actionQueue);
        actionQueue.Clear();
        onEnemyTurnEnd(temp);
    }

    public override void DenyAction()
    {
        base.DenyAction();
        StopCoroutine(thinkingProcess);
    }

    bool IsActionAvailable(ActionName actionName, List<Action> actions)
    {
        foreach (Action action in actions)
        {
            if (actionName == action.actionName) return true;
        }
        return false;
    }

    Action GetAction(ActionName actionName, List<Action> actions) 
    {
        foreach (Action action in actions)
        {
            if (actionName == action.actionName) return action;
        }
        return null;
    }
}
