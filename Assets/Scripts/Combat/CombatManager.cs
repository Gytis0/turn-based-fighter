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

    // Grid
    Tuple<Vector2, Vector2> gridBoundaries;
    float gridSpacing;
    [SerializeField] LayerMask movementBlockingObjects;

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
        enemy.onEnemyTurnEnd += AddActionToEnemyQueue;
        enemyEquipment.GetEquippedWeaponObject().GetComponent<OffensiveTrigger>().onEnemyHit += ApproveAttack;
        player.onDeath += StopCombat;
        enemy.onDeath += StopCombat;
        player.onFallen += PromptActionWhenFallen;
        enemy.onFallen += PromptActionWhenFallen;

        player.EnableCombatMode(true, gridSpacing);
        enemy.EnableCombatMode(true, gridSpacing);

        player.SetCombinations(combinationList);
        enemy.SetCombinations(combinationList);

        player.PromptAction(GetAvailableActions(player));

        timer.EnableTimer(60f);
    }

    public void StopCombat(CombatHumanoid loser)
    {
        inCombat = false;
        EnableUi(false);

        enemy.EnableCombatMode(false, gridSpacing);
        player.EnableCombatMode(false, gridSpacing);

        bool isPlayerWinner;
        HumanoidProperties playerProperties = player.GetComponent<HumanoidProperties>();
        HumanoidProperties enemyProperties = enemy.GetComponent<HumanoidProperties>();
        isPlayerWinner = loser.GetType() == typeof(CombatEnemy);

        onCombatEnd(isPlayerWinner, playerProperties, enemyProperties);
    }

    void ForcefullyEndTurn()
    {
        //Debug.Log("Forcefully ending turn [" + playersTurn + "]");
        if (isPlayersTurn) player.EndTurn();
        else enemy.EndTurn();
    }

    void AddActionToPlayerQueue(Action action)
    {
        playerActionsQueue.Enqueue(action);
    }

    void AddActionToEnemyQueue(Action action)
    {
        enemyActionsQueue.Enqueue(action);
    }

    void ApproveAction(Action action, CombatHumanoid character)
    {
        Debug.Log("Approving " + action.actionName);
        bool approved = false;
        List<Action> tempActionList = GetAvailableActions(character);
        foreach (Action availableAction in tempActionList)
        {
            if (action.actionName == ActionName.Skip || (availableAction.actionName == action.actionName && (action.directions[0] == Direction.None || availableAction.directions.Contains(action.directions[0]))))
            {
                character.ExecuteAction(action);
                approved = true;
                break;
            }
        }

        if (!approved)
        {
            DenyAction(character);
            character.PromptAction(tempActionList);
            Debug.Log("Action " + action.actionName.ToString() + " got denied from " + character.name);
        }

        if (approved)
        {
            character.ExecuteAction(action);
            SwitchTurn();
            timer.EnableTimer(60f);
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
        else if (targetCombatHumanoid.GetCombatState() == CombatState.Blocking && action.actionName == ActionName.Stab) targetEquipment.DamageShield(weapon);
        else if (targetCombatHumanoid.GetCombatState() == CombatState.Blocking_Left && action.actionName == ActionName.Swing && action.directions[0] == Direction.Right) targetEquipment.DamageShield(weapon);
        else if(targetCombatHumanoid.GetCombatState() == CombatState.Blocking_Right && action.actionName == ActionName.Swing && action.directions[0] == Direction.Left) targetEquipment.DamageShield(weapon);
        else if(action.actionName == ActionName.Kick) { targetCombatHumanoid.GetKicked(); }
        else
        {
            targetCombatHumanoid.TakeDamage(weapon);
            if(targetCombatHumanoid.GetCombatStance() == CombatStance.Fallen)
            {
                DenyAction(targetCombatHumanoid);
            }
        }

    }

    void DenyAction(CombatHumanoid character)
    {
        character.DenyAction();
        if (character.GetType() == typeof(CombatPlayer)) playerActionsQueue.Clear();
        else enemyActionsQueue.Clear();
    }

    void SwitchTurn()
    {
        //Debug.Log("Switching turn [" + playersTurn + "]");
        isPlayersTurn = !isPlayersTurn;
        UpdateIndicator();

        if(isPlayersTurn) { player.PromptAction(GetAvailableActions(player)); }
        else { enemy.PromptAction(GetAvailableActions(enemy)); }
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
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0))) action.AddDirection(Direction.Right);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0))) action.AddDirection(Direction.Left);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing))) action.AddDirection(Direction.Forward);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing))) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Run)
            {
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing * 2, 0))) action.AddDirection(Direction.Right);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing * 2, 0))) action.AddDirection(Direction.Left);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing * 2))) action.AddDirection(Direction.Forward);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing * 2))) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Dodge)
            {
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0))) action.AddDirection(Direction.Right);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0))) action.AddDirection(Direction.Left);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing))) action.AddDirection(Direction.Forward);
                if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing))) action.AddDirection(Direction.Backward);
                if (action.directions.Count == 0)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionName == ActionName.Roll)
            {
                Direction direction = GetRollDirection(humanoid.transform.rotation);
                if (direction == Direction.Forward || direction == Direction.Backward)
                {
                    if (IsPositionValid(humanoid.transform.position, position + new Vector2(gridSpacing, 0))) action.AddDirection(Direction.Right);
                    if (IsPositionValid(humanoid.transform.position, position + new Vector2(-gridSpacing, 0))) action.AddDirection(Direction.Left);
                }
                else
                {
                    if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, gridSpacing))) action.AddDirection(Direction.Forward);
                    if (IsPositionValid(humanoid.transform.position, position + new Vector2(0, -gridSpacing))) action.AddDirection(Direction.Backward);
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

    bool IsPositionValid(Vector3 startPos, Vector2 endPos)
    {
        startPos.Set(startPos.x, .5f, startPos.z);
        Vector3 endPos3d = new Vector3(endPos.x, 0.5f, endPos.y);
        float length = Vector3.Distance(startPos, endPos3d);
        RaycastHit[] hits = Physics.RaycastAll(startPos, (endPos3d - startPos), length, movementBlockingObjects);

        if (endPos.x >= gridBoundaries.Item1.x &&
            endPos.x <= gridBoundaries.Item2.x &&
            endPos.y >= gridBoundaries.Item1.y &&
            endPos.y <= gridBoundaries.Item2.y &&
            hits.Length == 0) return true;

        return false;
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

    Direction GetRollDirection(Quaternion rotation)
    {
        float normalized = rotation.normalized.y;
        if (normalized <= 0.125 || normalized > 0.875) return Direction.Forward;
        else if (normalized <= 0.375 && normalized > 0.125) return Direction.Right;
        else if (normalized <= 0.625 && normalized > 0.375) return Direction.Backward;
        else return Direction.Left;
    }

    void PromptActionWhenFallen(CombatHumanoid character)
    {
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

        string playerAnimation = player.GetCurrentAnimationName().ToLower();
        if (playerAnimation.Contains("idle") && playerActionsQueue.Count != 0){
            ApproveAction(playerActionsQueue.Dequeue(), player);
        }

        string enemyAnimation = enemy.GetCurrentAnimationName().ToLower();
        if (enemyAnimation.Contains("idle") && enemyActionsQueue.Count != 0)
        {
            ApproveAction(enemyActionsQueue.Dequeue(), enemy);
        }
    }
}

