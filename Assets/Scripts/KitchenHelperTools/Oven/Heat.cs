using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heat : DigitalScreen {
    public GameObject heatAdjuster;

	void Update () {
        // Map 0-180 -> 0-220
        SetHeat((int)((heatAdjuster.transform.rotation.eulerAngles.z * 220f) / 180f));
	}

    public void SetHeat(int value)
    {
        if (val != value)
        {
            int tempValue = value;
            val = value;

            int first = val / 100;
            tempValue = tempValue % 100;

            int second = tempValue / 10;
            tempValue = tempValue % 10;

            int third = tempValue;

            SetDigit(0, first);
            SetDigit(1, second);
            SetDigit(2, third);
        }
    }
}