using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStateGroup : MonoBehaviour {
    public List<FoodState> foodStates;
    public string stateName;
    public FoodStateGroup clone = null;

    public void SetFoodStateGroup(List<FoodState> l)
    {
        this.foodStates = l;
    }

    public void SetStateName(string stateName)
    {
        this.stateName = stateName;
    }

    public string GetStateName()
    {
        return stateName;
    }

    public void Clone(FoodStateGroup fg)
    {
        this.foodStates = fg.foodStates;
        fg.clone = this;
    }
}