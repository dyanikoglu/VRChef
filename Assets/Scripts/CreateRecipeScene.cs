using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule { 
    public class CreateRecipeScene : MonoBehaviour
    {
        public SimulationController simulationController;
        public GameObject[] places;
        int placeCount = 0;
        public string recipeString;
        private Recipe recipe;
        // Use this for initialization
        private void Start()
        {
            Recipe r = Recipe.LoadRecipe(recipeString);
            SetRecipe(r);
            simulationController.SetRecipeToControl(r);
            UseActionList(r.GetActions());
        }

        public void SetRecipe(Recipe _recipe)
        {
            this.recipe = _recipe;
        }

        public Recipe GetRecipe()
        {
            return this.recipe;
        }

        void CreateObjectInScene(GameObject ingredient, int quantity)
        {
            float increaseZ = -0.2f;
            float increaseX = -0.1f;
            int rowCount = 0;
            for (int i = 0; i < quantity*2; i++)
            {
                Vector3 pos = places[placeCount].transform.position;
                pos.z = pos.z + increaseZ;
                pos.x = pos.x + increaseX;
                GameObject createdFood = Instantiate(ingredient, pos, ingredient.transform.rotation);
                if (rowCount == 3)
                {
                    rowCount = 0;
                    increaseX = increaseX + 0.1f;
                    increaseZ = -0.2f;
                }
                else
                {
                    rowCount++;
                    increaseZ = increaseZ + 0.1f;
                }
                
                createdFood.GetComponent<FoodStatus>().OperationDone += simulationController.OnOperationDone;
            }
            placeCount++;
        }

        void UseActionList(List<Action> actionList)
        {
            Food b;
            foreach (Action a in actionList)
            {
                if(a.GetInvolvedFood().GetPrev() == null)
                {
                    b = a.GetInvolvedFood();
                }
                else
                {
                    b = a.GetInvolvedFood();
                    while(b.GetPrev()!=null)
                    {
                        b = b.GetPrev();
                    }
                }
                GameObject prefab = GetComponent<FindPrefab>().GetPrefab(b.GetFoodIdentifier());
                if (a.GetActionType().ToString().Equals("Chop"))
                {
                    Chop chop = (Chop)a;
                    if (prefab.GetComponent<CanBePeeled>())
                    {
                        /* if (prefab.transform.GetChild(0).GetComponent<CanBeChopped>()==null)
                        {
                            prefab.transform.GetChild(0).gameObject.AddComponent<CanBeChopped>();
                        }*/
                        prefab.transform.GetChild(0).GetComponent<CanBeChopped>().maximumChopCount = chop.GetRequiredPieceCount();
                        
                    }
                    else
                    {
                        /*if (prefab.GetComponent<CanBeChopped>()==null)
                        {
                            prefab.AddComponent<CanBeChopped>();
                        }*/
                        prefab.GetComponent<CanBeChopped>().maximumChopCount = chop.GetRequiredPieceCount();
                    }
                }
                /*else if (a.GetActionType().ToString().Equals("Peel"))
                {
                    Peel peel = (Peel)a;
                    if (prefab.GetComponent<CanBePeeled>() == null)
                    {
                        prefab.AddComponent<CanBePeeled>();
                    }
                }*/
                else if (a.GetActionType().ToString().Equals("Cook"))
                {
                    Cook cook = (Cook)a;
                    if (prefab.GetComponent<CanBePeeled>())
                    {
                        /*if (prefab.transform.GetChild(0).GetComponent<CanBeCooked>() == null)
                        {
                            prefab.transform.GetChild(0).gameObject.AddComponent<CanBeCooked>();
                        }*/
                        prefab.transform.GetChild(0).GetComponent<CanBeCooked>().requiredCookHeat = (int)cook.GetRequiredHeat();
                        prefab.transform.GetChild(0).GetComponent<CanBeCooked>().requiredCookTime = (int)cook.GetRequiredTime();


                    }
                    else
                    {
                        /*if (prefab.GetComponent<CanBeCooked>() == null)
                        {
                            prefab.AddComponent<CanBeCooked>();
                        }*/
                        prefab.GetComponent<CanBeCooked>().requiredCookHeat = (int)cook.GetRequiredHeat();
                        prefab.GetComponent<CanBeCooked>().requiredCookTime = (int)cook.GetRequiredTime();
                    }
                  
                }
                else if (a.GetActionType().ToString().Equals("Fry"))
                {
                    Fry fry = (Fry)a;
                    if (prefab.GetComponent<CanBePeeled>())
                    {
                        /*if (prefab.transform.GetChild(0).GetComponent<CanBeFried>() == null)
                        {
                            prefab.transform.GetChild(0).gameObject.AddComponent<CanBeFried>();
                        }*/
                        prefab.transform.GetChild(0).GetComponent<CanBeFried>().fryingTimeInSeconds = fry.GetRequiredTime();

                    }
                    else
                    {
                        /*if (prefab.GetComponent<CanBeFried>() == null)
                        {
                            prefab.AddComponent<CanBeFried>();
                        }*/
                        prefab.GetComponent<CanBeFried>().fryingTimeInSeconds = fry.GetRequiredTime();
                    }
                }
                /*else if (a.GetActionType().ToString().Equals("Squeeze"))
                {
                    if (prefab.GetComponent<CanBeSqueezed>() == null)
                    {
                        prefab.AddComponent<CanBeSqueezed>();
                    }
                }
                else if (a.GetActionType().ToString().Equals("Smash"))
                {
                    continue;
                }
                else if (a.GetActionType().ToString().Equals("Break"))
                {
                   //control the if break script is added to prefab
                }*/
                else if (a.GetActionType().ToString().Equals("Boil"))
                {
                    Boil boil = (Boil)a;
                    /*if (prefab.GetComponent<CanBeBoiled>() == null)
                    {
                        prefab.AddComponent<CanBeBoiled>();
                    }*/
                    prefab.GetComponent<CanBeBoiled>().requiredBoilingTime = (int)boil.GetRequiredTime();
                }
            }
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