using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // References
    GameObject cinematicCamera;
    GameObject preFightScreen;

    int[] points;
    Dictionary<int, ItemData> allItems = new();
    Dictionary<ArmorType, Armor> armorItems = new();


    GameObject menuObject;
    MainMenu menu;

    GameObject enemy;
    HumanoidProperties enemyProperties;

    GameObject player;
    HumanoidProperties playerProperties;
    PlayerInventory playerInventory;

    ItemManager itemManager;
    CombatManager combatManager;

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
            playerProperties = player.GetComponent<HumanoidProperties>();
        }

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            Interactable.onTravel += LoadFightScene;
        }

        // TEMPORARY
        cinematicCamera = GameObject.FindGameObjectWithTag("Cinematic Camera");
        preFightScreen = GameObject.FindGameObjectWithTag("Pre Fight Screen");

        StartButton.onStartGame += StartGame;

        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyProperties = enemy.GetComponent<HumanoidProperties>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerProperties = player.GetComponent<HumanoidProperties>();
        player.GetComponent<PlayerControls>().restrictedMovement = true;

        ItemData hammerItemData = itemManager.GetItem("Sword").GetItemData();
        ItemData shieldItemData = itemManager.GetItem("Wooden Shield").GetItemData();
        Armor chestplate = (Armor)(itemManager.GetItem("Iron Chest").GetItemData());
        allItems.Add(0, hammerItemData);
        allItems.Add(1, shieldItemData);
        armorItems.Add(ArmorType.Chestplate, chestplate);

        SetEnemyStats();
        SetEnemyItems();

        //SetPlayerStats();
        playerProperties.SetStats(new int[] { 3, 3, 3, 4 });
        SetPlayerItems();

        SetCharacterStatsWindow(preFightScreen.transform.GetChild(1).gameObject, "Enemy", enemyProperties.GetHealth(), enemyProperties.GetStamina(), enemyProperties.GetComposure(), false, playerProperties.GetIntelligence());
        SetCharacterStatsWindow(preFightScreen.transform.GetChild(2).gameObject, "Player", playerProperties.GetHealth(), playerProperties.GetStamina(), playerProperties.GetComposure(), true);

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
            StartButton.onStartGame += StartGame;

            cinematicCamera = GameObject.FindGameObjectWithTag("Cinematic Camera");
            preFightScreen = GameObject.FindGameObjectWithTag("Pre Fight Screen");

            enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemyProperties = enemy.GetComponent<HumanoidProperties>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<HumanoidProperties>();
            player.GetComponent<PlayerControls>().restrictedMovement = true;

            SetPlayerItems();

            SetPlayerStats();

            SetEnemyStats();

            SetEnemyItems();

            SetCharacterStatsWindow(preFightScreen.transform.GetChild(1).gameObject, "Enemy", enemyProperties.GetHealth(), enemyProperties.GetStamina(), enemyProperties.GetComposure(), false, playerProperties.GetIntelligence());
            SetCharacterStatsWindow(preFightScreen.transform.GetChild(2).gameObject, "Player", playerProperties.GetHealth(), playerProperties.GetStamina(), playerProperties.GetComposure(), true);

        }

    }

    public void LoadEquipmentScene()
    {
        menu.GetPoints().CopyTo(points, 0);
        
        SceneManager.LoadScene(1);
    }

    public void LoadFightScene()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        allItems = playerInventory.GetItemsInventory();
        armorItems = playerInventory.GetArmorInventory();

        SceneManager.LoadScene(2);
    }

    void SetPlayerStats()
    {
        playerProperties.SetStats(points);
    }

    void SetPlayerItems()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        playerInventory.SetItemInventory(allItems);
        playerInventory.SetArmorInventory(armorItems);
    }
    
    void SetEnemyStats()
    {
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
        // References
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        Inventory inventory = enemy.GetComponentInChildren<Inventory>();
        Equipment equipment = enemy.GetComponent<Equipment>();

        // Generate weapons and shields to equip
        Dictionary<int ,ItemData> itemsToEquip = new Dictionary<int ,ItemData>();
        List<Weapon> weapons = itemManager.GetAllWeapons();
        Weapon randomWeapon = weapons[Random.Range(0, weapons.Count)];
        itemsToEquip.Add(0, randomWeapon);

        if(randomWeapon.GetWeaponType() == WeaponType.OneHanded)
        {
            List<Shield> shields = itemManager.GetAllShields();
            Shield randomShield = shields[Random.Range(0, shields.Count)];
            itemsToEquip.Add(1, randomShield);
        }

        // Generate armors to equip
        List<Armor> armors = itemManager.GetAllArmors();
        List<Armor> selectedArmors = new List<Armor>();

        int randomIndex;
        for (int i = 0; i < 4; i++) 
        {
            randomIndex = Random.Range(0, armors.Count);
            selectedArmors.Add(armors[randomIndex]);
            armors.RemoveAt(randomIndex);
        }

        for (int i = 0; i < selectedArmors.Count - 1; i++) 
        {
            for (int j = i + 1; j < selectedArmors.Count; j++)
            {
                if (selectedArmors[i].GetArmorType() == selectedArmors[j].GetArmorType())
                {
                    if (selectedArmors[i].GetArmorPoints() > selectedArmors[j].GetArmorPoints())
                    {
                        selectedArmors.RemoveAt(j);
                        j -= 1;
                    }
                    else
                    {
                        selectedArmors.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
        }

        Dictionary<ArmorType, Armor> armorsToEquip = new Dictionary<ArmorType, Armor>();
        foreach (Armor armor in selectedArmors)
        {
            armorsToEquip[armor.GetArmorType()] = armor;
        }

        // Add armors to the inventory
        inventory.SetArmorInventory(armorsToEquip);

        equipment.EquipHand(itemManager.GetItem(itemsToEquip[0].GetName()), HandSlot.RightHand);
        if(itemsToEquip.Count > 1)
        {
            equipment.EquipHand(itemManager.GetItem(itemsToEquip[1].GetName()), HandSlot.LeftHand);
        }
    }

    public void StartGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerProperties = player.GetComponent<HumanoidProperties>();

        // Setting up armors for player
        Dictionary<ArmorType, Armor> playerArmors = playerInventory.GetArmorInventory();
        Equipment playerEquipment = playerProperties.transform.GetComponent<Equipment>();
        playerEquipment.SetEquippedArmors(playerArmors);

        // Setting up armors for enemy
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        Dictionary<ArmorType, Armor> enemyArmors = enemy.GetComponentInChildren<Inventory>().GetArmorInventory();
        Equipment enemyEquipment = enemy.GetComponent<Equipment>();
        enemyEquipment.SetEquippedArmors(enemyArmors);

        preFightScreen.SetActive(false);

        cinematicCamera.SetActive(false);

        player.transform.position = new Vector3(-16f, 0f, -10f);

        combatManager = CombatManager.Instance;
        combatManager.StartCombat();
    }

    void SetCharacterStatsWindow(GameObject windowParentObject, string name, int health, int stamina, int composure, bool precise, int intelligence = 5)
    {
        GameObject windowObject = windowParentObject.transform.GetChild(0).gameObject;
        CharacterStatsUI characterStatsUI = windowObject.GetComponent<CharacterStatsUI>();
        characterStatsUI.SetTitle(name);

        if(precise || intelligence == 5) characterStatsUI.SetSlidersValues(health, stamina, composure);
        else
        {
            int range = (5-intelligence) * 20;

            int randomRangeDown = Random.Range(0, range + 1);

            int minHealth = Mathf.Clamp(health - randomRangeDown, 20, 100);
            int actualReduce = health - minHealth;
            int maxHealth = Mathf.Clamp(health + range - actualReduce, 20, 100);

            randomRangeDown = Random.Range(0, range + 1);

            int minStamina = Mathf.Clamp(stamina - randomRangeDown, 20, 100);
            actualReduce = stamina - minStamina;
            int maxStamina = Mathf.Clamp(stamina + range - actualReduce, 20, 100);

            randomRangeDown = Random.Range(0, range + 1);

            int minComposure = Mathf.Clamp(composure - randomRangeDown, 20, 100);
            actualReduce = composure - minComposure;
            int maxComposure = Mathf.Clamp(composure + range - actualReduce, 20, 100);

            characterStatsUI.SetSlidersIntervals(minHealth, maxHealth, minStamina, maxStamina, minComposure, maxComposure);
        }
    }
}
