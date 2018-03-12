using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Break : Action
    {
        public Break() : base()
        {
            this.actionType = ActionType.Break;
        }

        public Break(int stepNumber, Food foodToBeBroken) : base(ActionType.Break, stepNumber, foodToBeBroken)
        {
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetIsBroken(true);
        }
    }
}