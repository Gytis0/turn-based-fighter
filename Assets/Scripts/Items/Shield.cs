using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield", menuName = "Items/Shield")]
public class Shield : ItemData
{
    [SerializeField] protected int durability;
    [SerializeField] protected List<ShieldActions> actions;

    public List<ShieldActions> GetShieldActions() { return actions; }

}