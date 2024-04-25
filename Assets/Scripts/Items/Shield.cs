using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield", menuName = "Items/Shield")]
public class Shield : ItemData
{
    [SerializeField] protected int durability;
    [SerializeField] protected List<ActionName> actions;
    protected override ItemType itemType { get { return ItemType.Shield; } }

    public void Damage(int damage)
    {
        durability -= damage;
    }
    public List<ActionName> GetShieldActions() { return actions; }
    public int GetDurability() { return durability; }
}