using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionParameter : MonoBehaviour {
    public Slider sliderRef;
    public Text headerRef;
    private int paramIndex = 0;
    public string paramName = "";

    public int GetParamIndex() {
        return paramIndex;
    }

    public void SetParamIndex(int i)
    {
        paramIndex = i;
    }

    public void UpdateHeader()
    {
        headerRef.text = paramName + ": " + sliderRef.value;
    }

    public void Update()
    {
        UpdateHeader();
    }

    public int GetValue()
    {
        return (int)sliderRef.value;
    }
}
