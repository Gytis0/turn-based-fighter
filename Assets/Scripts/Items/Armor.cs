using System.Collections.Generic;
using UnityEngine;
using System;

public class Armor : ItemData
{
    [SerializeField]
    protected int armorPoints, durability, basicResistance;

    [SerializeField]
    protected List<Tuple<DamageTypes, int>> resistances = new();

    [SerializeField]
    string serializedResistances;

    [SerializeField]
    ArmorType type;
}
