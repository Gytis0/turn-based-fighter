using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventory : Inventory
{
    InputController inputActions;

    Transform itemInventoryRoot;
    List<Transform> allItemsSlots;

    Transform armorInventoryRoot;
    Dictionary<ArmorType, Transform> armorItemsSlots = new();

    Transform weaponInventoryRoot;
    List<Transform> weaponsSlots = new();

    Equipment equipment;
    
    [SerializeField] Sprite emptySlot;
    [SerializeField] Sprite lockedSlot;

    ItemManager itemManager;
    int inventorySize = 6;
    private void Awake()
    {
        itemManager = ItemManager.Instance;

        try
        {
            itemInventoryRoot = GameObject.FindGameObjectWithTag("Item Inventory").transform;
            armorInventoryRoot = GameObject.FindGameObjectWithTag("Armor Inventory").transform;
            weaponInventoryRoot = GameObject.FindGameObjectWithTag("Weapon Inventory").transform;
        }
        catch (System.Exception e) { }

        equipment = GetComponentInParent<Equipment>();

        if (itemInventoryRoot != null)
        { 
            allItemsSlots = new List<Transform>();
            foreach (Transform t in itemInventoryRoot)
            {
                allItemsSlots.Add(t.GetChild(0));
            }
        }

        if (itemInventoryRoot != null)
        {
            for (int i = 0; i < armorInventoryRoot.childCount; i++)
            {
                armorItemsSlots.Add((ArmorType)Enum.GetValues(typeof(ArmorType)).GetValue(i), armorInventoryRoot.GetChild(i).GetChild(0));
            }
        }

        if(weaponInventoryRoot != null)
        {
            foreach (Transform t in weaponInventoryRoot)
            {
                weaponsSlots.Add(t.GetChild(0));
            }
        }

        inputActions = new InputController();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            Interactable.onPickUp += AddItem;
            Interactable.onPickUpAndEquip += EquipFromGround;

            DragAndDrop.onItemAdd += AddItemFromArmorInventory;
            DragAndDrop.onItemDrop += DropItem;
            DragAndDrop.onArmorEquip += EquipArmor;
            DragAndDrop.onArmorDrop += DropArmor;
            DragAndDrop.onSwitch += SwitchItem;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            DragAndDropEquipment.onItemAddFromArmor += AddItemFromArmorInventory;
            DragAndDropEquipment.onItemDrop += DropItem;
            DragAndDropEquipment.onArmorEquip += EquipArmor;
            DragAndDropEquipment.onArmorDrop += DropArmor;
            DragAndDropEquipment.onSwitch += SwitchItem;
            DragAndDropEquipment.onWeaponryEquip += EquipWeapon;
            DragAndDropEquipment.onWeaponryUnequip += UnequipWeapon;
            DragAndDropEquipment.onItemAddFromWeaponry += AddItemFromWeaponryInventory;
        }
        
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Interactable.onPickUp -= AddItem;
            Interactable.onPickUpAndEquip -= EquipFromGround;

            DragAndDrop.onItemAdd -= AddItemFromArmorInventory;
            DragAndDrop.onItemDrop -= DropItem;
            DragAndDrop.onArmorEquip -= EquipArmor;
            DragAndDrop.onArmorDrop -= DropArmor;
            DragAndDrop.onSwitch -= SwitchItem;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            DragAndDropEquipment.onItemAddFromArmor -= AddItemFromArmorInventory;
            DragAndDropEquipment.onItemDrop -= DropItem;
            DragAndDropEquipment.onArmorEquip -= EquipArmor;
            DragAndDropEquipment.onArmorDrop -= DropArmor;
            DragAndDropEquipment.onSwitch -= SwitchItem;
            DragAndDropEquipment.onWeaponryEquip -= EquipWeapon;
            DragAndDropEquipment.onWeaponryUnequip -= UnequipWeapon;
            DragAndDropEquipment.onItemAddFromWeaponry += AddItemFromWeaponryInventory;
        }
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

    void AddItemFromWeaponryInventory(ItemData itemData, int index)
    {
        allItems.Add(index, itemData);

        UpdateItemInventory();
        UpdateWeaponryInventory();
    }

    void DropItem(int index)
    {
        Item droppedItem = itemManager.GetItem(allItems[index].GetName());
        ItemData droppedItemData = droppedItem.GetItemData();
        Instantiate(droppedItem, transform.position, droppedItemData.GetDropRotation());

        allItems.Remove(index);

        UpdateItemInventory();
    }

    public void SwitchItem(int indexFrom, int indexTo)
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
        if (allItemsSlots == null) return;

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

    public int FindFirstEmptySlot()
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

    public override void SetItemInventory(Dictionary<int, ItemData> inventory)
    {
        base.SetItemInventory(inventory);
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

    public void SetArmorInventory(Dictionary<ArmorType, Armor> inventory)
    {
        armorItems = inventory;
        UpdateArmorInventory();
    }

    // Weapons-----------------------------------------------------------

    void EquipWeapon(Item item, HandSlot handSlot, int index)
    {
        if(equipment.EquipHand(item, handSlot))
        {
            allItems.Remove(index);
            UpdateWeaponryInventory();
            UpdateItemInventory();
        }
    }

    void UnequipWeapon(HandSlot handSlot)
    {
        equipment.UnequipHand(handSlot);
        UpdateWeaponryInventory();
        UpdateItemInventory();
    }

    void UpdateWeaponryInventory()
    {
        Image tempImage;
        ItemSlot tempItemData;
        for (int i = 0; i < 2; i++)
        {
            tempImage = weaponsSlots[i].GetComponent<Image>();
            tempItemData = weaponsSlots[i].GetComponent<ItemSlot>();

            if (equipment.handSlots[i] != null)
            {
                tempImage.sprite = equipment.handSlots[i].GetItemData().GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = equipment.handSlots[i].GetItemData();
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

    public Weapon GetEquippedWeaponData()
    {
        return equipment.GetEquippedWeaponData();
    }


}