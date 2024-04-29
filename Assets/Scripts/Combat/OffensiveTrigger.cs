using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTrigger : MonoBehaviour
{
    public delegate void EnemyHit(Weapon weapon, Action action, bool isPlayer);
    public event EnemyHit onEnemyHit;
    
    Weapon weapon;
    Collider collider;
    Action action;
    public bool isThisPlayer;

    void Start()
    {
        Item item;
        if(TryGetComponent<Item>(out item))
        {
            weapon = (Weapon)(item.GetItemData());
        }
        else
        {
            weapon = new Weapon(5);
        }
        collider = GetComponent<Collider>();
        collider.isTrigger = false;
    }

    public void SetAction(Action action)
    {
        this.action = action;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player Hitbox") && !isThisPlayer) 
        {
            collider.isTrigger = false;
            onEnemyHit(weapon, action, !isThisPlayer);
        }
        else if (other.CompareTag("Enemy Hitbox") && isThisPlayer)
        {
            collider.isTrigger = false;
            onEnemyHit(weapon, action, !isThisPlayer);
        }
    }
}
