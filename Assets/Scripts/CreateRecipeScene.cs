using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule { 
    public class CreateRecipeScene : MonoBehaviour {
        public GameObject[] places;
        int placeCount = 0;
        public string recipe;
        // Use this for initialization
        private void Start()
        {
            Recipe r = Recipe.LoadRecipe(recipe);
            UseActionList(r.GetActions());
        }

        void CreateObjectInScene(GameObject ingredient, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Instantiate(ingredient, places[placeCount].transform.position, ingredient.transform.rotation);
            }
            placeCount++;
        }

        void UseActionList(List<Action> actionList)
        {
            List<GameObject> f=new List<GameObject>();
            
            foreach(Action a in actionList)
            {
                
                
                if (a.GetInvolvedFood().GetPrev() == null)
                {
                    if (f.Count==0)
                    {
                        f.Add(GetComponent<FindPrefab>().GetPrefab(a.GetInvolvedFood().GetFoodIdentifier()));
                    }
                    else if (f.Contains(GetComponent<FindPrefab>().GetPrefab(a.GetInvolvedFood().GetFoodIdentifier())))
                    {
                        f.Add(GetComponent<FindPrefab>().GetPrefab(a.GetInvolvedFood().GetFoodIdentifier()));
                    }
                    else
                    {
                        CreateObjectInScene(f[0], f.Count);
                        f.Clear();
                        f.Add(GetComponent<FindPrefab>().GetPrefab(a.GetInvolvedFood().GetFoodIdentifier()));
                    }
                }
            }
            if (f != null)
            {
                CreateObjectInScene(f[0], f.Count);
                f.Clear();
            }
        }

    }
}