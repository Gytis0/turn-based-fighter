using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int[] points;
    Dictionary<int, ItemData> allItems = new();
    Dictionary<ArmorType, Armor> armorItems = new();


    [SerializeField] MainMenu menu;

    PlayerProperties playerProperties;
    PlayerInventory inventory;

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
    }

    private void Start()
    {
        Interactable.onTravel += LoadFightScene;

        points = new int[4];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerProperties = player.GetComponent<PlayerProperties>();
        }
    }

    

    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            Interactable.onTravel += LoadFightScene;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
            playerProperties = player.GetComponent<PlayerProperties>();
            playerProperties.SetStats(points);
        }
        else if(level == 2)
        {
            inventory.SetItemInventory(allItems);
            inventory.SetArmorInventory(armorItems);
        }

    }

    private void OnEnable()
    {
        Interactable.onTravel += LoadFightScene;
    }

    private void OnDisable()
    {
        Interactable.onTravel -= LoadFightScene;
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

}
