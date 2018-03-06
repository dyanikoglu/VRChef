using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState : MonoBehaviour {
    public RecipeModule.Food recipeFoodRef;
    public FoodState clone = null;

    public void SetFood(RecipeModule.Food recipeFoodRef)
    {
        this.recipeFoodRef = recipeFoodRef;
    }

    public RecipeModule.Food GetFood()
    {
        return recipeFoodRef;
    }

    public void Clone(FoodState fs)
    {
        this.recipeFoodRef = fs.GetFood();
        fs.clone = this;
    }
}