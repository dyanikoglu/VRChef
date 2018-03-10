using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState : MonoBehaviour {
    public FoodState clone = null;

    public void Clone(FoodState fs)
    {
        fs.clone = this;
    }
}