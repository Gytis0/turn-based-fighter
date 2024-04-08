using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    InputController inputActions;

    Transform itemInventoryRoot;
    Dictionary<int, Item> allItems = new();
    List<Transform> allItemsSlots = new();

    Transform armorInventoryRoot;
    Dictionary<ArmorType, Armor> armorItems = new();
    Dictionary<ArmorType, Transform> armorItemsSlots = new();
    
    [SerializeField] Sprite emptySlot;

    [SerializeField] GameObject ironHelmet;
    [SerializeField] GameObject ironChest;
    [SerializeField] GameObject ironShoulders;
    [SerializeField] GameObject ironGloves;
    [SerializeField] GameObject ironPants;
    [SerializeField] GameObject ironBoots;

    [SerializeField] GameObject boots;
    [SerializeField] GameObject pants;
    [SerializeField] GameObject chest;


    private void Awake()
    {
        itemInventoryRoot = GameObject.FindGameObjectWithTag("Item Inventory").transform;
        armorInventoryRoot = GameObject.FindGameObjectWithTag("Armor Inventory").transform;

        foreach (Transform t in itemInventoryRoot)
        {
            allItemsSlots.Add(t.GetChild(0));
        }

        for(int i = 0; i < armorInventoryRoot.childCount; i++)
        {
            armorItemsSlots.Add((ArmorType)Enum.GetValues(typeof(ArmorType)).GetValue(i), armorInventoryRoot.GetChild(i).GetChild(0));
        }

        inputActions = new InputController();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        Interactable.onPickUp += AddItem;
        Interactable.onPickUpAndEquip += EquipFromGround;

        DragAndDrop.onItemAdd += AddItemFromArmorInventory;
        DragAndDrop.onItemDrop += DropItem;
        DragAndDrop.onArmorEquip += EquipArmor;
        DragAndDrop.onArmorDrop += DropArmor;
        DragAndDrop.onSwitch += SwitchItem;
    }

    private void OnDisable()
    {
        Interactable.onPickUp -= AddItem;
        Interactable.onPickUpAndEquip -= EquipFromGround;

        DragAndDrop.onItemAdd -= AddItemFromArmorInventory;
        DragAndDrop.onItemDrop -= DropItem;
        DragAndDrop.onArmorEquip -= EquipArmor;
        DragAndDrop.onArmorDrop -= DropArmor;
        DragAndDrop.onSwitch -= SwitchItem;
    }

    // All items------------------------------------------------------------

    void AddItem(GameObject item)
    {
        if (allItems.Count >= allItemsSlots.Count) return;
        int index = FindFirstEmptySlot();

        allItems.Add(index, item.GetComponent<Item>());
        Destroy(item);
        UpdateItemInventory();
    }

    void AddItemFromArmorInventory(ItemData itemData, int index)
    {
        Item item = ItemManager.GetItemObject(itemData.GetName()).GetComponent<Item>();
        allItems.Add(index, item);
        armorItems.Remove(((Armor)itemData).GetArmorType());

        UpdateItemInventory();
        UpdateArmorInventory();
    }

    void DropItem(int index)
    {
        GameObject droppedItem = ItemManager.GetItemObject(allItems[index].GetItemData().GetName());
        ItemData droppedItemData = droppedItem.GetComponent<Item>().GetItemData();
        Instantiate(droppedItem, transform.position, droppedItemData.GetDropRotation());

        allItems.Remove(index);

        UpdateItemInventory();
    }

    void SwitchItem(int indexFrom, int indexTo)
    {
        Item temp;

        if (allItems.TryGetValue(indexTo, out temp))
        {
            temp = allItems[indexTo];
        }
        allItems[indexTo] = allItems[indexFrom];

        if (temp != null)
        {
            allItems[indexFrom] = temp;
        }
        else
        {
            allItems.Remove(indexFrom);
        }

        UpdateItemInventory();
    }

    void UpdateItemInventory()
    {
        Image tempImage;
        ItemSlot tempItemData;
        for (int i = 0; i < allItemsSlots.Count; i++)
        {
            tempImage = allItemsSlots[i].GetComponent<Image>();
            tempItemData = allItemsSlots[i].GetComponent<ItemSlot>();

            if (allItems.ContainsKey(i))
            {
                tempImage.sprite = allItems[i].GetItemData().GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = allItems[i].GetItemData();
            }
            else
            {
                tempImage.sprite = emptySlot;
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 0.5f);

                tempItemData.itemData = null;
            }
        }
    }

    int FindFirstEmptySlot()
    {
        for (int i = 0; i < allItemsSlots.Count; i++)
        {
            if (!allItems.ContainsKey(i))
            {
                return i;
            }
        }
        return -1;
    }

    // Armors------------------------------------------------------------

    void EquipFromGround(GameObject item)
    {
        Armor armor = ((Armor) item.transform.GetComponent<Item>().GetItemData());
        if (armorItems.ContainsKey(armor.GetArmorType()))
        {
            return;
        }

        armorItems.Add(armor.GetArmorType(), armor);
        Destroy(item);

        UpdateArmorInventory();
    }

    void EquipArmor(Armor armor, int index)
    {
        ArmorType armorType = armor.GetArmorType();
        if (armorItems.ContainsKey(armorType))
        {
            return;
        }

        allItems.Remove(index);
        armorItems.Add(armorType, armor);

        UpdateItemInventory();
        UpdateArmorInventory();
    }

    void DropArmor(ArmorType armorType)
    {
        GameObject droppedItem = ItemManager.GetItemObject(armorItems[armorType].GetName());
        ItemData droppedItemData = droppedItem.GetComponent<Item>().GetItemData();
        Instantiate(droppedItem, transform.position, droppedItemData.GetDropRotation());

        armorItems.Remove(armorType);

        UpdateArmorInventory();
    }

    void UpdateArmorInventory()
    {
        Image tempImage;
        ItemSlot tempItemData;
        foreach(ArmorType armorType in Enum.GetValues(typeof(ArmorType)))
        {
            tempImage = armorItemsSlots[armorType].GetComponent<Image>();
            tempItemData = armorItemsSlots[armorType].GetComponent<ItemSlot>();

            if (armorItems.ContainsKey(armorType))
            {
                tempImage.sprite = armorItems[armorType].GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = armorItems[armorType];
            }
            else
            {
                tempImage.sprite = emptySlot;
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 0.5f);

                tempItemData.itemData = null;
            }
        }

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
}