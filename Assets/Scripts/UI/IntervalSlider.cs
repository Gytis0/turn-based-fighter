using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntervalSlider : MonoBehaviour
{
    Slider slider;
    RectTransform fillArea;
    float sliderSize;
    public float min, max;

    public void SetInterval(float min, float max)
    {
        this.min = min;
        this.max = max;

        slider = GetComponent<Slider>();
        fillArea = transform.Find("Fill Area").GetComponent<RectTransform>();
        sliderSize = slider.GetComponent<RectTransform>().sizeDelta.x;

        fillArea.offsetMin = new Vector2(sliderSize * min / 100f, fillArea.offsetMin.y);
        fillArea.offsetMax = new Vector2(((sliderSize * min / 100f) - 10), fillArea.offsetMax.y);

        slider.maxValue = 100;
        slider.value = max - min;
    }

    public void AlterInterval(float change)
    {
        min = Mathf.Clamp(min + change, 0, 100);
        max = Mathf.Clamp(max + change, 0, 100);

        SetInterval(min, max);
    }
}
