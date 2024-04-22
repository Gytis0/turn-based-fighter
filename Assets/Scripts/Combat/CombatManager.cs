using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UIElements;
using System.Linq;

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
    [SerializeField] List<ActionCombination> combinationList;

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
        //parameters should accept all the info about fighters' properties and equipment
        EnableUi(true);
        UpdateIndicator();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        enemyObject = GameObject.FindGameObjectWithTag("Enemy");

        player = playerObject.GetComponent<CombatPlayer>();
        enemy = enemyObject.GetComponent<CombatEnemy>();

        playerEquipment = playerObject.GetComponent<Equipment>();
        enemyEquipment = enemyObject.GetComponent<Equipment>();

        player.onTurnDone += HandleTurnEnd;
        enemy.onTurnDone += HandleTurnEnd;

        player.EnableCombatMode(true);
        enemy.EnableCombatMode(true);

        player.SetCombinations(combinationList);
        enemy.SetCombinations(combinationList);

        player.EnableButtons(GetAvailableTurns(player));

        timer.EnableTimer(5f);
    }

    public void StopCombat()
    {
        EnableUi(false);

        enemy.EnableCombatMode(false);
        player.EnableCombatMode(false);
    }

    void ForcefullyEndTurn()
    {
        Debug.Log("Forcefully ending turn [" + playersTurn + "]");
        if (playersTurn) player.EndTurn();
        else enemy.EndTurn();
    }

    void HandleTurnEnd(Action action)
    {
        Debug.Log("Handling turn end [" + playersTurn + "]");
        SwitchTurn();
        timer.EnableTimer(3f);
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
        transform.GetChild(0).gameObject.SetActive(_enable);
    }

    List<Action> GetAvailableTurns(CombatHumanoid humanoid)
    {
        List<Action> result = new List<Action>(actionList);
        float stamina = humanoid.GetStamina();
        float requiredStamina;
        CombatState combatState = humanoid.GetCombatState();

        for(int i = 0; i < result.Count;)
        {
            Action action = result[i];
            // Remove if the state is not suitable
            if (!action.availableWhen.Contains(combatState) && !action.availableWhen.Contains(CombatState.Any))
            {
                result.RemoveAt(i);
                continue;
            }

            // Remove if there is not enough stamina
            if (action.actionType == ActionType.Movement || action.actionType == ActionType.Agile)
            {
                requiredStamina = action.baseStaminaDrain * (humanoid.GetAllWeight() / 100);
                if (requiredStamina > stamina)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionType == ActionType.Offensive)
            {
                requiredStamina = action.baseStaminaDrain * (humanoid.GetWeaponWeight() / 100);
                if (requiredStamina > stamina)
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if (action.actionType == ActionType.Defensive)
            {
                requiredStamina = action.baseStaminaDrain * (humanoid.GetShieldWeight() / 100);
                if (requiredStamina > stamina)
                {
                    result.RemoveAt(i); continue;
                }
            }

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
            else if(action.actionType == ActionType.Defensive)
            {
                if (humanoid.GetShieldActions() == null)
                {
                    result.RemoveAt(i); continue;
                }
                if (!humanoid.GetShieldActions().Contains(action.actionName))
                {
                    result.RemoveAt(i); continue;
                }
            }
            else if(action.actionType == ActionType.Offensive)
            {
                if (humanoid.GetWeaponActions() == null)
                {
                    result.RemoveAt(i); continue;
                }
                if (!humanoid.GetWeaponActions().Contains(action.actionName))
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
        Vector3 endPos3d = new Vector3(endPos.x, 0.5f, endPos.y);
        if (endPos.x >= gridBoundaries.Item1.x &&
            endPos.x <= gridBoundaries.Item2.x &&
            endPos.y >= gridBoundaries.Item1.y &&
            endPos.y <= gridBoundaries.Item2.y &&
            !Physics.Linecast(startPos, endPos3d, movementBlockingObjects)) return true;

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

