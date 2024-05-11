using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using cakeslice;
using TMPro;

public class TimerTests
{
    [UnityTest]
    public IEnumerator TimeThreeSeconds()
    {
        bool timerDisabled = false;
        GameObject textObject = new GameObject("Text");
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        GameObject testObject = new GameObject("Timer");
        Timer timer = testObject.AddComponent<Timer>();
        timer.timerText = text;

        timer.onTimerDone += () => timerDisabled = true;

        timer.EnableTimer(3f);
        
        yield return new WaitForSeconds(3.1f);

        Assert.IsTrue(timerDisabled);
    }
}
