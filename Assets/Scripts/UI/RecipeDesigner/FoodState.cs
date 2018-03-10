using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState : MonoBehaviour {
    public string stateName = "";
    public FoodState clone = null;

    public void SetStateName(string stateName)
    {
        this.stateName = stateName;
    }

    public string GetStateName()
    {
        return stateName;
    }

    public void Clone(FoodState fs)
    {
        this.stateName = fs.GetStateName();
        fs.clone = this;
    }
}