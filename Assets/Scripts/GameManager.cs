using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    GameObject cinematicCamera;
    GameObject preFightScreen;
    GameObject menuObject;
    MainMenu menu;

    // End screen
    GameObject endScreen;

    int[] points;
    Dictionary<int, ItemData> allItems = new();
    Dictionary<ArmorType, Armor> armorItems = new();

    GameObject enemy;
    HumanoidProperties enemyProperties;

    GameObject player;
    HumanoidProperties playerProperties;
    PlayerInventory playerInventory;

    ItemManager itemManager;
    CombatManager combatManager;

    Color redColor = new Color(0.564f, 0.243f, 0.27f, 1f);
    Color greenColor = new Color(0.243f, 0.564f, 0.27f, 1f);

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
        combatManager = CombatManager.Instance;
        menuObject = GameObject.FindGameObjectWithTag("Main Menu");
        endScreen = GameObject.FindGameObjectWithTag("End Screen");

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
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            // TEMPORARY //
            cinematicCamera = GameObject.FindGameObjectWithTag("Cinematic Camera");
            preFightScreen = GameObject.FindGameObjectWithTag("Pre Fight Screen");

            StateChanger.onStartGame += StartGame;

            enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemyProperties = enemy.GetComponent<HumanoidProperties>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<HumanoidProperties>();
            playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
            player.GetComponent<PlayerControls>().restrictedMovement = true;

            ItemData item1 = itemManager.GetItem("One Edged Axe").GetItemData();
            ItemData item2 = itemManager.GetItem("Broadsword").GetItemData();
            ItemData item3 = itemManager.GetItem("Hammer").GetItemData();
            ItemData item4 = itemManager.GetItem("Spear").GetItemData();
            ItemData shieldItemData = itemManager.GetItem("Wooden Shield").GetItemData();
            Armor chestplate = (Armor)(itemManager.GetItem("Iron Chest").GetItemData());
            allItems.Add(0, item1);
            allItems.Add(1, item2);
            allItems.Add(2, item3);
            allItems.Add(3, item4);
            allItems.Add(4, shieldItemData);
            armorItems.Add(ArmorType.Chestplate, chestplate);

            SetEnemyStats();
            SetEnemyItems();

            //SetPlayerStats();
            playerProperties.SetStats(new int[] { 3, 3, 3, 2 });
            SetPlayerItems();

            SetCharacterStatsWindow(preFightScreen.transform.GetChild(1).gameObject, "Enemy", enemyProperties.GetHealth(), enemyProperties.GetStamina(), enemyProperties.GetComposure(), false, playerProperties.GetIntelligence());
            SetCharacterStatsWindow(preFightScreen.transform.GetChild(2).gameObject, "Player", playerProperties.GetHealth(), playerProperties.GetStamina(), playerProperties.GetComposure(), true);

            combatManager.onCombatEnd += EndGame;
            StateChanger.onContinueGame += LoadMainMenu;

        }
    }

    private void OnDisable()
    {
        StateChanger.onStartGame -= StartGame;
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 0)
        {
            MainMenu.onGameStart += LoadEquipmentScene;
        }
        else if(level == 1)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<HumanoidProperties>();
            Interactable.onTravel += LoadFightScene;

            SetPlayerStats();
        }
        else if(level == 2)
        {
            StateChanger.onStartGame += StartGame;
            StateChanger.onContinueGame += LoadMainMenu;

            combatManager.onCombatEnd += EndGame;
            cinematicCamera = GameObject.FindGameObjectWithTag("Cinematic Camera");
            preFightScreen = GameObject.FindGameObjectWithTag("Pre Fight Screen");

            enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemyProperties = enemy.GetComponent<HumanoidProperties>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<HumanoidProperties>();
            playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
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
        MainMenu.onGameStart -= LoadEquipmentScene;
        menu.GetPoints().CopyTo(points, 0);
        
        SceneManager.LoadScene(1);
    }

    public void LoadFightScene()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        allItems = playerInventory.GetItemsInventory();
        armorItems = playerInventory.GetArmorInventory();
        Interactable.onTravel -= LoadFightScene;

        SceneManager.LoadScene(2);
    }

    public void LoadMainMenu()
    {
        combatManager.onCombatEnd -= EndGame;
        StateChanger.onStartGame -= StartGame;
        StateChanger.onContinueGame -= LoadMainMenu;
        endScreen.transform.GetChild(0).gameObject.SetActive(false);

        SceneManager.LoadScene(0);
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
        preFightScreen = GameObject.FindGameObjectWithTag("Pre Fight Screen");

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

        combatManager.StartCombat();
    }

    void SetCharacterStatsWindow(GameObject windowParentObject, string name, float health, float stamina, float composure, bool precise, int intelligence = 5)
    {
        GameObject windowObject = windowParentObject.transform.GetChild(0).gameObject;
        CharacterStatsWindow characterStatsUI = windowObject.GetComponent<CharacterStatsWindow>();
        characterStatsUI.SetTitle(name);

        if(precise || intelligence == 5) characterStatsUI.SetSlidersValues(health, stamina, composure);
        else
        {
            int range = (5-intelligence) * 20;

            int randomRangeDown = Random.Range(0, range + 1);

            float minHealth = Mathf.Clamp(health - randomRangeDown, 20, 100);
            float actualReduce = health - minHealth;
            float maxHealth = Mathf.Clamp(health + range - actualReduce, 20, 100);

            randomRangeDown = Random.Range(0, range + 1);

            float minStamina = Mathf.Clamp(stamina - randomRangeDown, 20, 100);
            actualReduce = stamina - minStamina;
            float maxStamina = Mathf.Clamp(stamina + range - actualReduce, 20, 100);

            randomRangeDown = Random.Range(0, range + 1);

            float minComposure = Mathf.Clamp(composure - randomRangeDown, 20, 100);
            actualReduce = composure - minComposure;
            float maxComposure = Mathf.Clamp(composure + range - actualReduce, 20, 100);

            characterStatsUI.SetSlidersIntervals(minHealth, maxHealth, minStamina, maxStamina, minComposure, maxComposure);
        }
    }

    public void EndGame(bool isPlayerWinner, HumanoidProperties playerProperties, HumanoidProperties enemyProperties)
    {
        Transform root = endScreen.transform.GetChild(0);
        root.gameObject.SetActive(true);
        TextMeshProUGUI title = root.Find("Header").GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI score = root.Find("Score").GetComponent<TextMeshProUGUI>();
        Image headerImage = root.Find("Header").GetComponent<Image>();
        Image footerImage = root.Find("Footer").GetComponent<Image>();
        CharacterStatsWindow playerStats = root.Find("Stats").GetChild(0).GetComponent<CharacterStatsWindow>();
        CharacterStatsWindow enemyStats = root.Find("Stats").GetChild(1).GetComponent<CharacterStatsWindow>();

        if (isPlayerWinner)
        {
            title.SetText("Victory");
            headerImage.color = footerImage.color = greenColor;
        }
        else
        {
            title.SetText("Defeat");
            headerImage.color = footerImage.color = redColor;
        }

        score.SetText("Score: " + CalculateScore(playerProperties));

        playerStats.SetTitle("Player");
        playerStats.SetSlidersValues(playerProperties.GetHealth(), playerProperties.GetMaxHealth(), playerProperties.GetStamina(), playerProperties.GetMaxStamina(), playerProperties.GetComposure(), playerProperties.GetMaxComposure()) ;

        enemyStats.SetTitle("Enemy");
        enemyStats.SetSlidersValues(enemyProperties.GetHealth(), enemyProperties.GetMaxHealth(), enemyProperties.GetStamina(), enemyProperties.GetMaxStamina(), enemyProperties.GetComposure(), enemyProperties.GetMaxComposure());
    }

    int CalculateScore(HumanoidProperties properties)
    {
        return (int)((properties.GetHealth() / properties.GetMaxHealth()) * 500 + (properties.GetStamina() / properties.GetMaxStamina()) * 300 + (properties.GetComposure() / properties.GetMaxComposure()) * 100);
    }
}
