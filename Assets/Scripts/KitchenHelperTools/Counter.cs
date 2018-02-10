using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour {

    private string zero = "1111110";
    private string one = "0000110";
    private string two = "1011011";
    private string three = "1001111";
    private string four = "0100111";
    private string five = "1101101";
    private string six = "1111101";
    private string seven = "1000110";
    private string eight = "1111111";
    private string nine = "1101111";
    private Renderer[][] digitRefs;

    private bool isInitited = false;

    private string[] digitCodes;

    private int counterInSecs = 0;
    private int currentHours = -1;
    private int currentMinutes = -1;

    public Renderer[] digit_1; // hours
    public Renderer[] digit_2; // minutes 1
    public Renderer[] digit_3; // minutes 2
    public Renderer seconds;

    public Material blankMat;
    public Material activeMat;

    // Use this for initialization
    void Start () {
        digitCodes = new string[] { zero, one, two, three, four, five, six, seven, eight, nine };
        digitRefs = new Renderer[][] { digit_1, digit_2, digit_3 };
    }

    private void CounterUpdate()
    {
        int minutes = counterInSecs-- / 60;
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
        if(counterInSecs % 2 == 0)
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
        counterInSecs = seconds;

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
        StartCoroutine(InitiateCounter(counterInSecs));
        InvokeRepeating("CounterUpdate", 0, 1);
    }

    public void StopCounter()
    {
        isInitited = false;
        StopCoroutine("InitiateCounter");
        CancelInvoke("CounterUpdate");
    }

    private void SetDigit(int digitIndex, int num)
    {
        Renderer[] digitRef = digitRefs[digitIndex];

        string digitCode = digitCodes[num];

        int codeSeq = 0;
        foreach(char c in digitCode)
        {
            if(c == '1')
            {
                digitRef[codeSeq++].material = activeMat;
            }

            else
            {
                digitRef[codeSeq++].material = blankMat;
            }   
        }
    }
}