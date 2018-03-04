using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState : MonoBehaviour {
    private RecipeModule.Food recipeFoodRef;

    public void SetFood(RecipeModule.Food recipeFoodRef)
    {
        this.recipeFoodRef = recipeFoodRef;
    }

    public RecipeModule.Food GetFood()
    {
        return recipeFoodRef;
    }

}
