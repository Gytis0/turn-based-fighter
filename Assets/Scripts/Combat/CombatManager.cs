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
    Humanoid player, enemy;
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

       EnableUi(false);
    }

    void Start()
    {
        UpdateIndicator();
    }

    public void StartCombat()
    {
        //parameters should accept all the info about fighters' properties and equipment
        EnableUi(true);
        UpdateIndicator();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        enemyObject = GameObject.FindGameObjectWithTag("Enemy");

        player = playerObject.GetComponent<Humanoid>();
        enemy = enemyObject.GetComponent<Humanoid>();

        playerEquipment = playerObject.GetComponent<Equipment>();
        enemyEquipment = enemyObject.GetComponent<Equipment>();

        enemy.EnableCombatMode(true);
        player.EnableCombatMode(true);

        timer.enableTimer(10f);
    }

    public void StopCombat()
    {
        EnableUi(false);

        enemy.EnableCombatMode(false);
        player.EnableCombatMode(false);
    }

    

    void UpdateIndicator()
    {
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

