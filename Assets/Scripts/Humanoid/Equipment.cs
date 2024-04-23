using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using static UnityEditor.Progress;

public class Equipment : MonoBehaviour
{
    public Item[] handSlots;
    protected Dictionary<ArmorType, Armor> armorItems = new();

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
        // If the slot is taken, return
        if (handSlots[(int)slot] != null) return false;

        ItemData itemData = item.GetItemData();
        if (itemData.GetItemType() == ItemType.Weapon)
        {
            return EquipWeapon(item, slot);
        }
        else if (itemData.GetItemType() == ItemType.Shield)
        {
            return EquipShield(item, slot);
        }
        else return false;

        
    }

    bool EquipWeapon(Item item, HandSlot slot)
    {
        WeaponType weaponType = ((Weapon)item.GetItemData()).GetWeaponType();

        // If the weapon is two handed, and there's already an item, return
        if (weaponType == WeaponType.TwoHanded && (IsHandEquipped(HandSlot.LeftHand) || IsHandEquipped(HandSlot.RightHand))) return false;

        // If an equipped item is two handed, return
        if (handSlots[0] != null && handSlots[0].GetItemData().GetItemType() == ItemType.Weapon)
        {
            if (((Weapon)handSlots[0].GetItemData()).GetWeaponType() == WeaponType.TwoHanded) return false;
        }
        if (handSlots[1] != null && handSlots[1].GetItemData().GetItemType() == ItemType.Weapon)
        {
            if (((Weapon)handSlots[1].GetItemData()).GetWeaponType() == WeaponType.TwoHanded) return false;
        }


        Item itemToEquip = Instantiate(item);
        handSlots[(int)slot] = itemToEquip;
        if (slot == HandSlot.RightHand) { itemToEquip.transform.parent = rightHand.transform; }
        else if (slot == HandSlot.LeftHand) { itemToEquip.transform.parent = leftHand.transform; }
        itemToEquip.AppearInHand(slot);
        return true;
    }

    bool EquipShield(Item item, HandSlot slot)
    {
        // If an equipped item is two handed, return
        if (handSlots[0] != null && handSlots[0].GetItemData().GetItemType() == ItemType.Weapon )
        {
            if (((Weapon)handSlots[0].GetItemData()).GetWeaponType() == WeaponType.TwoHanded) return false;
        }
        if (handSlots[1] != null && handSlots[1].GetItemData().GetItemType() == ItemType.Weapon)
        {
            if (((Weapon)handSlots[1].GetItemData()).GetWeaponType() == WeaponType.TwoHanded) return false;
        }


        Item itemToEquip = Instantiate(item);
        handSlots[(int)slot] = itemToEquip;
        if (slot == HandSlot.RightHand) { itemToEquip.transform.parent = rightHand.transform; }
        else if (slot == HandSlot.LeftHand) { itemToEquip.transform.parent = leftHand.transform; }
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

    public Shield GetEquippedShield()
    {
        if (handSlots[0] != null && handSlots[0].GetItemData().GetType() == typeof(Shield))
        {
            return (Shield)handSlots[0].GetItemData();
        }
        else if (handSlots[1] != null && handSlots[1].GetItemData().GetType() == typeof(Shield))
        {
            return (Shield)handSlots[1].GetItemData();
        }
        else return null;
    }

    public void SetEquippedArmors(Dictionary<ArmorType, Armor> armors)
    {
        armorItems = armors;
    }

    public float GetWeaponWeight()
    {
        if(GetEquippedWeapon() == null) return 0f;
        return GetEquippedWeapon().GetWeight();
    }

    public float GetShieldWeight()
    {
        if(GetEquippedShield() == null) return 0f;
        return GetEquippedShield().GetWeight();
    }

    public float GetAllWeight()
    {
        float sum = 0f;
        sum += GetWeaponWeight();
        sum += GetShieldWeight();
        foreach(var obj in armorItems)
        {
            sum += obj.Value.GetWeight();
        }

        return sum;
    }

    public List<ActionName> GetShieldActions()
    {
        if (GetEquippedShield() == null) return null;
        return GetEquippedShield().GetShieldActions();
    }

    public List<ActionName> GetWeaponActions()
    {
        if (GetEquippedWeapon() == null) return null;
        return GetEquippedWeapon().GetWeaponActions();
    }
}