using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBorder : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    public void Color()
    {
        transform.parent.GetComponent<Image>().color = selectedColor;
    }
}
