using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int[] points;
    Dictionary<int, ItemData> allItems = new();
    Dictionary<ArmorType, Armor> armorItems = new();


    GameObject menuObject;
    MainMenu menu;

    PlayerProperties playerProperties;
    PlayerInventory inventory;

    ItemManager itemManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        itemManager = ItemManager.Instance;
        menuObject = GameObject.FindGameObjectWithTag("Main Menu");
        if(menuObject != null)
        {
            menu = menuObject.GetComponent<MainMenu>();
            MainMenu.onGameStart += LoadEquipmentScene;
        }

        points = new int[4];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerProperties = player.GetComponent<PlayerProperties>();
        }

        // TEMPORARY
        SetEnemyStats();

    }



    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            Interactable.onTravel += LoadFightScene;
            MainMenu.onGameStart -= LoadEquipmentScene;

            SetPlayerStats();
        }
        else if(level == 2)
        {
            Interactable.onTravel -= LoadFightScene;

            SetPlayerItems();

            SetPlayerStats();

            SetEnemyStats();
        }

    }

    public void LoadEquipmentScene()
    {
        menu.GetPoints().CopyTo(points, 0);
        
        SceneManager.LoadScene(1);
    }

    public void LoadFightScene()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        allItems = inventory.GetItemsInventory();
        armorItems = inventory.GetArmorInventory();

        SceneManager.LoadScene(2);
    }

    void SetPlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerProperties = player.GetComponent<PlayerProperties>();
        playerProperties.SetStats(points);
    }

    void SetPlayerItems()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        inventory.SetItemInventory(allItems);
        inventory.SetArmorInventory(armorItems);
    }
    
    void SetEnemyStats()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        HumanoidProperties enemyProperties = enemy.GetComponent<HumanoidProperties>();

        int availablePoints = 12;
        int[] points = new int[4];

        for(int i = 0; i < 4; i++)
        {
            int tempPoints = Random.Range(1, Mathf.Min(5, availablePoints - (3-i)));
            availablePoints -= tempPoints;
            points[i] = tempPoints;
        }
       
        enemyProperties.SetStats(points);
    }

    void SetEnemyItems()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        Inventory inventory = enemy.GetComponentInChildren<Inventory>();

        List<Weapon> weapons = new List<Weapon>();
        Weapon randomWeapon = weapons[Random.Range(0, weapons.Count)];

        List<Armor> armors = new List<Armor>();

    }
}
