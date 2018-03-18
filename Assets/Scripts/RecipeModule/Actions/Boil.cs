using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Boil : Action
    {
        [FullSerializer.fsProperty]
        private float requiredHeat;
        [FullSerializer.fsProperty]
        private float requiredTime;

        public Boil() : base()
        {
            this.actionType = ActionType.Boil;
            requiredHeat = 0;
            requiredTime = 0;
        }

        public Boil(int stepNumber, Food foodToBeBoiled, float requiredHeat, float requiredTime) : base(ActionType.Boil, stepNumber, foodToBeBoiled)
        {
            this.requiredHeat = requiredHeat;
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

        public float GetRequiredHeat()
        {
            return requiredHeat;
        }

        public void SetRequiredHeat(float requiredHeat)
        {
            this.requiredHeat = requiredHeat;
        }

        #endregion
    }
}