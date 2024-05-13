using UnityEngine;
using System.Collections.Generic;
using System;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public delegate void GameEnd(bool isPlayerWinner, HumanoidProperties playerProperties, HumanoidProperties enemyProperties);
    public event GameEnd onCombatEnd;

    bool inCombat = false;
    bool isPlayersTurn  = true;
    GameObject playerObject, enemyObject;
    CombatPlayer player;
    CombatEnemy enemy;
    Equipment playerEquipment, enemyEquipment;
    Timer timer;

    Queue<Action> playerActionsQueue = new Queue<Action>();
    Queue<Action> enemyActionsQueue = new Queue<Action>();

    
    // When the action will be done, what action is being executed
    Tuple<float, Action> lastPlayerAction;
    Tuple<float, Action> lastEnemyAction;

    List<Tuple<float, Action>> playerTimeline = new List<Tuple<float, Action>>();
    List<Tuple<float, Action>> enemyTimeline = new List<Tuple<float, Action>>();

    bool isPlayerInAction;
    bool isEnemyInAction;

    float combatTotalTime = 0f;
    float freeTime = 10f;

    // Grid
    Tuple<Vector2, Vector2> gridBoundaries;
    float gridSpacing;


    // References
    [SerializeField] UnityEngine.UI.Image indicator;
    [SerializeField] List<Action> actionList;
    [SerializeField] List<ActionCombination> combinationList;
    [SerializeField] GameObject combatObjects;

    [SerializeField] List <UnityEngine.UI.Image> images = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
        timer = transform.GetComponent<Timer>();
        timer.onTimerDone += ForcefullyEndTurn;

        EnableUi(false);
    }

    private void Start()
    {
        gridBoundaries = ObjectGrid.Instance.GetGridBoundaries();
        gridSpacing = ObjectGrid.Instance.GetObjectSpacing();
    }

    public void StartCombat()
    {
        combatTotalTime = 0f;
        playerTimeline.Add(new Tuple<float, Action>(0f, new Action(ActionName.Skip, ActionType.Skip)));
        enemyTimeline.Add(new Tuple<float, Action>(0f, new Action(ActionName.Skip, ActionType.Skip)));
        inCombat = true;
        EnableUi(true);
        UpdateIndicator();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        enemyObject = GameObject.FindGameObjectWithTag("Enemy");

        player = playerObject.GetComponent<CombatPlayer>();
        enemy = enemyObject.GetComponent<CombatEnemy>();

        playerEquipment = playerObject.GetComponent<Equipment>();
        enemyEquipment = enemyObject.GetComponent<Equipment>();

        player.onPlayerTurnEnd += AddActionToPlayerQueue;
        playerEquipment.GetEquippedWeaponObject().GetComponent<OffensiveTrigger>().onEnemyHit += ApproveAttack;
        player.GetLegTrigger().onEnemyHit += ApproveAttack;
        player.onDeath += StopCombat;
        player.onFallen += PromptActionWhenFallen;

        enemy.onEnemyTurnEnd += AddActionToEnemyQueue;
        enemyEquipment.GetEquippedWeaponObject().GetComponent<OffensiveTrigger>().onEnemyHit += ApproveAttack;
        enemy.GetLegTrigger().onEnemyHit += ApproveAttack;
        enemy.onDeath += StopCombat;
        enemy.onFallen += PromptActionWhenFallen;

        player.EnableCombatMode(true, gridSpacing, gridBoundaries);
        enemy.EnableCombatMode(true, gridSpacing, gridBoundaries);

        player.SetCombinations(combinationList);
        enemy.SetCombinations(combinationList);

        player.PromptAction(GetAvailableActions(player));

        timer.EnableTimer(60f);
    }

    public void StopCombat(CombatHumanoid loser)
    {
        inCombat = false;
        EnableUi(false);

        enemy.EnableCombatMode(false, gridSpacing, gridBoundaries);
        player.EnableCombatMode(false, gridSpacing, gridBoundaries);

        bool isPlayerWinner;
        HumanoidProperties playerProperties = player.GetComponent<HumanoidProperties>();
        HumanoidProperties enemyProperties = enemy.GetComponent<HumanoidProperties>();
        isPlayerWinner = loser.GetType() == typeof(CombatEnemy);

        onCombatEnd(isPlayerWinner, playerProperties, enemyProperties);
    }

    void ForcefullyEndTurn()
    {
        Debug.Log("Forcefully ending turn [" + isPlayersTurn + "]");
        if (isPlayersTurn) player.EndTurn();
        else enemy.EndTurn();
    }

    void AddActionToPlayerQueue(Queue<Action> actions)
    {
        if (!isPlayersTurn) return;
        
        Debug.Log("Adding player actions");
        foreach (Action action in actions)
        {
            playerActionsQueue.Enqueue(action);
        }
        Update();
        SwitchTurn();
    }

    void AddActionToEnemyQueue(Queue<Action> actions)
    {
        if (isPlayersTurn) return;

        Debug.Log("Adding enemy actions");
        foreach (Action action in actions)
        {
            enemyActionsQueue.Enqueue(action);
        }
        Update();
        SwitchTurn();
    }

    void ApproveAction(Action action, CombatHumanoid character)
    {
        
        bool approved = false;
        List<Action> tempActionList = GetAvailableActions(character);
        foreach (Action availableAction in tempActionList)
        {
            if (action.actionName == ActionName.Skip || (availableAction.actionName == action.actionName && (action.directions[0] == Direction.None || availableAction.directions.Contains(action.directions[0]))))
            {
                approved = true;
                break;
            }
        }

        if (!approved)
        {
            DenyAction(character);
            character.PromptAction(tempActionList);
            Debug.Log("Denied " + action.actionName + " for " + character.name);
        }

        if (approved)
        {
            Debug.Log("Approved " + action.actionName + " for " + character.name);
            if (action.actionType == ActionType.Offensive)
            {
                action.duration = (action.baseDuration - (character.GetWeaponSpeed() / 3));
            }
            else if(action.actionType == ActionType.Movement || action.actionType == ActionType.Agile)
            {
                action.duration = action.baseDuration * (character.GetAllWeight() / 100 + 1) ;
            }
            action.duration *= 1 - (Mathf.Clamp(character.GetComposure() / character.GetMaxComposure() * 4, 0.01f, 1f)) + 1;

            if(action.actionType == ActionType.Skip)
            {
                action.duration = action.baseDuration;
            }

            character.ExecuteAction(action);
            if (character.GetType() == typeof(CombatPlayer))
            {
                if (action.actionType != ActionType.Skip)
                {
                    lastPlayerAction = new Tuple<float, Action>(combatTotalTime + action.duration, action);
                    playerTimeline.Add(lastPlayerAction);
                }
            }
            else
            {
                if (action.actionType != ActionType.Skip)
                {
                    lastEnemyAction = new Tuple<float, Action>(combatTotalTime + action.duration, action);
                    enemyTimeline.Add(lastEnemyAction);
                }
            }
        }
    }

    void ApproveAttack(Weapon weapon, Action action, bool isTargetPlayer)
    {
        CombatHumanoid targetCombatHumanoid;
        Equipment targetEquipment;
        if (isTargetPlayer)
        {
            targetCombatHumanoid = player;
            targetEquipment = playerEquipment;
        }
        else
        {
            targetCombatHumanoid = enemy;
            targetEquipment = enemyEquipment;
        }

        // Check if the block is successful
        if (targetCombatHumanoid.GetCombatState() == CombatState.Blocking && action.actionName == ActionName.Overhead) targetEquipment.DamageShield(weapon);
        else if ((targetCombatHumanoid.GetCombatState() == CombatState.Blocking) && action.actionName == ActionName.Stab) targetEquipment.DamageShield(weapon);
        else if (targetCombatHumanoid.GetCombatState() == CombatState.Blocking_Left && action.actionName == ActionName.Swing && action.directions[0] == Direction.Right) targetEquipment.DamageShield(weapon);
        else if (targetCombatHumanoid.GetCombatState() == CombatState.Blocking_Right && action.actionName == ActionName.Swing && action.directions[0] == Direction.Left) targetEquipment.DamageShield(weapon);
        // Check if the duck was successful
        else if (targetCombatHumanoid.GetCombatState() == CombatState.Ducking && (action.actionName == ActionName.Swing || (action.actionName == ActionName.Stab && action.directions[0] != Direction.Backward))) return;
        else if (action.actionName == ActionName.Kick) { targetCombatHumanoid.GetKicked(weapon); }
        else
        {
            targetCombatHumanoid.TakeDamage(weapon);
            if (targetCombatHumanoid.GetCombatStance() == CombatStance.Fallen)
            {
                DenyAction(targetCombatHumanoid);
            }
        }

    }

    void DenyAction(CombatHumanoid character)
    {
        character.DenyAction();
        if (character.GetType() == typeof(CombatPlayer))
        {
            playerActionsQueue.Clear();
            if (playerTimeline[playerTimeline.Count - 1].Item1 > combatTotalTime) playerTimeline[playerTimeline.Count - 1] = new Tuple<float, Action>(combatTotalTime, playerTimeline[playerTimeline.Count - 1].Item2);
            if (isPlayersTurn) character.PromptAction(GetAvailableActions(character));
        }
        else
        {
            enemyActionsQueue.Clear();
            if (enemyTimeline[enemyTimeline.Count - 1].Item1 > combatTotalTime) enemyTimeline[enemyTimeline.Count - 1] = new Tuple<float, Action>(combatTotalTime, enemyTimeline[enemyTimeline.Count - 1].Item2);
            if (!isPlayersTurn) character.PromptAction(GetAvailableActions(character));
        }
    }

    void SwitchTurn()
    {
        isPlayersTurn = !isPlayersTurn;
        UpdateIndicator();

        float givenTime = 0f;

        if (isPlayersTurn)
        {
            Debug.Log("Player turn");

            if (isPlayerInAction && lastEnemyAction != null)
            {
                givenTime = Math.Max(lastEnemyAction.Item1, playerTimeline[playerTimeline.Count - 1].Item1) - combatTotalTime;
                lastEnemyAction = null;
            }
            else if (!isPlayerInAction && lastEnemyAction != null)
            {
                givenTime = lastEnemyAction.Item1 - combatTotalTime;
                lastEnemyAction = null;
            }
            else if (lastEnemyAction == null)
            {
                givenTime = freeTime;
            }

            givenTime *= player.GetComposure() / player.GetMaxComposure();
            player.PromptAction(GetAvailableActions(player));
        }
        else 
        {
            Debug.Log("Enemey turn");
            if (isEnemyInAction && lastPlayerAction != null)
            {
                givenTime = Math.Max(lastPlayerAction.Item1, enemyTimeline[enemyTimeline.Count - 1].Item1) - combatTotalTime;
                lastPlayerAction = null;
            }
            else if(!isEnemyInAction && lastPlayerAction != null)
            {
                givenTime = lastPlayerAction.Item1 - combatTotalTime;
                lastPlayerAction = null;
            }
            else if (lastPlayerAction == null)
            {
                givenTime = freeTime;
            }

            givenTime *= enemy.GetComposure() / enemy.GetMaxComposure();
            enemy.PromptAction(GetAvailableActions(enemy)); 
        }
        givenTime += 5f;
        Debug.Log("Total given time: " + givenTime);
        timer.EnableTimer(givenTime);
    }

    void UpdateIndicator()
    {
        //Debug.Log("Updating indicator [" + playersTurn + "]");
        Color color;
        Vector2 pos;

        if (isPlayersTurn)
        {
            color = Color.green;
            pos = new Vector2(-20, 0);
        }

        else
        {
            color = Color.red;
            pos = new Vector2(20, 0);
        }

        // update color
        foreach (UnityEngine.UI.Image image in images)
        {
            image.color = color;
        }

        // update position
        indicator.transform.GetChild(1).GetComponent<RectTransform>().localPosition = pos;
    }

    void EnableUi(bool _enable)
    {
        combatObjects.SetActive(_enable);
    }

    List<Action> GetAvailableActions(CombatHumanoid humanoid)
    {
        List<Action> result = new List<Action>();
        foreach (Action tempAction in actionList)
        {
            result.Add(new Action(tempAction));
        }
        float stamina = humanoid.GetStamina();
        float composure = humanoid.GetComposure();
        float maxComposure = humanoid.GetMaxComposure();
        float requiredStamina;
        float requiredComposure;
        CombatStance combatStance = humanoid.GetCombatStance();

        for (int i = 0; i < result.Count;)
        {
            requiredStamina = requiredComposure = 0f;
            Action action = result[i];
            // Remove if the state is not suitable
            if (!action.availableWhen.Contains(combatStance) && !action.availableWhen.Contains(CombatStance.Any))
            {
                result.RemoveAt(i);
                continue;
            }

            // Remove if there is not enough stamina or composure or equipment does not support the action
            if (action.actionType == ActionType.Movement || action.actionType == ActionType.Agile)
            {
                requiredStamina = action.baseStaminaDrain * (1 + humanoid.GetAllWeight() / 100);
                requiredComposure = action.baseComposureDrain;
            }
            else if (action.actionType == ActionType.Offensive)
            {
                if (action.actionName == ActionName.Kick)
                {
                    if (Vector3.Distance(player.transform.position, enemy.transform.position) > 2.5f)
                    {
                        //Debug.Log("Removing kick. The distance is: " + Vector3.Distance(player.transform.position, enemy.transform.position));
                        result.RemoveAt(i); continue;
                    }
                    requiredStamina = action.baseStaminaDrain * (1 + humanoid.GetAllWeight() / 100);
                    requiredComposure = action.baseComposureDrain * (1 + humanoid.GetAllWeight() / 100);
                }
                else
                {
                    if (humanoid.GetWeaponActions() == null)
                    {
                        result.RemoveAt(i); continue;
                    }
                    if (!humanoid.GetWeaponActions().Contains(action.actionName))
                    {
                        result.RemoveAt(i); continue;
                    }

                    requiredStamina = action.baseStaminaDrain * (1 + humanoid.GetWeaponWeight() / 100);
                    requiredComposure = action.baseComposureDrain * (1 + humanoid.GetWeaponWeight() / 100);
                }


            }
            else if (action.actionType == ActionType.Defensive && action.actionName != ActionName.Stand_Ground)
            {
                if (humanoid.GetShieldActions() == null)
                {
                    result.RemoveAt(i); continue;
                }
                if (!humanoid.GetShieldActions().Contains(action.actionName))
                {
                    result.RemoveAt(i); continue;
                }

                requiredStamina = action.baseStaminaDrain * (1 + humanoid.GetShieldWeight() / 100);
                requiredComposure = action.baseComposureDrain * (1 + humanoid.GetShieldWeight() / 100);
            }

            // One off for stand ground and skip
            if (action.actionName == ActionName.Stand_Ground)
            {
                if (composure >= maxComposure)
                {
                    result.RemoveAt(i); continue;
                }

                requiredStamina = action.baseStaminaDrain;
                requiredComposure = action.baseComposureDrain;
            }

            if (action.actionName == ActionName.Skip)
            {
                requiredStamina = action.baseStaminaDrain;
                requiredComposure = action.baseComposureDrain;
            }


            // Discard everything that cannot be performed because of composure or stamina
            if (requiredStamina > stamina || requiredComposure > composure)
            {
                result.RemoveAt(i); continue;
            }

            // Check boundaries and blocking objects for movement
            Vector2 position = new Vector2(humanoid.transform.position.x, humanoid.transform.position.z);
            if (action.actionName == ActionName.Step)
            {
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Right);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Left);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing), gridBoundaries)) action.AddDirection(Direction.Forward);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing), gridBoundaries)) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Run)
            {
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing * 2, 0), gridBoundaries)) action.AddDirection(Direction.Right);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing * 2, 0), gridBoundaries)) action.AddDirection(Direction.Left);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing * 2), gridBoundaries)) action.AddDirection(Direction.Forward);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing * 2), gridBoundaries)) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Dodge)
            {
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Right);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Left);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing), gridBoundaries)) action.AddDirection(Direction.Forward);
                if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing), gridBoundaries)) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Roll)
            {
                Direction direction = UtilityScripts.GetFacingDirection(humanoid.transform.rotation);
                if (direction == Direction.Forward || direction == Direction.Backward)
                {
                    if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Right);
                    if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0), gridBoundaries)) action.AddDirection(Direction.Left);
                }
                else
                {
                    if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing), gridBoundaries)) action.AddDirection(Direction.Forward);
                    if (UtilityScripts.IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing), gridBoundaries)) action.AddDirection(Direction.Backward);
                }


                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }

            result[i].staminaDrain = requiredStamina;
            result[i].composureDrain = requiredComposure;
            i++;
        }
        return result;
    }

    Action GetAction(ActionName actionName)
    {
        foreach(Action action in actionList)
        {
            if(action.actionName == actionName) return action;
        }
        Debug.LogError("No action exists with a name " + actionName);
        return null;
    }

    void PromptActionWhenFallen(CombatHumanoid character)
    {
        Debug.Log("Prompting for an action when fallen: " + character);
        if(isPlayersTurn && character.GetType() == typeof(CombatPlayer)){
            character.PromptAction(GetAvailableActions(character));
        }
        else if(!isPlayersTurn && character.GetType() == typeof(CombatEnemy))
        {
            character.PromptAction(GetAvailableActions(character));
        }
    }

    private void Update()
    {
        if (!inCombat) return;

        combatTotalTime += Time.deltaTime;

        isPlayerInAction = playerTimeline[playerTimeline.Count- 1].Item1 + 0.3f > combatTotalTime;
        isEnemyInAction = enemyTimeline[enemyTimeline.Count - 1].Item1 + 0.3f > combatTotalTime;

        // Remove lastAction if it's over
        if (lastPlayerAction != null && lastPlayerAction.Item1 > combatTotalTime) lastPlayerAction = null;
        if (lastEnemyAction != null && lastEnemyAction.Item1 > combatTotalTime) lastEnemyAction = null;

        // Checking every frame if a queued action can be done
        if (!isPlayerInAction && playerActionsQueue.Count != 0){
            Debug.Log("There is a queued action availaible for player: " +  playerActionsQueue.Peek().actionName);
            ApproveAction(playerActionsQueue.Dequeue(), player);
        }

        if (!isEnemyInAction && enemyActionsQueue.Count != 0)
        {
            Debug.Log("There is a queued action availaible for enemy: " + enemyActionsQueue.Peek().actionName);
            ApproveAction(enemyActionsQueue.Dequeue(), enemy);
        }
    }
}

