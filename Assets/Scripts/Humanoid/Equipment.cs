using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Equipment : MonoBehaviour
{
    Armor[] armors;
    Item[] handSlots;

    // References
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject leftHand;

    // Start is called before the first frame update
    void Start()
    {
        int count = System.Enum.GetNames(typeof(ArmorType)).Length;
        armors = new Armor[count];

        count = System.Enum.GetNames(typeof (HandSlot)).Length;
        handSlots = new Item[count];

    }

    public void EquipHand(Item item, HandSlot slot)
    {
        handSlots[(int)slot] = item;
        if(slot == HandSlot.RightHand) { item.transform.parent = rightHand.transform; }
        else if(slot == HandSlot.LeftHand) { item.transform.parent = leftHand.transform; }
        item.AppearInHand(slot);
    }

    public Item UnequipHand(HandSlot slot)
    {
        Item uneqquipedItem = handSlots[(int)slot];
        handSlots[(int)slot] = null;
        return uneqquipedItem;
    }

    public bool IsHandEquipped(HandSlot slot)
    {
        if (handSlots[(int)slot] == null) return false;
        else return true;
    }

    public Weapon GetEquippedWeapon()
    {
        if (handSlots[0] != null && handSlots[0].GetItemData().GetType() == typeof(Weapon))
        {
            return (Weapon)handSlots[0].GetItemData();
        }
        else if (handSlots[1] != null && handSlots[1].GetItemData().GetType() == typeof(Weapon))
        {
            return (Weapon)handSlots[1].GetItemData();
        }
        else return null;
    }
}