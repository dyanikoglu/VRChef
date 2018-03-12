using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Cook : Action
    {
        public enum CookType
        {
            Overcooked, Cooked, Underdone
        }
        [FullSerializer.fsProperty]
        private float requiredHeat;
        [FullSerializer.fsProperty]
        private float requiredTime;
        [FullSerializer.fsProperty]
        private CookType cookType;
        
        public Cook() : base()
        {
            this.actionType = ActionType.Cook;
            requiredHeat = 0;
            requiredTime = 0;
            cookType = 0;
        }

        public Cook(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime, CookType cookType) : base(ActionType.Cook, stepNumber, foodToBeCooked)
        {
            this.requiredHeat = requiredHeat;
            this.requiredTime = requiredTime;
            this.cookType = cookType;

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

        public CookType GetCookType()
        {
            return cookType;
        }

        public void SetCookType(CookType cookType)
        {
            this.cookType = cookType;
        }

        #endregion
    }
}
