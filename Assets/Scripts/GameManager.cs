using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int[] points;
    Dictionary<int, ItemData> allItems = new();
    Dictionary<ArmorType, Armor> armorItems = new();


    MainMenu menu;

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
        menu = GameObject.FindGameObjectWithTag("Main Menu").GetComponent<MainMenu>();
        if(menu != null)
        {
            MainMenu.onGameStart += LoadEquipmentScene;
        }

        points = new int[4];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerProperties = player.GetComponent<PlayerProperties>();
        }
    }

    

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Loaded");
        if(level == 1)
        {
            Interactable.onTravel += LoadFightScene;
            MainMenu.onGameStart -= LoadEquipmentScene;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<PlayerProperties>();
            playerProperties.SetStats(points);
        }
        else if(level == 2)
        {
            Interactable.onTravel -= LoadFightScene;
            
            inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
            inventory.SetItemInventory(allItems);
            inventory.SetArmorInventory(armorItems);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerProperties = player.GetComponent<PlayerProperties>();
            playerProperties.SetStats(points);
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

}
