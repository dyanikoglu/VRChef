using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGroupState : MonoBehaviour {
    public List<RecipeModule.Food> recipeFoods;
    public FoodGroupState clone = null;

    public void SetFoodGroup(List<RecipeModule.Food> l)
    {
        this.recipeFoods = l;
    }

    public void Clone(FoodGroupState fg)
    {
        this.recipeFoods = fg.recipeFoods;
        fg.clone = this;
    }
}