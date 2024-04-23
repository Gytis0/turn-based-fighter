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
        List<Action> result = new List<Action>();
        foreach(Action tempAction in actionList)
        {
            result.Add(new Action(tempAction));
        }
        float stamina = humanoid.GetStamina();
        float requiredStamina = 0f;
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

            // Remove if there is not enough stamina or equipment does not support the action
            if (action.actionType == ActionType.Movement || action.actionType == ActionType.Agile)
            {
                requiredStamina = action.baseStaminaDrain * (humanoid.GetAllWeight() / 100);
            }
            else if (action.actionType == ActionType.Offensive)
            {
                if (action.actionName == ActionName.Push)
                {
                    if (Vector3.Distance(player.transform.position, enemy.transform.position) > 2f)
                    {
                        Debug.Log("Removing push. The distance is: " + Vector3.Distance(player.transform.position, enemy.transform.position));
                        result.RemoveAt(i); continue;
                    }
                    requiredStamina = action.baseStaminaDrain * (humanoid.GetAllWeight() / 100);
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
                }

                
            }
            else if (action.actionType == ActionType.Defensive)
            {
                if (action.actionName == ActionName.Stand_Ground)
                {
                    i++;
                    continue;
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
            }

            if (requiredStamina > stamina)
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
        // TEMPORARY
        gizmoStart = startPos;
        gizmoEnd = endPos3d;
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

    Vector3 gizmoStart, gizmoEnd;
    void OnDrawGizmosSelected()
    {
        Vector2 position = new Vector2(player.transform.position.x, player.transform.position.z);
        IsPositionValid(player.transform.position, position + new Vector2(0, gridSpacing * 2));

        if (gizmoEnd != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(gizmoStart, gizmoEnd);
        }
       
    }
}

