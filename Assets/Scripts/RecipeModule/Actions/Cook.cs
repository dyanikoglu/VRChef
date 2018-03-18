using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Cook : Action
    {
        [FullSerializer.fsProperty]
        private float requiredHeat;
        [FullSerializer.fsProperty]
        private float requiredTime;
        
        public Cook() : base()
        {
            this.actionType = ActionType.Cook;
            requiredHeat = 0;
            requiredTime = 0;
        }

        public Cook(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime) : base(ActionType.Cook, stepNumber, foodToBeCooked)
        {
            this.requiredHeat = requiredHeat;
            this.requiredTime = requiredTime;

            DeriveResultedFood();
        }

        new private void DeriveResultedFood()
        {
            resultedFood.SetIsCooked(true);
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
