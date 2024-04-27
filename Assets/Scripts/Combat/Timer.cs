using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void TimerDone();
    public event TimerDone onTimerDone;

    [SerializeField]
    TextMeshProUGUI timerText;

    float remainingTime = 0;
    bool timerEnabled = false;
    // Update is called once per frame
    void Update()
    {
        if (timerEnabled)
        {
            if (remainingTime <= 0)
            {
                DisableTimer();
                onTimerDone();
            }
           
            remainingTime -= Time.deltaTime;
            timerText.SetText(((int) remainingTime).ToString());
        }
    }

    public void EnableTimer(float seconds) {
        remainingTime = seconds;
        timerEnabled = true;
        timerText.gameObject.SetActive(true);
    }

    void DisableTimer()
    {
        Debug.Log("Disabling timer");

        timerEnabled = false;
        timerText.gameObject.SetActive(false);
    }
}
