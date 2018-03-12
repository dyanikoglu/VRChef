using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Smash : Action
    {
        public Smash() : base()
        {
            this.actionType = ActionType.Smash;
        }

        public Smash(int stepNumber, Food foodToBeSmashed) : base(ActionType.Smash, stepNumber, foodToBeSmashed)
        {
            DeriveResultedFood();
        }

        new protected void DeriveResultedFood()
        {
            resultedFood.SetIsSmashed(true);
        }
    }
}
