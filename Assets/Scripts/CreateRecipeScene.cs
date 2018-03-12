using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule { 
    public class CreateRecipeScene : MonoBehaviour
    {
        public SimulationController simulationController;
        public GameObject[] places;
        int placeCount = 0;
        public string recipe;
        // Use this for initialization
        private void Start()
        {
            Recipe r = Recipe.LoadRecipe(recipe);
            simulationController.SetRecipeToControl(r);
            UseActionList(r.GetActions());
        }

        void CreateObjectInScene(GameObject ingredient, int quantity)
        {
            for (int i = 0; i < quantity*2; i++)
            {
                GameObject createdFood = Instantiate(ingredient, places[placeCount].transform.position, ingredient.transform.rotation);
                createdFood.GetComponent<FoodCharacteristic>().OperationDone += simulationController.OnOperationDone;
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
                        if (prefab.transform.GetChild(0).GetComponent<CanBeChopped>()==null)
                        {
                            prefab.transform.GetChild(0).gameObject.AddComponent<CanBeChopped>();
                        }
                        prefab.transform.GetChild(0).GetComponent<CanBeChopped>().maximumChopCount = chop.GetRequiredPieceCount();
                        
                    }
                    else
                    {
                        if (prefab.GetComponent<CanBeChopped>()==null)
                        {
                            prefab.AddComponent<CanBeChopped>();
                        }
                        prefab.GetComponent<CanBeChopped>().maximumChopCount = chop.GetRequiredPieceCount();
                    }
                }
                else if (a.GetActionType().ToString().Equals("Peel"))
                {
                    Peel peel = (Peel)a;
                    if (prefab.GetComponent<CanBePeeled>() == null)
                    {
                        prefab.AddComponent<CanBePeeled>();
                    }
                    //add required parameters
                }
                else if (a.GetActionType().ToString().Equals("Cook"))
                {
                    Cook cook = (Cook)a;
                    if (prefab.GetComponent<CanBeCooked>() == null)
                    {
                        prefab.AddComponent<CanBeCooked>();
                    }
                    prefab.GetComponent<CanBeCooked>().requiredCookHeat = (int)cook.GetRequiredHeat();
                    prefab.GetComponent<CanBeCooked>().requiredCookTime = (int)cook.GetRequiredTime();
                    //add cooktype parameter
                }
                else if (a.GetActionType().ToString().Equals("Fry"))
                {
                    Fry fry = (Fry)a;
                    if (prefab.GetComponent<CanBePeeled>() == null)
                    {
                        prefab.AddComponent<CanBePeeled>();
                    }
                    //add required parameters
                }
                else if (a.GetActionType().ToString().Equals("Squeeze"))
                {
                    if (prefab.GetComponent<CanBeSqueezed>() == null)
                    {
                        prefab.AddComponent<CanBeSqueezed>();
                    }
                }
                else if (a.GetActionType().ToString().Equals("Smash"))
                {
                    if (prefab.GetComponent<CanBeSmashed>() == null)
                    {
                        prefab.AddComponent<CanBeSmashed>();
                    }
                }
                else if (a.GetActionType().ToString().Equals("Break"))
                {
                   
                }
                else if (a.GetActionType().ToString().Equals("Boil"))
                {
                    Boil boil = (Boil)a;
                    if (prefab.GetComponent<CanBeBoiled>() == null)
                    {
                        prefab.AddComponent<CanBeBoiled>();
                    }
                    //add required parameters
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