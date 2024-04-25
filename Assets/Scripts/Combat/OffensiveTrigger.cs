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

    void Start()
    {
        weapon = (Weapon)GetComponent<Item>().GetItemData();
        collider = GetComponent<Collider>();
        collider.isTrigger = false;
    }

    public void SetAction(Action action)
    {
        this.action = action;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player Hitbox") || other.CompareTag("Enemy Hitbox")) 
        {
            onEnemyHit(weapon, action, other.CompareTag("Player"));
        }
    }
}
