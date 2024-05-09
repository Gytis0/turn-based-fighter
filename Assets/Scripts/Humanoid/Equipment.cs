using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Weapon weapon = new Weapon((Weapon)item.GetItemData());

        Item itemToEquip = Instantiate(item);
        itemToEquip.SetItemData(weapon);

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
        Shield shield = (Shield)item.GetItemData();

        Item itemToEquip = Instantiate(item);
        itemToEquip.SetItemData(shield);

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

    void Drop(HandSlot slot)
    {
        Item item = new Item(handSlots[(int)slot], handSlots[(int)slot].GetItemData());
        UnequipHand(slot);

        item.AppearInWorld(transform.position);
    }

    public bool IsHandEquipped(HandSlot slot)
    {
        if (handSlots[(int)slot] == null) return false;
        else return true;
    }

    public Weapon GetEquippedWeaponData()
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

    public GameObject GetEquippedWeaponObject()
    {
        if (handSlots[0] != null && handSlots[0].GetItemData().GetType() == typeof(Weapon))
        {
            return leftHand.transform.GetChild(0).gameObject;
        }
        else if (handSlots[1] != null && handSlots[1].GetItemData().GetType() == typeof(Weapon))
        {
            return rightHand.transform.GetChild(0).gameObject;
        }
        else return null;
    }

    public Shield GetEquippedShieldData()
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
        foreach(ArmorType key in armors.Keys)
        {
            armorItems.Add(key, new Armor(armors[key]));
        }
    }

    public float GetWeaponWeight()
    {
        if(GetEquippedWeaponData() == null) return 0f;
        return GetEquippedWeaponData().GetWeight();
    }

    public float GetShieldWeight()
    {
        if(GetEquippedShieldData() == null) return 0f;
        return GetEquippedShieldData().GetWeight();
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
        if (GetEquippedShieldData() == null) return null;
        return GetEquippedShieldData().GetShieldActions();
    }

    public List<ActionName> GetWeaponActions()
    {
        if (GetEquippedWeaponData() == null) return null;
        return GetEquippedWeaponData().GetWeaponActions();
    }

    public bool IsTwoHanded()
    {
        return GetEquippedWeaponData().GetWeaponType() == WeaponType.TwoHanded;
    }

    public bool IsLeftHanded()
    {
        if (handSlots[0] != null && handSlots[0].GetItemData().GetType() == typeof(Weapon))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DamageShield(Weapon weapon)
    {
        weapon.AlterDurability(-1);
        Shield shield = GetEquippedShieldData();
        if (weapon.GetDamage() > 30) shield.Damage(2);
        else shield.Damage(1);

        if(shield.GetDurability() <= 0)
        {
            if (handSlots[0] != null && handSlots[0].GetItemData().GetType() == typeof(Shield))
            {
                Drop(HandSlot.LeftHand);
            }
            else if (handSlots[1] != null && handSlots[1].GetItemData().GetType() == typeof(Shield))
            {
                Drop(HandSlot.RightHand);
            }
        }
    }

    public void DamageArmors(Weapon weapon)
    {
        weapon.AlterDurability(-1);
        int damage;
        if (weapon.GetDamage() > 30) damage = 2;
        else damage = 1;

        foreach (Armor armor in armorItems.Values)
        {
            armor.AlterDurability(-damage);
        }
    }

    public int GetArmorDamageReduction(DamageTypes damageType)
    {
        int sum = 0;
        foreach(Armor armor in armorItems.Values)
        {
            sum += armor.GetArmorPoints();
            sum += armor.GetResistance(damageType);
        }
        return sum;
    }
}