using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
public class Armor : ItemData
{
    [SerializeField]
    protected int durability, baseResistance;

    [SerializeField]
    protected Resistances[] resistances;

    [SerializeField]
    ArmorType type;
}

[System.Serializable]
public class Resistances
{
    public DamageTypes damageType;
    public int resistance;
}