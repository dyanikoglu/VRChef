using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGroup : MonoBehaviour {
    public List<RecipeModule.Food> recipeFoods;
    

    public void SetFoodGroup(List<RecipeModule.Food> l)
    {
        this.recipeFoods = l;
    }
}