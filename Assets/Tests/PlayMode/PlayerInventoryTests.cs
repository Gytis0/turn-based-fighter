using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerInvenotryTest
{
    [UnityTest]
    public IEnumerator InventorySwitchItems()
    {
        yield return null;
        
        GameObject testObject = new GameObject();
        
        PlayerInventory playerInventory = testObject.AddComponent<PlayerInventory>();
        Dictionary<int, ItemData> items = new Dictionary<int, ItemData>
        {
            { 0, ScriptableObject.CreateInstance<Weapon>() },
            {1, ScriptableObject.CreateInstance<Shield>() }
        };
        playerInventory.SetItemInventory(items);
        playerInventory.SwitchItem(0, 1);
        items = playerInventory.GetInventory();

        Assert.IsTrue(items[0].GetType() == typeof(Shield));
    }

    [UnityTest]
    public IEnumerator InventoryFindFirstEmptySlot()
    {
        yield return null;

        GameObject testObject = new GameObject();

        PlayerInventory playerInventory = testObject.AddComponent<PlayerInventory>();
        Dictionary<int, ItemData> items = new Dictionary<int, ItemData>
        {
            { 0, ScriptableObject.CreateInstance<Weapon>() },
            {2, ScriptableObject.CreateInstance<Shield>() }
        };
        playerInventory.SetItemInventory(items);

        Assert.AreEqual(1, playerInventory.FindFirstEmptySlot());
    }
}
