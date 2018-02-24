using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRecipeScene : MonoBehaviour {
    public GameObject[] places;
    public GameObject[] ingredients;
    // Use this for initialization
    void Start() {
        //getTheIngredients();
        int i = 0;
        for(int j = 0; j < ingredients.Length; j++)
        {
            CreateObjectInScene(ingredients[j], places[i], 2);
            i++;
            if(i > places.Length)
            {
                i = 0;
            }
        } 
	}

    void CreateObjectInScene(GameObject ingredient, GameObject place, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            Instantiate(ingredient, place.transform.position, ingredient.transform.rotation);
        }
    }


}
