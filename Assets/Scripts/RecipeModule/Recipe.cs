using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Recipe : MonoBehaviour
    {

        public GameObject[] foodList;

        private List<InvolvedFood> _involvedFoods;
        private List<Action> _actions;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public InvolvedFood AddNewInvolvedFood(GameObject o)
        {
            if (o.GetComponent<FoodStatus>())
            {
                InvolvedFood invFood = new InvolvedFood();
                _involvedFoods.Add(invFood);

                // Return back the created object. It's required to have a refence to object in UI module.
                return invFood;
            }

            return null;
        }
    }
}
