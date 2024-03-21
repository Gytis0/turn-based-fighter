using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStatic : MonoBehaviour
{
    Transform cam, controller;
    RectTransform rectTransform;

    private void Awake()
    {
        cam = Camera.main.transform;
        controller = cam.transform.parent;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    { 
        rectTransform.rotation = Quaternion.Euler(cam.localRotation.eulerAngles + controller.localRotation.eulerAngles);
    }
}
