using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventory : Inventory
{
    InputController inputActions;

    Transform itemInventoryRoot;
    Dictionary<int, ItemData> allItems = new();
    List<Transform> allItemsSlots = new();

    Transform armorInventoryRoot;
    Dictionary<ArmorType, Armor> armorItems = new();
    Dictionary<ArmorType, Transform> armorItemsSlots = new();
    
    [SerializeField] Sprite emptySlot;

    ItemManager itemManager;
    int inventorySize = 6;
    private void Awake()
    {
        itemManager = ItemManager.Instance;
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
        if (allItems.Count >= inventorySize) return;
        int index = FindFirstEmptySlot();

        allItems.Add(index, item.GetComponent<Item>().GetItemData());
        Destroy(item);
        UpdateItemInventory();
    }

    void AddItemFromArmorInventory(ItemData itemData, int index)
    {
        allItems.Add(index, itemData);
        armorItems.Remove(((Armor)itemData).GetArmorType());

        UpdateItemInventory();
        UpdateArmorInventory();
    }

    void DropItem(int index)
    {
        Item droppedItem = itemManager.GetItem(allItems[index].GetName());
        ItemData droppedItemData = droppedItem.GetItemData();
        Instantiate(droppedItem, transform.position, droppedItemData.GetDropRotation());

        allItems.Remove(index);

        UpdateItemInventory();
    }

    void SwitchItem(int indexFrom, int indexTo)
    {
        ItemData temp;

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
        for (int i = 0; i < inventorySize; i++)
        {
            tempImage = allItemsSlots[i].GetComponent<Image>();
            tempItemData = allItemsSlots[i].GetComponent<ItemSlot>();

            if (allItems.ContainsKey(i))
            {
                tempImage.sprite = allItems[i].GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = allItems[i];
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
        for (int i = 0; i < inventorySize; i++)
        {
            if (!allItems.ContainsKey(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void SetItemInventory(Dictionary<int, ItemData> inventory)
    {
        allItems = inventory;
        UpdateItemInventory();
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
        Item droppedItem = itemManager.GetItem(armorItems[armorType].GetName());
        ItemData droppedItemData = droppedItem.GetItemData();
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