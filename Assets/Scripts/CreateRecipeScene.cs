using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRecipeScene : MonoBehaviour {
    public GameObject[] places;
    public GameObject[] ingredients;
    // Use this for initialization
    void Start() {
        //getTheIngredients();
        //for each ingredient call the method for placing the ingredients.
        Instantiate(ingredients[0], places[0].transform.position, ingredients[0].transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
