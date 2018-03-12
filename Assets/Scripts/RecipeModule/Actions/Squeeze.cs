using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Squeeze : Action
    {
        public Squeeze() : base()
        {
            this.actionType = ActionType.Squeeze;
        }

        public Squeeze(int stepNumber, Food foodToBeSqueezed) : base(ActionType.Squeeze, stepNumber, foodToBeSqueezed)
        {
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetIsSqueezed(true);
        }
    }
}