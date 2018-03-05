using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState : MonoBehaviour {
    private RecipeModule.Food recipeFoodRef;
    private FoodState origin;

    private void Start()
    {
        origin = this;
    }

    public void SetFood(RecipeModule.Food recipeFoodRef)
    {
        this.recipeFoodRef = recipeFoodRef;
    }

    public void SetOrigin(FoodState origin)
    {
        this.origin = origin;
    }

    public FoodState GetOrigin()
    {
        return origin;
    }

    public RecipeModule.Food GetFood()
    {
        if(this.origin == null)
        {
            return recipeFoodRef;
        }

        return this.origin.GetFood();
    }
}
