using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using cakeslice;

public class EquipmentTests
{
    [UnityTest]
    public IEnumerator EquipRightHand()
    {
        GameObject equipmentObject = new GameObject("Equipment");
        Equipment equipment = equipmentObject.AddComponent<Equipment>();
        GameObject rightHand = new GameObject("Right Hand");
        GameObject leftHand = new GameObject("Left Hand");

        equipment.rightHand = rightHand;
        equipment.leftHand = leftHand;

        GameObject itemObject = new GameObject("Item");
        itemObject.AddComponent<MeshRenderer>();
        itemObject.AddComponent<Rigidbody>();
        itemObject.AddComponent<BoxCollider>();
        Item item = itemObject.AddComponent<Item>();
        item.SetItemData(new Weapon(10, 5, 1));

        bool equipResult = equipment.EquipHand(item, HandSlot.RightHand);

        Assert.IsTrue(equipResult);
        Assert.IsNotNull(equipment.handSlots[(int)HandSlot.RightHand]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetEquippedWeapon()
    {
        int damage = 5;
        GameObject equipmentObject = new GameObject("Equipment");
        Equipment equipment = equipmentObject.AddComponent<Equipment>();
        GameObject rightHand = new GameObject("Right Hand");
        GameObject leftHand = new GameObject("Left Hand");

        equipment.rightHand = rightHand;
        equipment.leftHand = leftHand;

        GameObject itemObject = new GameObject("Item");
        itemObject.AddComponent<MeshRenderer>();
        itemObject.AddComponent<Rigidbody>();
        itemObject.AddComponent<BoxCollider>();
        Item item = itemObject.AddComponent<Item>();
        item.SetItemData(new Weapon(damage, 5, 1));

        equipment.EquipHand(item, HandSlot.RightHand);

        Assert.AreEqual(equipment.GetEquippedWeaponData().GetDamage(), damage);

        yield return null;
    }
}
