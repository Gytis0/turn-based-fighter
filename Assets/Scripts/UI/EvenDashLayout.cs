using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvenDashLayout : MonoBehaviour
{
    GridLayoutGroup layout;
    List<GameObject> dashes = new List<GameObject>();

    float maxValue;
    int interval = 10;
    int cellSizeX = 2;
    void Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        maxValue = GetComponentInParent<Slider>().maxValue;
        RectTransform rectTransform = GetComponent<RectTransform>();

        int sections = (int)(maxValue / interval);
        int dashCount = sections - 1;

        foreach (Transform dash in transform)
        {
            dashes.Add(dash.gameObject);
        }
       
        for (int i = 1; i <= dashes.Count; i++)
        {
            if (i <= dashCount) dashes[i - 1].SetActive(true);
            else dashes[i-1].SetActive(false);
        }

        layout.spacing = new Vector2((rectTransform.sizeDelta.x - (dashCount * cellSizeX)) / sections, 0);
        layout.cellSize = new Vector2(cellSizeX, rectTransform.sizeDelta.y);
    }
}
