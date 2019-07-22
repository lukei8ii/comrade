using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int timeLeft = 60; //Seconds Overall
    public Text countdown; //UI Text Object

    bool keepCounting = true;

    void Start()
    {
        StartCoroutine("LoseTime");
        Time.timeScale = 1; //Just making sure that the timeScale is right
    }

    void Update()
    {
        countdown.text = ("" + timeLeft); //Showing the Score on the Canvas
    }

    //Simple Coroutine
    IEnumerator LoseTime()
    {
        while (keepCounting)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
            keepCounting = timeLeft > 0;

            if (!keepCounting)
                Messenger.Broadcast<Timer>(Events.OnTimeout, this);
        }
    }
}
