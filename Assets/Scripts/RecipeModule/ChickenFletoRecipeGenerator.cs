using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class ChickenFletoRecipeGenerator : MonoBehaviour
    {
        public Recipe recipe;

        public GameObject tomato;
        public GameObject potato;
        public GameObject chicken;

        void Start()
        {
            //// Recipe task list start

            // Chop 3 chicken fletos
            Food chicken_chopped_1 = recipe.DescribeNewChopAction(0, chicken, 2, Chop.PieceVolumeSize.Big); // Focused chicken object
            Food chicken_chopped_2 = recipe.DescribeNewChopAction(1, chicken, 2, Chop.PieceVolumeSize.Big);
            Food chicken_chopped_3 = recipe.DescribeNewChopAction(2, chicken, 2, Chop.PieceVolumeSize.Big);

            // Put them together - Put 2 & 3 to near of 1
            Food chicken_put_2 = recipe.DescribeNewPutTogetherAction(3, chicken_chopped_2, chicken_chopped_1);
            Food chicken_put_3 = recipe.DescribeNewPutTogetherAction(4, chicken_chopped_3, chicken_chopped_1);

            // Chop 2 tomatoes
            Food tomato_chopped_1 = recipe.DescribeNewChopAction(5, tomato, 6, Chop.PieceVolumeSize.Small);
            Food tomato_chopped_2 = recipe.DescribeNewChopAction(6, tomato, 6, Chop.PieceVolumeSize.Small);

            // Put chopped tomatoes near chicken_1
            Food tomato_put_1 = recipe.DescribeNewPutTogetherAction(7, tomato_chopped_1, chicken_chopped_1);
            Food tomato_put_2 = recipe.DescribeNewPutTogetherAction(8, tomato_chopped_2, chicken_chopped_1);

            // Peel a potato
            Food potato_peeled = recipe.DescribeNewPeelAction(9, potato);

            // Chop the peeled potato
            Food potato_chopped = recipe.DescribeNewChopAction(10, potato_peeled, 6, Chop.PieceVolumeSize.Middle);

            // Put chopped potato near chicken_1
            Food potato_put = recipe.DescribeNewPutTogetherAction(11, potato_chopped, chicken_chopped_1);

            // Cook'em'all
            Food chicken_cooked_1 = recipe.DescribeNewCookAction(12, chicken_chopped_1.GetLatestState(), 150, 600, Cook.CookType.Cooked);
            Food chicken_cooked_2 = recipe.DescribeNewCookAction(13, chicken_chopped_2.GetLatestState(), 150, 600, Cook.CookType.Cooked);
            Food chicken_cooked_3 = recipe.DescribeNewCookAction(14, chicken_chopped_3.GetLatestState(), 150, 600, Cook.CookType.Cooked);
            Food tomato_cooked_1 = recipe.DescribeNewCookAction(15, tomato_chopped_1.GetLatestState(), 150, 600, Cook.CookType.Cooked);
            Food tomato_cooked_2 = recipe.DescribeNewCookAction(16, tomato_chopped_2.GetLatestState(), 150, 600, Cook.CookType.Cooked);
            Food potato_cooked = recipe.DescribeNewCookAction(17, potato_chopped.GetLatestState(), 150, 600, Cook.CookType.Cooked);

            //// Recipe task list end

            recipe.Save();
        }
    }
}