using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : HumanoidProperties
{
    public void SetStats(int[] points)
    {
        maxHealth = points[0] * 20;
        maxStamina = points[1] * 20;
        maxComposure = points[2] * 20;
        intelligence = points[3];
    }
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
