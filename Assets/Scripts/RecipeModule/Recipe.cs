using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Recipe : MonoBehaviour
    {
        private List<Food> _initialFoods;
        private List<Action> _actions;
        private int totalStepCount = 0;

        private void Start()
        {
            _initialFoods = new List<Food>();
            _actions = new List<Action>();
        }

        public void ReorderActions()
        {
            List<Action> newActions = new List<Action>(_actions.Count);
            
            foreach(Action a in _actions)
            {
                newActions.Insert(a.GetStepNumber(), a);
            }

            this._actions = newActions;
        }

        public void RemoveAction(Action a)
        {
            if(_initialFoods.Contains(a.GetInvolvedFood())) {
                _initialFoods.Remove(a.GetInvolvedFood());
            }

            Food f = a.GetInvolvedFood();

            while(f != null)
            {
                f = a.GetInvolvedFood().GetNext();

                if (a.GetInvolvedFood().GetNext() != null)
                {
                    a.GetInvolvedFood().GetNext().SetPrev(null);
                }

                if (a.GetInvolvedFood().GetPrev() != null)
                {
                    a.GetInvolvedFood().GetPrev().SetNext(null);
                }
            }

            _actions.Remove(a);
        }

        public Food DescribeNewChopAction(int stepNumber, GameObject foodObject, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Chop action = new Chop(stepNumber, f, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewChopAction(int stepNumber, Food foodToBeChopped, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Chop action = new Chop(stepNumber, foodToBeChopped, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewCookAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Cook action = new Cook(stepNumber, f, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewCookAction(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Cook action = new Cook(stepNumber, foodToBeCooked, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPeelAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Peel action = new Peel(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPeelAction(int stepNumber, Food foodToBePeeled)
        {
            Peel action = new Peel(stepNumber, foodToBePeeled);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSqueezeAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Squeeze action = new Squeeze(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSqueezeAction(int stepNumber, Food foodToBePeeled)
        {
            Squeeze action = new Squeeze(stepNumber, foodToBePeeled);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSmashAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Smash action = new Smash(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSmashAction(int stepNumber, Food foodToBePeeled)
        {
            Smash action = new Smash(stepNumber, foodToBePeeled);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewFryAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Fry.FryType fryType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Fry action = new Fry(stepNumber, f, requiredHeat, requiredTime, fryType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }
        
        public Food DescribeNewFryAction(int stepNumber, Food foodToBeFried, float requiredHeat, float requiredTime, Fry.FryType fryType)
        {
            Fry action = new Fry(stepNumber, foodToBeFried, requiredHeat, requiredTime, fryType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBreakAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Break action = new Break(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBreakAction(int stepNumber, Food foodToBeBroken)
        {
            Break action = new Break(stepNumber, foodToBeBroken);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPutTogetherAction(int stepNumber, GameObject foodObject, Food destinationFood)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            PutTogether action = new PutTogether(stepNumber, f, destinationFood);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        // Both of objects are prefabs, create new Food classes for both of them
        public Food DescribeNewPutTogetherAction(int stepNumber, GameObject foodObject, GameObject destinationFood)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Food f2 = new Food(foodObject);
            _initialFoods.Add(f2);

            PutTogether action = new PutTogether(stepNumber, f, f2);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPutTogetherAction(int stepNumber, Food foodToBePutTogether, Food destinationFood)
        {
            PutTogether action = new PutTogether(stepNumber, foodToBePutTogether, destinationFood);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        
        public Food DescribeNewBoilAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Boil.BoilType boilType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Boil action = new Boil(stepNumber, f, requiredHeat, requiredTime, boilType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBoilAction(int stepNumber, Food foodToBeBoiled, float requiredHeat, float requiredTime, Boil.BoilType boilType)
        {
            Boil action = new Boil(stepNumber, foodToBeBoiled, requiredHeat, requiredTime, boilType);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }  

        #region Mutators

        public List<Food> GetInitialFoods()
        {
            return _initialFoods;
        }

        public List<Action> GetActions()
        {
            return _actions;
        }

        public int GetTotalStepCount()
        {
            return totalStepCount;
        }

        #endregion
    }
}
