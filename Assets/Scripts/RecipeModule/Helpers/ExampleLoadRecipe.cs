using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class ExampleLoadRecipe : MonoBehaviour
    {
        public string recipeNameToLoad;
        
        void Start()
        {    
            Recipe r = Recipe.LoadRecipe(recipeNameToLoad);

            foreach(Food f in r.GetInitialFoods()) 
            {
                print(f.GetFoodIdentifier());
            }
        }
    }
}
