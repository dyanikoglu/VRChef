using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Recipe : MonoBehaviour
    {
        public GameObject[] foodList;

        private List<Food> _initialFoods;
        private List<Action> _actions;

        /*
         * This overloaded function is for first action of initial objects, since these objects will be in GameObject type.
         * Adds this GameObject to initialFoods as Food.
         */
        public Action DescribeNewChopAction(int stepNumber, GameObject foodObject, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Chop action = new Chop(stepNumber, f, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            return action;
        }

        /*
        * This overloaded function is for next actions(except first) of initial objects, since these objects will be in Food type.
        * Doesn't add the Food object to initialFoods.
        */
        public Action DescribeNewChopAction(int stepNumber, Food foodToBeChopped, int requiredPieceCount, Chop.PieceVolumeSize pieceVolumeSize)
        {
            Chop action = new Chop(stepNumber, foodToBeChopped, requiredPieceCount, pieceVolumeSize);

            _actions.Add(action);

            return action;
        }


        /*
         * This overloaded function is for first action of initial objects, since these objects will be in GameObject type.
         * Adds this GameObject to initialFoods as Food.
         */
        public Action DescribeNewCookAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Food f = new Food(foodObject);
            _initialFoods.Add(f);

            Cook action = new Cook(stepNumber, f, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            return action;
        }

        /*
        * This overloaded function is for next actions(except first) of initial objects, since these objects will be in Food type.
        * Doesn't add the Food object to initialFoods.
        */
        public Action DescribeNewCookAction(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime, Cook.CookType cookType)
        {
            Cook action = new Cook(stepNumber, foodToBeCooked, requiredHeat, requiredTime, cookType);

            _actions.Add(action);

            return action;
        }
    }
}
