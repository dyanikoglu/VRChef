using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Food
    {
        private GameObject objectPrefab;

        private bool isChopped;
        private bool isCooked;
        private bool isFried;
        private bool isPeeled;
        private bool isSqueezed;
        private bool isBroken;
        private bool isSmashed;
        private bool isBoiled;
        private bool isMixed;
        private bool isPutTogether;

        private Food stayingWith; // Indicates that this object is staying(and should stay) together with this object.

        private Food next;
        private Food prev;

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

            objectPrefab = null;
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

            objectPrefab = f.objectPrefab;
        }

        public Food(GameObject o)
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

            objectPrefab = o;
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

        public GameObject GetPrefab()
        {
            return objectPrefab;
        }

        public void SetPrefab(GameObject objectPrefab)
        {
            this.objectPrefab = objectPrefab;
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