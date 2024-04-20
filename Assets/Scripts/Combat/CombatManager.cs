using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    bool playersTurn  = true;
    GameObject playerObject, enemyObject;
    CombatHumanoid player, enemy;
    Equipment playerEquipment, enemyEquipment;
    Timer timer;

    // References
    [SerializeField] Image indicator;
    [SerializeField] List<Action> actionList;
    [SerializeField] List<ActionCombination> combinationList;

    [SerializeField] List <Image> images = new();

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
        Time.timeScale = 0.1f;

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
        foreach (Image image in images)
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
}

