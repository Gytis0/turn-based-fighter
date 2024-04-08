using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public enum armorSlot{
        HEAD,
        CHEST,
        LEGS,
        BOOTS
    }
    InputController inputActions;
    Dictionary<int, Item> equipmentItems = new();
    Dictionary<armorSlot, Item> armorItems = new();

    // References
    [SerializeField] Sprite emptySlot;

    Transform inventoryUIroot;
    List<Transform> slots = new();

    private void Awake()
    {
        inventoryUIroot = GameObject.FindGameObjectWithTag("Inventory").transform;

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
        Interactable.onEquip += Equip;
        DragAndDrop.onDrop += DropItem;
        DragAndDrop.onSwitch += SwitchItem;
    }

    private void OnDisable()
    {
        Interactable.onPickUp -= AddItem;
        Interactable.onEquip -= Equip;
        DragAndDrop.onDrop -= DropItem;
        DragAndDrop.onSwitch -= SwitchItem;
    }

    void AddItem(GameObject item)
    {
        if (equipmentItems.Count >= slots.Count) return;
        int index = FindFirstEmptySlot();

        equipmentItems.Add(index, item.GetComponent<Item>());
        item.transform.parent = transform;
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.SetActive(false);

        UpdateInventoryUI();
    }

    void Equip(GameObject item)
    {

    }
    void DropItem(int index)
    {
        GameObject droppedItem = ItemManager.GetItemObject(equipmentItems[index].GetItemData().GetName());
        ItemData droppedItemData = droppedItem.GetComponent<Item>().GetItemData();
        Instantiate(droppedItem, transform.position, droppedItemData.GetDropRotation());

        equipmentItems.Remove(index);
        
        UpdateInventoryUI();
    }

    void SwitchItem(int indexFrom, int indexTo)
    {
        Item temp;

        if (equipmentItems.TryGetValue(indexTo, out temp))
        {
            temp = equipmentItems[indexTo];
        }
        equipmentItems[indexTo] = equipmentItems[indexFrom];

        if(temp != null) 
        {
            equipmentItems[indexFrom] = temp;
        }
        else
        {
            equipmentItems.Remove(indexFrom);
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

            if (equipmentItems.ContainsKey(i))
            {
                tempImage.sprite = equipmentItems[i].GetItemData().GetIcon();
                if (!tempImage.hasBorder) tempImage.type = Image.Type.Simple;
                else tempImage.type = Image.Type.Sliced;
                tempImage.color = new Color(1, 1, 1, 1);

                tempItemData.itemData = equipmentItems[i].GetItemData();
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
            if (!equipmentItems.ContainsKey(i))
            {
                return i;
            }
        }
        return -1;
    }
}