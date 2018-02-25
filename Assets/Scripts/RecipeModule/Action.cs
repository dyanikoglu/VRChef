using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Action
    {
        public enum ActionType
        {
            Fry, Chop, Peel, Cook, Break, Smash, Mix, PutTogether
        }

        protected List<InvolvedFood> involvedFoods;
        protected ActionType actionType;
        protected int stepNumber;

        #region Mutators

        public ActionType GetActionType()
        {
            return actionType;
        }

        public int GetInvolvedFoodCount()
        {
            return involvedFoods.Count;
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

        public List<InvolvedFood> GetInvolvedFoods()
        {
            return involvedFoods;
        }

        #endregion
    }
}
