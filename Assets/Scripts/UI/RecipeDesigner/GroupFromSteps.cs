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
}
