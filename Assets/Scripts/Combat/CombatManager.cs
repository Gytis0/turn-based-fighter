using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UIElements;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    bool playersTurn  = true;
    GameObject playerObject, enemyObject;
    CombatPlayer player;
    CombatEnemy enemy;
    Equipment playerEquipment, enemyEquipment;
    Timer timer;

    // Grid
    Tuple<Vector2, Vector2> gridBoundaries;
    float gridSpacing;
    [SerializeField] LayerMask movementBlockingObjects;

    // References
    [SerializeField] UnityEngine.UI.Image indicator;
    [SerializeField] List<Action> actionList;
    [SerializeField] List<Action> tempActionList;
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
        EnableUi(true);
        UpdateIndicator();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        enemyObject = GameObject.FindGameObjectWithTag("Enemy");

        player = playerObject.GetComponent<CombatPlayer>();
        enemy = enemyObject.GetComponent<CombatEnemy>();

        playerEquipment = playerObject.GetComponent<Equipment>();
        enemyEquipment = enemyObject.GetComponent<Equipment>();

        player.onPlayerTurnEnd += ApproveAction;
        enemy.onEnemyTurnEnd += ApproveAction;

        player.EnableCombatMode(true, gridSpacing);
        enemy.EnableCombatMode(true, gridSpacing);

        player.SetCombinations(combinationList);
        enemy.SetCombinations(combinationList);

        tempActionList = GetAvailableActions(player);
        player.EnableButtons(tempActionList);

        timer.EnableTimer(30f);
    }

    public void StopCombat()
    {
        EnableUi(false);

        enemy.EnableCombatMode(false, gridSpacing);
        player.EnableCombatMode(false, gridSpacing);
    }

    void ForcefullyEndTurn()
    {
        Debug.Log("Forcefully ending turn [" + playersTurn + "]");
        if (playersTurn) player.EndTurn();
        else enemy.EndTurn();
    }

    void ApproveAction(Action action, CombatHumanoid character)
    {
        bool approved = false;

        foreach (Action availableAction in tempActionList)
        {
            if (availableAction.actionName == action.actionName && (action.directions[0] == Direction.None || availableAction.directions.Contains(action.directions[0])))
            {
                character.ExecuteAction(action);
                approved = true;
                break;
            }
        }

        if (!approved && character.GetType() == typeof(CombatPlayer))
        {
            ((CombatPlayer)character).EnableButtons(tempActionList);
            Debug.Log("Turn denied. Prompting UI");

        }
        else if (!approved && character.GetType() == typeof(CombatEnemy))
        {
            // Tell AI to rethink its action
        }

        if (approved)
        {
            Debug.Log("Turn approved. Switching [" + playersTurn + "]");
            SwitchTurn();
            timer.EnableTimer(30f);
        }
    }

    void SwitchTurn()
    {
        Debug.Log("Switching turn [" + playersTurn + "]");
        playersTurn = !playersTurn;
        UpdateIndicator();
    }

    void UpdateIndicator()
    {
        Debug.Log("Updating indicator [" + playersTurn + "]");
        Color color;
        Vector2 pos;

        if (playersTurn)
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
        float requiredStamina = 0f;
        float requiredComposure = 0f;
        CombatStance combatStance = humanoid.GetCombatStance();

        for (int i = 0; i < result.Count;)
        {
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
                requiredStamina = action.baseStaminaDrain * (humanoid.GetAllWeight() / 100);
            }
            else if (action.actionType == ActionType.Offensive)
            {
                if (action.actionName == ActionName.Kick)
                {
                    if (Vector3.Distance(player.transform.position, enemy.transform.position) > 2f)
                    {
                        Debug.Log("Removing kick. The distance is: " + Vector3.Distance(player.transform.position, enemy.transform.position));
                        result.RemoveAt(i); continue;
                    }
                    requiredStamina = action.baseStaminaDrain * (humanoid.GetAllWeight() / 100);
                    requiredComposure = action.baseComposureDrain * (humanoid.GetAllWeight() / 100);
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

                    requiredStamina = action.baseStaminaDrain * (humanoid.GetWeaponWeight() / 100);
                    requiredComposure = action.baseComposureDrain * (humanoid.GetWeaponWeight() / 100);
                }


            }
            else if (action.actionType == ActionType.Defensive)
            {
                if (action.actionName == ActionName.Stand_Ground)
                {
                    if (composure >= maxComposure)
                    {
                        result.RemoveAt(i); continue;
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }

                if (humanoid.GetShieldActions() == null)
                {
                    result.RemoveAt(i); continue;
                }
                if (!humanoid.GetShieldActions().Contains(action.actionName))
                {
                    result.RemoveAt(i); continue;
                }

                requiredStamina = action.baseStaminaDrain * (humanoid.GetShieldWeight() / 100);
                requiredComposure = action.baseComposureDrain * (humanoid.GetShieldWeight() / 100);
            }

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
                if (humanoid.transform.rotation.y % 180 == 0)
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
}

