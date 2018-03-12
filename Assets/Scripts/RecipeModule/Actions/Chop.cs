using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Chop : Action
    {
        [FullSerializer.fsProperty]
        private int requiredPieceCount;

        public Chop() : base()
        {
            this.actionType = ActionType.Chop;
            this.requiredPieceCount = 0;
        }

        public Chop(int stepNumber, Food foodToBeChopped, int requiredPieceCount) : base(ActionType.Chop, stepNumber, foodToBeChopped)
        {
            this.requiredPieceCount = requiredPieceCount;
            DeriveResultedFood();
        }

        new private void DeriveResultedFood()
        {
            resultedFood.SetIsChopped(true);
        }

        

        #region Mutators
        public int GetRequiredPieceCount()
        {
            return requiredPieceCount;
        }

        public void SetRequiredPieceCount(int requiredPieceCount)
        {
            this.requiredPieceCount = requiredPieceCount;
        }
        #endregion
    }
}
