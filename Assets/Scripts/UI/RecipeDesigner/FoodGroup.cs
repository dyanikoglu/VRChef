using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGroup : MonoBehaviour {
    public List<RecipeModule.Food> recipeFoods;
    public FoodGroup clone = null;

    public void SetFoodGroup(List<RecipeModule.Food> l)
    {
        this.recipeFoods = l;
    }

    public void Clone(FoodGroup fg)
    {
        this.recipeFoods = fg.recipeFoods;
        fg.clone = this;
    }
}