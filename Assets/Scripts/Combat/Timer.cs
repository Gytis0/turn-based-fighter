using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI timerText;

    float remainingTime = 0;
    bool timerEnabled = false;

    // Update is called once per frame
    void Update()
    {
        if (timerEnabled)
        {
            Debug.Log("Timer enbaled. Time: " + remainingTime);
            if (remainingTime <= 0)
            {
                disableTimer();
                Debug.Log("Timer disabled");

            }

            remainingTime -= Time.deltaTime;
            timerText.SetText(((int) remainingTime).ToString());
        }
    }

    public void enableTimer(float seconds) { 
        timerEnabled = true;
        remainingTime = seconds;
        timerText.gameObject.SetActive(true);
    }

    public void disableTimer()
    {
        timerEnabled = false;
        timerText.gameObject.SetActive(false);
    }
}
