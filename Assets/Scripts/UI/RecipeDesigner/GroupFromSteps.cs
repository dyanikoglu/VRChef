using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A dummy script to just store a reference to vertical grouping line.
public class GroupFromSteps : MonoBehaviour {
    public RectTransform verticalLineRef;
    public GameObject foodGroupZoneRef;

    public FoodGroup GetFoodGroup()
    {
        return foodGroupZoneRef.GetComponentInChildren<FoodGroup>();
    }

    public void SetFoodGroup(List<FoodState> l)
    {
        List<RecipeModule.Food> foodList = new List<RecipeModule.Food>();
        foreach(FoodState fs in l)
        {
            foodList.Add(fs.GetFood());
        }
        GetFoodGroup().SetFoodGroup(foodList);
    }
}
