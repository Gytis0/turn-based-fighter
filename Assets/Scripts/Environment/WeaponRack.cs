using UnityEngine;

public class WeaponRack : MonoBehaviour
{
    void Start()
    {
        foreach (Transform t in transform)
        {   
            t.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
