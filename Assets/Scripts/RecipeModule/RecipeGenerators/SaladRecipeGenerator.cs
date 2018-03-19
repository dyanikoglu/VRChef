using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class SaladRecipeGenerator : MonoBehaviour
    {
        public GameObject tomato;
        public GameObject cucumber;
        public GameObject cabbage;
        public GameObject pepper;

        void Start()
        {
            Recipe recipe = new Recipe("Salad");
            //// Recipe task list start

            // Chop 2 tomatoes into small pieces
            Food tomato_chopped_1 = recipe.DescribeNewChopAction(0, tomato, 12);
            Food tomato_chopped_2 = recipe.DescribeNewChopAction(1, tomato, 12);

            // Chop 3 cucumbers into small pieces
            Food cucumber_chopped_1 = recipe.DescribeNewChopAction(3, cucumber, 12);
            Food cucumber_chopped_2 = recipe.DescribeNewChopAction(4, cucumber, 12);
            Food cucumber_chopped_3 = recipe.DescribeNewChopAction(5, cucumber, 12);

            // Chop a cabbage into middle pieces
            Food cabbage_chopped = recipe.DescribeNewChopAction(6, cabbage, 8);

            // Chop a pepper into middle pieces
            Food pepper_chopped = recipe.DescribeNewChopAction(6, pepper, 8);

            // Combine all of them near tomato_1
            recipe.DescribeNewPutTogetherAction(7, tomato_chopped_2, tomato_chopped_1, 0);
            recipe.DescribeNewPutTogetherAction(8, cucumber_chopped_1, tomato_chopped_1, 0);
            recipe.DescribeNewPutTogetherAction(9, cucumber_chopped_2, tomato_chopped_1, 0);
            recipe.DescribeNewPutTogetherAction(10, cucumber_chopped_3, tomato_chopped_1, 0);
            recipe.DescribeNewPutTogetherAction(11, cabbage_chopped, tomato_chopped_1, 0);
            recipe.DescribeNewPutTogetherAction(12, pepper_chopped, tomato_chopped_1, 0);

            //// Recipe task list end
            Recipe.SaveRecipe(recipe);
        }
    }
}