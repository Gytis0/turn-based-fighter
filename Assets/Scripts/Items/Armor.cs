using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
public class Armor : ItemData
{
    [SerializeField]
    protected int durability, armorPoints;

    [SerializeField]
    protected Resistances[] resistances;

    [SerializeField]
    ArmorType type;

    protected override ItemType itemType { get { return ItemType.Armor; } }

    public int GetDurability() { return durability; }
    public int GetArmorPoints() { return armorPoints; }
    public Resistances[] GetResistances() {  return resistances; }
    public ArmorType GetArmorType() {  return type; }
}

[System.Serializable]
public class Resistances
{
    public DamageTypes damageType;
    public int resistance;
}