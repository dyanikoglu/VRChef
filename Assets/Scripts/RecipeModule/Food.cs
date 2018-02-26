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

        #endregion
    }
}
