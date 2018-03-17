using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPopup : MonoBehaviour {
    private PseudoAction _pseudoActionRef;

    public Text headerRef;
    public GameObject parametersRef;
    public GameObject parameterPrefab;
    public int paramStartY = 135;
    public int paramIntervalY = 70;
    
    // Import all parameter values into PseudoAction object
    public void SaveValues()
    {
        List<int> tempList = new List<int>(parametersRef.transform.childCount);
        foreach(ActionParameter ap in parametersRef.transform.GetComponentsInChildren<ActionParameter>())
        {
            tempList.Insert(ap.GetParamIndex(), ap.GetValue());
        }

        _pseudoActionRef.SetParameterValues(tempList);
    }

    public PseudoAction GetPseudoActionRef()
    {
        return _pseudoActionRef;
    }

    public void SetPseudoActionRef(PseudoAction pa)
    {
        _pseudoActionRef = pa;
    }
}
