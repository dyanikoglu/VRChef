using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A dummy script to just store a reference to vertical grouping line & FoodGroup object
public class GroupFromSteps : MonoBehaviour {
    public RectTransform verticalLineRef;
    public GameObject foodGroupZoneRef;
    public RecipeManager recipeManager;

    // Outputs of these steps are gathered together and created this food group
    public List<Step> boundedSteps;

    public FoodStateGroup GetFoodStateGroup()
    {
        return foodGroupZoneRef.GetComponentInChildren<FoodStateGroup>();
    }

    public bool CheckForMissingSteps()
    {
        foreach(Step s in boundedSteps)
        {
            if(s == null)
            {
                return true;
            }
        }

        return false;
    }

    public int GetLastStepNumber()
    {
        int maxNo = -1;
        foreach(Step s in boundedSteps)
        {
            if(s.GetStepNumber() > maxNo)
            {
                maxNo = s.GetStepNumber();
            }
        }

        return maxNo;
    }

    public void RemoveGroup()
    {
        recipeManager.RemoveGroup(GetFoodStateGroup());
    }
}
