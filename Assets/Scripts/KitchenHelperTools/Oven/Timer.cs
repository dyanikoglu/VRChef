using System.Collections;
using UnityEngine;
using VRTK;

public class Timer : DigitalScreen {
    private const int MAX_VALUE = 35940;

    private int currentHours = -1;
    private int currentMinutes = -1;
    
    public Renderer seconds;
    public GameObject increaseButtonRef;
    public GameObject decreaseButtonRef;
   

    protected override void Start()
    {
        base.Start();

        increaseButtonRef.GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += TimerIncrement;
        decreaseButtonRef.GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += TimerDecrement;
    }

    private void TimerIncrement(object sender, InteractableObjectEventArgs e)
    {
        StopTimer();

        if (val + 60 <= MAX_VALUE)
        {
            SetTimer(val + 60);
        }

        else
        {
            SetTimer(MAX_VALUE);
        }
    }

    private void TimerDecrement(object sender, InteractableObjectEventArgs e)
    {
        StopTimer();

        if (val - 60 >= 0)
        {
            SetTimer(val - 60);
        }

        else
        {
            SetTimer(0);
        }
    }

    private void TimerUpdate()
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

    private IEnumerator InitiateTimer(int timerInSecs)
    {
        yield return new WaitForSeconds(timerInSecs);
        StopTimer();
    }

    public void SetTimer(int seconds)
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

    public void StartTimer()
    {
        StartCoroutine(InitiateTimer(val));
        InvokeRepeating("TimerUpdate", 0, 1);
    }

    public void StopTimer()
    {
        StopCoroutine("InitiateTimer");
        CancelInvoke("TimerUpdate");
    }
}