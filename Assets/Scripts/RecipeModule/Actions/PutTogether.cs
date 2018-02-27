using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class PutTogether : Action
    {
        Food destinationFood;

        public PutTogether() : base()
        {
            this.actionType = ActionType.PutTogether;
        }

        public PutTogether(int stepNumber, Food foodToBePut, Food destinationFood) : base(ActionType.PutTogether, stepNumber, foodToBePut)
        {
            this.destinationFood = destinationFood;
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetStayingWith(destinationFood);
            resultedFood.SetPutTogether(true);
        }
    }
}