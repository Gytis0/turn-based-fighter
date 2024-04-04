using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : HumanoidProperties
{
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = PlayerPrefs.GetInt("healthPoints") * 20;
        maxStamina = PlayerPrefs.GetInt("staminaPoints") * 20;
        maxComposure = PlayerPrefs.GetInt("composurePoints") * 20;
        intelligence = PlayerPrefs.GetInt("intelligencePoints");
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
