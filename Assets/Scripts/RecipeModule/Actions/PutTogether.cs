using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class PutTogether : Action
    {
        [FullSerializer.fsProperty]
        Food destinationFood;
        int destinationFoodIndex;

        public PutTogether() : base()
        {
            this.actionType = ActionType.PutTogether;
        }

        public PutTogether(int stepNumber, Food foodToBePut, Food destinationFood, int destinationFoodIndex) : base(ActionType.PutTogether, stepNumber, foodToBePut)
        {
            this.destinationFood = destinationFood;
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetStayingWith(destinationFood);
            resultedFood.SetPutTogether(true);
        }

        public Food GetFootToBePut()
        {
            return involvedFood;
        }

        public Food GetDestinationFood()
        {
            return destinationFood;
        }

        public int GetDestinationFoodIndex()
        {
            return destinationFoodIndex;
        }
    }
}