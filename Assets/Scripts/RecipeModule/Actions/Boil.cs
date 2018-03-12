using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Boil : Action
    {
        public enum BoilType
        {
            Overboiled, Boiled, Underdone
        }
        [FullSerializer.fsProperty]
        private float requiredHeat;
        [FullSerializer.fsProperty]
        private float requiredTime;
        [FullSerializer.fsProperty]
        private BoilType boilType;

        public Boil() : base()
        {
            this.actionType = ActionType.Boil;
            requiredHeat = 0;
            requiredTime = 0;
            boilType = 0;
        }

        public Boil(int stepNumber, Food foodToBeBoiled, float requiredHeat, float requiredTime, BoilType boilType) : base(ActionType.Boil, stepNumber, foodToBeBoiled)
        {
            this.requiredHeat = requiredHeat;
            this.requiredTime = requiredTime;
            this.boilType = boilType;

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

        public BoilType GetBoilType()
        {
            return boilType;
        }

        public void SetBoilType(BoilType boilType)
        {
            this.boilType = boilType;
        }

        #endregion
    }
}