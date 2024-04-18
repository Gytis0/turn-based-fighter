using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // References
    [SerializeField] Camera cinematicCamera;

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

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            Interactable.onTravel += LoadFightScene;
        }
        // TEMPORARY
        //SetEnemyStats();
        //SetEnemyItems();

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

            SetEnemyItems();
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
}
