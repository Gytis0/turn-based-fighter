using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class Weapon : ItemData
{
    public Weapon(int damage, int speed, int durability)
    {
        this.damage = damage;
        this.speed = speed;
        this.durability = durability;
    }

    public Weapon(int damage)
    {
        this.damage = damage;
    }
    public Weapon(Weapon weapon) : base(weapon)
    {

        damage = weapon.damage;
        speed = weapon.speed;
        durability = weapon.durability;
        damageType = weapon.damageType;
        weaponType = weapon.weaponType;
        actions = weapon.actions;
    }

    [SerializeField] protected int damage, speed, durability;
    [SerializeField] DamageTypes damageType;
    [SerializeField] WeaponType weaponType;
    [SerializeField] protected List<ActionName> actions;

    protected override ItemType itemType {  get { return ItemType.Weapon; } }

    public int GetDamage() { return damage; }
    public int GetSpeed() { return speed; }
    public int GetDurability() { return durability; }
    public DamageTypes GetDamageType() { return damageType; }
    public WeaponType GetWeaponType() { return weaponType; }
    public void AlterDurability(int amount) { durability += amount; }
    public List<ActionName> GetWeaponActions() { return actions; }
}