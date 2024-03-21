using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : HumanoidMovementController
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("enemy awake");
    }
}
