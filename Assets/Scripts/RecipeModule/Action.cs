using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Action
    {
        public enum ActionType
        {
            Fry, Chop, Peel, Cook, Squeeze, Break, Smash, Mix, PutTogether, Boil
        }

        protected Food involvedFood;
        protected Food resultedFood;
        protected ActionType actionType;
        protected int stepNumber;

        public Action()
        {
            involvedFood = null;
            resultedFood = null;
            actionType = 0;
            stepNumber = -1;
        }

        public Action(ActionType actionType, int stepNumber, Food involvedFood)
        {
            this.actionType = actionType;
            this.stepNumber = stepNumber;
            this.involvedFood = involvedFood;

            DeriveResultedFood();
        }

        protected void DeriveResultedFood()
        {
            Food derivedFood = new Food(involvedFood);
            derivedFood.SetActionDerivedBy(this);

            involvedFood.SetNext(derivedFood);
            derivedFood.SetPrev(involvedFood);

            this.resultedFood = derivedFood;
        }

        #region Mutators

        public Food GetResultedFood()
        {
            return resultedFood;
        }

        public Food GetInvolvedFood()
        {
            return involvedFood;
        }

        public ActionType GetActionType()
        {
            return actionType;
        }

        public void SetActionType(ActionType actionType)
        {
            this.actionType = actionType;
        }

        public int GetStepNumber()
        {
            return stepNumber;
        }

        public void SetStepNumber(int stepNumber)
        {
            this.stepNumber = stepNumber;
        }

        #endregion
    }
}
