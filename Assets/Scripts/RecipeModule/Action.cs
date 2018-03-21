using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Action
    {
        public enum ActionType
        {
            Fry, Chop, Peel, Cook, Squeeze, Break, Smash, Mix, PutTogether, Boil, Empty
        }

        [FullSerializer.fsProperty]
        protected Food involvedFood;
        [FullSerializer.fsProperty]
        protected Food resultedFood;
        [FullSerializer.fsProperty]
        protected ActionType actionType;
        [FullSerializer.fsProperty]
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
