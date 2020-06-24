using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    [SerializeField] Text Timer;

    float timer;
    float seconds;
    float milliseconds;

    bool start;
    // Start is called before the first frame update
    void Start()
    {
        start = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StopWatchCalcul();
    }

    void StopWatchCalcul()
    {
        if (start)
        {
            timer += Time.deltaTime;
            milliseconds = (int)((timer * 100) % 100);
            seconds = (int)(timer % 60);
            Timer.text = seconds.ToString("00") + "." + milliseconds.ToString("00");
        }
    }

    public void startTimer()
    {
        start = true;
    }

    public void stopTimer()
    {
        start = false;
    }

    public void resetTimer()
    {
        start = false;
        timer = 0;
        Timer.text = "00.00";
    }
}