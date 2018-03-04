using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder : MonoBehaviour {

    public FoodState foodState;
    public PseudoAction pseudoAction;

	// Use this for initialization
	void Start () {
        foodState.SetFood(new RecipeModule.Food("Tomato"));
        pseudoAction.SetAsChop(RecipeModule.Chop.PieceVolumeSize.Middle);   
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
