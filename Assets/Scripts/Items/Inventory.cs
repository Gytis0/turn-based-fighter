using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    protected Dictionary<int, ItemData> allItems = new();

    protected Dictionary<ArmorType, Armor> armorItems = new();

    [SerializeField] protected GameObject ironHelmet;
    [SerializeField] protected GameObject ironChest;
    [SerializeField] protected GameObject ironShoulders;
    [SerializeField] protected GameObject ironGloves;
    [SerializeField] protected GameObject ironPants;
    [SerializeField] protected GameObject ironBoots;

    [SerializeField] protected GameObject boots;
    [SerializeField] protected GameObject pants;
    [SerializeField] protected GameObject chest;

    // All items------------------------------------------------------------

    public Dictionary<int, ItemData> GetItemsInventory()
    {
        return allItems;
    }

    public virtual void SetItemInventory(Dictionary<int, ItemData> inventory)
    {
        allItems = inventory;
    }

    public Dictionary<int, ItemData> GetInventory()
    {
        return allItems;
    }

    // Armors------------------------------------------------------------

    protected void UpdateArmorInventory()
    {
        // :((
        if (!armorItems.ContainsKey(ArmorType.Feet))
        {
            boots.SetActive(false); ironBoots.SetActive(false);
        }
        else if (armorItems[ArmorType.Feet].GetName().ToLower() == "boots") boots.SetActive(true);
        else ironBoots.SetActive(true);

        if (!armorItems.ContainsKey(ArmorType.Chestplate))
        {
            chest.SetActive(false); ironChest.SetActive(false); ironShoulders.SetActive(false); ironGloves.SetActive(false);
        }
        else if (armorItems[ArmorType.Chestplate].GetName().ToLower() == "chest") chest.SetActive(true);
        else
        {
            ironChest.SetActive(true); ironShoulders.SetActive(true); ironGloves.SetActive(true);
        }

        if (!armorItems.ContainsKey(ArmorType.Legs))
        {
            pants.SetActive(false); ironPants.SetActive(false);
        }
        else if (armorItems[ArmorType.Legs].GetName().ToLower() == "pants") pants.SetActive(true);
        else ironPants.SetActive(true);

        if (!armorItems.ContainsKey(ArmorType.Head))
        {
            ironHelmet.SetActive(false);
        }
        else ironHelmet.SetActive(true);
    }

    public Dictionary<ArmorType, Armor> GetArmorInventory()
    {
        return armorItems;
    }

    public void SetArmorInventory(Dictionary<ArmorType, Armor> inventory)
    {
        armorItems = inventory;
        UpdateArmorInventory();
    }
}