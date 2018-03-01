using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RecipeModule
{
    public class Food
    {
        [FullSerializer.fsProperty]
        private string foodIdentifier;
        [FullSerializer.fsProperty]
        private bool isChopped;
        [FullSerializer.fsProperty]
        private bool isCooked;
        [FullSerializer.fsProperty]
        private bool isFried;
        [FullSerializer.fsProperty]
        private bool isPeeled;
        [FullSerializer.fsProperty]
        private bool isSqueezed;
        [FullSerializer.fsProperty]
        private bool isBroken;
        [FullSerializer.fsProperty]
        private bool isSmashed;
        [FullSerializer.fsProperty]
        private bool isBoiled;
        [FullSerializer.fsProperty]
        private bool isMixed;
        [FullSerializer.fsProperty]
        private bool isPutTogether;

        [FullSerializer.fsProperty]
        private Food stayingWith; // Indicates that this object is staying(and should stay) together with this object.

        [FullSerializer.fsProperty]
        private Food next;
        [FullSerializer.fsProperty]
        private Food prev;

        [FullSerializer.fsProperty]
        private Action actionDerivedBy;

        public Food()
        {
            isChopped = false;
            isCooked = false;
            isFried = false;
            isPeeled = false;
            isSqueezed = false;
            isBroken = false;
            isSmashed = false;
            isBoiled = false;
            isMixed = false;
            isPutTogether = false;

            actionDerivedBy = null;

            next = null;
            prev = null;

            foodIdentifier = "";
        }

        public Food(Food f)
        {
            isChopped = f.isChopped;
            isCooked = f.isCooked;
            isFried = f.isFried;
            isPeeled = f.isPeeled;
            isSqueezed = f.isSqueezed;
            isBroken = f.isBroken;
            isSmashed = f.isSmashed;
            isBoiled = f.isBoiled;
            isMixed = f.isMixed;
            isPutTogether = f.isPutTogether;

            actionDerivedBy = null;

            next = null;
            prev = null;

            foodIdentifier = f.foodIdentifier;
        }

        public Food(string foodIdentifier)
        {
            isChopped = false;
            isCooked = false;
            isFried = false;
            isPeeled = false;
            isSqueezed = false;
            isBroken = false;
            isSmashed = false;
            isBoiled = false;
            isMixed = false;
            isPutTogether = false;

            actionDerivedBy = null;

            next = null;
            prev = null;

            this.foodIdentifier = foodIdentifier;
        }

        #region Mutators

        public Food GetNext()
        {
            return next;
        }

        public Action GetActionDerivedBy()
        {
            return actionDerivedBy;
        }

        public void SetActionDerivedBy(Action actionDerivedBy)
        {
            this.actionDerivedBy = actionDerivedBy;
        }

        public void SetNext(Food next)
        {
            this.next = next;
        }

        public Food GetPrev()
        {
            return prev;
        }

        public void SetPrev(Food prev)
        {
            this.prev = prev;
        }

        public string GetFoodIdentifier()
        {
            return foodIdentifier;
        }

        public void SetFoodIdentifier(string foodIdentifier)
        {
            this.foodIdentifier = foodIdentifier;
        }

        public void SetIsChopped(bool isChopped)
        {
            this.isChopped = isChopped;
        }

        public bool GetIsChopped()
        {
            return isChopped;
        }

        public void SetIsCooked(bool isCooked)
        {
            this.isCooked = isCooked;
        }

        public bool GetIsCooked()
        {
            return isCooked;
        }

        public void SetIsPeeled(bool isPeeled)
        {
            this.isPeeled = isPeeled;
        }

        public bool GetIsPeeled()
        {
            return isPeeled;
        }

        public void SetIsSqueezed(bool isSqueezed)
        {
            this.isSqueezed = isSqueezed;
        }

        public bool GetIsSquuezed()
        {
            return isSqueezed;
        }

        public void SetIsSmashed(bool isSmashed)
        {
            this.isSmashed = isSmashed;
        }

        public bool GetIsSmashed()
        {
            return isSmashed;
        }

        public void SetIsFried(bool isFried)
        {
            this.isFried = isFried;
        }

        public bool GetIsFried()
        {
            return isFried;
        }

        public void SetIsBroken(bool isBroken)
        {
            this.isBroken = isBroken;
        }

        public bool GetIsBroken()
        {
            return isBroken;
        }

        public void SetIsBoiled(bool isBoiled)
        {
            this.isBoiled = isBoiled;
        }

        public bool GetIsBoiled()
        {
            return isBoiled;
        }

        public Food GetStayingWith()
        {
            return stayingWith;
        }

        public void SetStayingWith(Food f)
        {
            stayingWith = f;
        }

        public void SetPutTogether(bool isPutTogether)
        {
            this.isPutTogether = isPutTogether;
        }

        public bool GetPutTogether()
        {
            return isPutTogether;
        }

        public Food GetLatestState()
        {
            Food f = this;
            while(f.next != null)
            {
                f = f.next;
            }
            return f;
        }

        public Food GetFirstState()
        {
            Food f = this;
            while (f.prev != null)
            {
                f = f.prev;
            }
            return f;
        }

        #endregion
    }
}