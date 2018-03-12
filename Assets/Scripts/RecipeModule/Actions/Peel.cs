using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Peel : Action
    {
        public Peel(): base()
        {
            this.actionType = ActionType.Peel;
        }

        public Peel(int stepNumber, Food foodToBePeeled) : base(ActionType.Peel, stepNumber, foodToBePeeled)
        {
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetIsPeeled(true);
        }
    }
}
