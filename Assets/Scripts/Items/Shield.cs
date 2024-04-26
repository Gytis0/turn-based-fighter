using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield", menuName = "Items/Shield")]
public class Shield : ItemData
{
    public Shield(Shield shield) : base(shield)
    {
        durability = shield.durability;
        actions = shield.actions;

        rightHandOffset = shield.rightHandOffset;
        leftHandOffset = shield.leftHandOffset;
        leftHandRotation = shield.leftHandRotation;
        rightHandOffset = shield.rightHandOffset;

        dropAngle = shield.dropAngle;

        itemName = shield.itemName;
        icon = shield.icon;
        weight = shield.weight;
    }
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