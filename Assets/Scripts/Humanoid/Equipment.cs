using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Equipment : MonoBehaviour
{
    public Item[] handSlots;

    // References
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject leftHand;

    // Start is called before the first frame update
    void Awake()
    {
        int count = System.Enum.GetNames(typeof (HandSlot)).Length;
        handSlots = new Item[count];
    }
    

    public bool EquipHand(Item item, HandSlot slot)
    {
        if (handSlots[(int)slot] != null) return false;

        Item itemToEquip = Instantiate(item);
        handSlots[(int)slot] = itemToEquip;
        if(slot == HandSlot.RightHand) { itemToEquip.transform.parent = rightHand.transform; }
        else if(slot == HandSlot.LeftHand) { itemToEquip.transform.parent = leftHand.transform; }
        itemToEquip.AppearInHand(slot);
        return true;
    }

    public void UnequipHand(HandSlot slot)
    {
        Destroy(handSlots[(int)slot].gameObject);
        handSlots[(int)slot] = null;
    }

    public bool IsHandEquipped(HandSlot slot)
    {
        if (handSlots[(int)slot] == null) return false;
        else return true;
    }

    public bool IsBothHandsEquipped()
    {
        return IsHandEquipped(HandSlot.LeftHand) && IsHandEquipped(HandSlot.RightHand);
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