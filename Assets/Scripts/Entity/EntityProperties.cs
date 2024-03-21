using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityProperties : MonoBehaviour
{
    protected EntityOverhead entityOverhead;
   
    protected int health = 100;

    protected void Awake()
    {
        entityOverhead = GetComponentInChildren<EntityOverhead>();
    }

    public void AlterHealth(int change)
    {
        health += change;
        entityOverhead.UpdateHealthSlider(health);
    }
}
