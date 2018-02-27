using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class RecipeAutoUI : MonoBehaviour
    {

        public Recipe recipe;

        public GameObject tomato;
        public GameObject potato;
        public GameObject onion;

        // Use this for initialization
        void Start()
        {
            Food chop1 = recipe.DescribeNewChopAction(0, tomato, 6, Chop.PieceVolumeSize.Middle);
            Food cook1 = recipe.DescribeNewCookAction(1, chop1, 150, 600, Cook.CookType.Cooked);

            Food chop2 = recipe.DescribeNewChopAction(2, onion, 6, Chop.PieceVolumeSize.Big);
            Food fry1 = recipe.DescribeNewFryAction(3, chop2, 200, 300, Fry.FryType.Fried);

            print(chop1.GetNext().GetIsChopped());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}