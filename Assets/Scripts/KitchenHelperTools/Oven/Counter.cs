using System.Collections;
using UnityEngine;

public class Counter : DigitalScreen {
    private bool isInitited = false;
    private int currentHours = -1;
    private int currentMinutes = -1;

    public Renderer seconds;

    private void CounterUpdate()
    {
        int minutes = val-- / 60;
        int hours = minutes / 60;
        minutes = minutes % 60;

        // Set hour digit
        if(hours != currentHours)
        {
            SetDigit(0, hours);
            currentHours = hours;
        }

        // Set minute digit
        if(minutes != currentMinutes)
        {
            int firstDigit = minutes / 10;
            int secondDigit = minutes % 10;
            SetDigit(1, firstDigit);
            SetDigit(2, secondDigit);
            currentMinutes = minutes;
        }

        // Set second indicator
        if(val % 2 == 0)
        {
            seconds.material = activeMat;
        }
        else
        {
            seconds.material = blankMat;
        }
    }

    private IEnumerator InitiateCounter(int counterInSecs)
    {
        yield return new WaitForSeconds(counterInSecs);
        StopCounter();
    }

    public void SetCounter(int seconds)
    {
        val = seconds;

        int minutes = seconds / 60;
        int hours = minutes / 60;
        minutes = minutes % 60;

        // Set hour digit
        SetDigit(0, hours);
        currentHours = hours;

        // Set minute digit
        int firstDigit = minutes / 10;
        int secondDigit = minutes % 10;
        SetDigit(1, firstDigit);
        SetDigit(2, secondDigit);
        currentMinutes = minutes;
    }

    public void StartCounter()
    {
        isInitited = true;
        StartCoroutine(InitiateCounter(val));
        InvokeRepeating("CounterUpdate", 0, 1);
    }

    public void StopCounter()
    {
        isInitited = false;
        StopCoroutine("InitiateCounter");
        CancelInvoke("CounterUpdate");
    }
}