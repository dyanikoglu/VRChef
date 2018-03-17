using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionParameter : MonoBehaviour {
    public Slider sliderRef;
    public Text headerRef;
    private int paramIndex = 0;

    public int GetParamIndex() {
        return paramIndex;
    }

    public void SetParamIndex(int i)
    {
        paramIndex = i;
    }

    public int GetValue()
    {
        return (int)sliderRef.value;
    }
}
