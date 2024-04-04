using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{ 

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<HumanoidProperties>().AlterHealth(-5);

            Debug.Log("Dealing damage.");
        }
    }
}
