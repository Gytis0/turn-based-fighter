using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAlwaysLookIntoCamera : MonoBehaviour
{
    Camera cam;

    Vector3 cameraVector;
    Quaternion lookRotation;

    void Awake()
    {
        cam = Camera.main;
    }


    void Update()
    {
        cameraVector = (cam.transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(cameraVector);

        transform.rotation = lookRotation;
    }
}
