using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    InputController inputActions;
    Dictionary<int, Item> items = new();

    // References
    [SerializeField] Sprite emptySlot;
    Equipment equipment;

    Transform inventoryUIroot;
    List<Transform> slots = new();


    private void Awake()
    {
        inventoryUIroot = GameObject.FindGameObjectWithTag("Inventory").transform;
        equipment = transform.parent.GetComponent<Equipment>();

        foreach (Transform t in inventoryUIroot)
        {
            slots.Add(t.GetChild(0));
        }

        inputActions = new InputController();
        inputActions.Enable();

        //inputActions.FindAction("G").performed += x => DropItem(FindFirstItem());
    }

    private void OnEnable()
    {
        Interactable.onPickUp += AddItem;
        DragAndDrop.onDrop += DropItem;
        DragAndDrop.onSwitch += SwitchItem;
    }

    private void OnDisable()
    {
        Interactable.onPickUp -= AddItem;
        DragAndDrop.onDrop -= DropItem;
        DragAndDrop.onSwitch -= SwitchItem;
    }

    void AddItem(GameObject item)
    {
        if (items.Count >= slots.Count) return;
        int index = FindFirstEmptySlot();

        items.Add(index, item.GetComponent<Item>());
        item.transform.parent = transform;
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.SetActive(false);

        UpdateInventoryUI();
    }

    void DropItem(int index)
    {
        GameObject droppedItem = ItemManager.GetItemObject(items[index].GetItemData().GetName());
        Instantiate(droppedItem, transform.position, transform.rotation);

        items.Remove(index);
        
        UpdateInventoryUI();
    }

    void SwitchItem(int indexFrom, int indexTo)
    {
        Item temp;

        if (items.TryGetValue(indexTo, out temp))
        {
            temp = items[indexTo];
        }
        items[indexTo] = items[indexFrom];

        if(temp != null) 
        {
            items[indexFrom] = temp;
        }
        else
        {
            items.Remove(indexFrom);
        }

        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        Image tempImage;
        ItemSlot tempItemData;
        for (int i = 0; i < slots.Count; i++)
        {
            tempImage = slots[i].GetComponent<Image>();
            tempItemData = slots[i].GetComponent<ItemSlot>();

            if (items.ContainsKey(i))
            {
                tempImage.sprite = items[i].GetItemData().GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = items[i].GetItemData();
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
        for (int i = 0; i < slots.Count; i++)
        {
            if (!items.ContainsKey(i))
            {
                return i;
            }
        }
        return -1;
    }
}