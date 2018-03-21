using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Boil : Action
    {
        [FullSerializer.fsProperty]
        private float requiredTime;

        public Boil() : base()
        {
            this.actionType = ActionType.Boil;
            requiredTime = 0;
        }

        public Boil(int stepNumber, Food foodToBeBoiled, float requiredTime) : base(ActionType.Boil, stepNumber, foodToBeBoiled)
        {
            this.requiredTime = requiredTime;

            DeriveResultedFood();
        }

        new private void DeriveResultedFood()
        {
            resultedFood.SetIsBoiled(true);
        }

        #region Mutators

        public float GetRequiredTime()
        {
            return requiredTime;
        }

        public void SetRequiredTime(float requiredTime)
        {
            this.requiredTime = requiredTime;
        }

        #endregion
    }
}