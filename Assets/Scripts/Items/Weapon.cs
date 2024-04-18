using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class Weapon : ItemData
{
    [SerializeField] protected int damage, speed, durability;
    [SerializeField] DamageTypes damageType;
    [SerializeField] WeaponType weaponType;
    [SerializeField] protected List<WeaponActions> actions;

    protected override ItemType itemType {  get { return ItemType.Weapon; } }

    public int GetDamage() { return damage; }
    public int GetSpeed() { return speed; }
    public int GetDurability() { return durability; }
    public DamageTypes GetDamageType() { return damageType; }
    public WeaponType GetWeaponType() { return weaponType; }
    public void AlterDurability(int amount) { durability -= amount; }
    public List<WeaponActions> GetWeaponActions() { return actions; }
}