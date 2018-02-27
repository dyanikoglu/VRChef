using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule { 
    public class CreateRecipeScene : MonoBehaviour {
        public GameObject[] places;
        private bool isGetActionList = false;
        int placeCount = 0;
       // Use this for initialization
        void Update()
        {
            if (!isGetActionList)
            {
                isGetActionList = true;
                UseActionList(gameObject.GetComponent<Recipe>().GetActions());
            }
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
                        f.Add(a.GetInvolvedFood().GetPrefab());
                    }
                    else if (f.Contains(a.GetInvolvedFood().GetPrefab()))
                    {
                        f.Add(a.GetInvolvedFood().GetPrefab());
                    }
                    else
                    {
                        CreateObjectInScene(f[0], f.Count);
                        f.Clear();
                        f.Add(a.GetInvolvedFood().GetPrefab());
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