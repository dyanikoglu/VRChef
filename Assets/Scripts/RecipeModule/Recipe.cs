using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Recipe : MonoBehaviour
    {
        private List<Food> _initialFoods;
        private List<Action> _actions;

        public Action DescribeNewChopAction(int stepNumber, GameObject foodObject, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Chop action = new Chop(stepNumber, f, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewChopAction(int stepNumber, Food foodToBeChopped, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Chop action = new Chop(stepNumber, foodToBeChopped, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewCookAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Cook action = new Cook(stepNumber, f, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewCookAction(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Cook action = new Cook(stepNumber, foodToBeCooked, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewPeelAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Peel action = new Peel(stepNumber, f);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewPeelAction(int stepNumber, Food foodToBePeeled)
        {
            Peel action = new Peel(stepNumber, foodToBePeeled);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewSqueezeAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Squeeze action = new Squeeze(stepNumber, f);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewSqueezeAction(int stepNumber, Food foodToBePeeled)
        {
            Squeeze action = new Squeeze(stepNumber, foodToBePeeled);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewSmashAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Smash action = new Smash(stepNumber, f);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewSmashAction(int stepNumber, Food foodToBePeeled)
        {
            Smash action = new Smash(stepNumber, foodToBePeeled);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewFryAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Fry.FryType fryType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Fry action = new Fry(stepNumber, f, requiredHeat, requiredTime, fryType);

            _actions.Add(action);

            return action;
        }
        
        public Action DescribeNewFryAction(int stepNumber, Food foodToBeFried, float requiredHeat, float requiredTime, Fry.FryType fryType)
        {
            Fry action = new Fry(stepNumber, foodToBeFried, requiredHeat, requiredTime, fryType);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewBreakAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Break action = new Break(stepNumber, f);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewBreakAction(int stepNumber, Food foodToBeBroken)
        {
            Break action = new Break(stepNumber, foodToBeBroken);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewBoilAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Boil.BoilType boilType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Boil action = new Boil(stepNumber, f, requiredHeat, requiredTime, boilType);

            _actions.Add(action);

            return action;
        }

        public Action DescribeNewBoilAction(int stepNumber, Food foodToBeBoiled, float requiredHeat, float requiredTime, Boil.BoilType boilType)
        {
            Boil action = new Boil(stepNumber, foodToBeBoiled, requiredHeat, requiredTime, boilType);

            _actions.Add(action);

            return action;
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

        #endregion
    }
}
