using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPrefab : MonoBehaviour {

	public GameObject[] allPrefabs;

    public GameObject GetPrefab(string foodIdentifier)
    {
        foreach(GameObject o in allPrefabs)
        {
            if(!o.GetComponent<FoodStatus>())
            {
                continue;
            }

            if(o.GetComponent<FoodStatus>().foodIdentifier.Equals(foodIdentifier))
            {
                return o;
            }
        }

        return null;
    }
}
