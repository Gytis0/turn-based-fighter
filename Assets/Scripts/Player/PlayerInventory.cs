using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            slots.Add(t);
        }

        inputActions = new InputController();
        inputActions.Enable();

        //inputActions.FindAction("G").performed += x => DropItem(FindFirstItem());
    }

    private void OnEnable()
    {
        Interactable.onPickUp += AddItem;
    }

    private void OnDisable()
    {
        Interactable.onPickUp -= AddItem;
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

    void UpdateInventoryUI()
    {
        Image tempImage;
        for (int i = 0; i < slots.Count; i++)
        {
            tempImage = slots[i].GetComponent<Image>();

            if (items.ContainsKey(i))
            {
                tempImage.sprite = items[i].GetItemData().GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                tempImage.sprite = emptySlot;
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 0.5f);
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

    int FindFirstItem()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (items.ContainsKey(i))
            {
                return i;
            }
        }
        return -1;
    }
}