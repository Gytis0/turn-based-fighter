using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections.Generic;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    bool playersTurn  = true;
    GameObject player, enemy;
    HumanoidStats playerStats, enemyStats;
    Equipment playerEquipment, enemyEquipment;
    Timer timer;

    // References
    [SerializeField]
    Image indicator;

    [SerializeField]
    List <Image> images = new();

    [SerializeField] List<Button> agileButtons = new();
    [SerializeField] List<Button> offenseButtons = new();
    [SerializeField] List<Button> defenseButtons = new();

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

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<HumanoidStats>();
        playerEquipment = player.GetComponent<Equipment>();
        timer = transform.GetComponent<Timer>();

       EnableUi(false);
    }

    void Start()
    {
        UpdateIndicator();
    }

    public void StartCombat(GameObject _enemy)
    {
        //parameters should accept all the info about fighters' properties and equipment
        EnableUi(true);
        enemy = _enemy;
        enemyStats = enemy.GetComponent<HumanoidStats>();
        enemyEquipment = enemy.GetComponent<Equipment>();

        enemyStats.EnableCombatMode(true);
        playerStats.EnableCombatMode(true);

        timer.enableTimer(10f);
    }

    public void StopCombat()
    {
        EnableUi(false);

        enemyStats.EnableCombatMode(false);
        playerStats.EnableCombatMode(false);
    }

    void EnableButtons()
    {
        List<WeaponActions> weaponActions = playerEquipment.GetEquippedWeapon().GetWeaponActions();

        int i = 0, limit = weaponActions.Count;

        foreach (Button button in offenseButtons)
        {
            if (i < limit)
            {
                button.gameObject.SetActive(true);
                button.transform.GetComponentInChildren<TextMeshProUGUI>().SetText(weaponActions[i].ToString());
                if (weaponActions[i] == WeaponActions.Swing)
                {
                    button.onClick.AddListener(delegate { SwingWeapon(true); }) ;
                }
                else if (weaponActions[i] == WeaponActions.Overhead) 
                {
                    button.onClick.AddListener(OverheadWeapon);
                }
                else if (weaponActions[i] == WeaponActions.Stab)
                {
                    button.onClick.AddListener(StabEnemy);
                }
            }

            i++;
        }
    }

    void DisableButtons()
    {
        foreach (Button button in agileButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in offenseButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in defenseButtons)
        {
            button.gameObject.SetActive(false);
        }
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
        if (_enable)
        {
            EnableButtons();
        }
        else
        {
            DisableButtons();
        }
    }

    private void OnEnable()
    {
        Interactable.onAttack += StartCombat;
    }

    private void OnDisable()
    {
        Interactable.onAttack -= StartCombat;
    }

    // Actions
    public void SwingWeapon(bool left)
    {
        // Play an animation for left or right swing
        enemyStats.TakeDamage(playerEquipment.GetEquippedWeapon().GetDamage());

    }

    public void OverheadWeapon()
    {
        enemyStats.TakeDamage(playerEquipment.GetEquippedWeapon().GetDamage());
    }

    public void StabEnemy()
    {
        enemyStats.TakeDamage(playerEquipment.GetEquippedWeapon().GetDamage());
    }
}

