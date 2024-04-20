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
    float balance;
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
            balance = 1 / Time.timeScale;
            remainingTime -= Time.deltaTime * 10;
            timerText.SetText(((int) remainingTime).ToString());
        }
    }

    public void EnableTimer(float seconds) {
        Debug.Log("Enabling timer");

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
