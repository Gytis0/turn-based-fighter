using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public List<Item> items = new List<Item>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Armor> armors = new List<Armor>();
    public List<Shield> shields = new List<Shield>();

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

        foreach(Item item in items)
        {
            if (item.GetType() == typeof(Weapon))
            {
                weapons.Add((Weapon)item.GetItemData());
            }
            else if(item.GetType() == typeof(Armor))
            {
                armors.Add((Armor)item.GetItemData());
            }
            else if( item.GetType() == typeof(Shield))
            {
                shields.Add((Shield)item.GetItemData());
            }
        }

    }

    public Item GetItem(string itemName)
    {
        ItemData temp;
        foreach (Item item in items)
        {
            temp = item.GetComponent<Item>().GetItemData();
            if (temp.GetName().ToLower() == itemName.ToLower())
            {
                return item;
            }
        }

        return null;
    }
    public List<Weapon> GetAllWeapons()
    {
        return weapons;
    }
    public List<Armor> GetAllArmors() 
    {
        return armors;
    }
    public List<Shield> GetAllShields()
    {
        return shields;
    }

}
