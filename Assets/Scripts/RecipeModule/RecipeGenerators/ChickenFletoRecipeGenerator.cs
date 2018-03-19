using UnityEngine;

namespace RecipeModule
{
    public class ChickenFletoRecipeGenerator : MonoBehaviour
    {
        public GameObject tomato;
        public GameObject potato;
        public GameObject chicken;

        void Start()
        {
            Recipe recipe = new Recipe("Chicken_Fleto");

            //// Recipe task list start
            recipe.SetRecipeName("Chicken_Fleto");

            // Chop 3 chicken fletos
            Food chicken_chopped_1 = recipe.DescribeNewChopAction(0, chicken, 2); // Focused chicken object
            Food chicken_chopped_2 = recipe.DescribeNewChopAction(1, chicken, 2);
            Food chicken_chopped_3 = recipe.DescribeNewChopAction(2, chicken, 2);

            // Put them together - Put 2 & 3 to near of 1
            Food chicken_put_2 = recipe.DescribeNewPutTogetherAction(3, chicken_chopped_2, chicken_chopped_1, 0);
            Food chicken_put_3 = recipe.DescribeNewPutTogetherAction(4, chicken_chopped_3, chicken_chopped_1, 0);

            // Chop 2 tomatoes
            Food tomato_chopped_1 = recipe.DescribeNewChopAction(5, tomato, 6);
            Food tomato_chopped_2 = recipe.DescribeNewChopAction(6, tomato, 6);

            // Put chopped tomatoes near chicken_1
            Food tomato_put_1 = recipe.DescribeNewPutTogetherAction(7, tomato_chopped_1, chicken_chopped_1, 0);
            Food tomato_put_2 = recipe.DescribeNewPutTogetherAction(8, tomato_chopped_2, chicken_chopped_1, 0);

            // Peel a potato
            Food potato_peeled = recipe.DescribeNewPeelAction(9, potato);

            // Chop the peeled potato
            Food potato_chopped = recipe.DescribeNewChopAction(10, potato_peeled, 6);

            // Put chopped potato near chicken_1
            Food potato_put = recipe.DescribeNewPutTogetherAction(11, potato_chopped, chicken_chopped_1, 0);

            // Cook'em'all
            Food chicken_cooked_1 = recipe.DescribeNewCookAction(12, chicken_chopped_1.GetLatestState(), 150, 600);
            Food chicken_cooked_2 = recipe.DescribeNewCookAction(13, chicken_chopped_2.GetLatestState(), 150, 600);
            Food chicken_cooked_3 = recipe.DescribeNewCookAction(14, chicken_chopped_3.GetLatestState(), 150, 600);
            Food tomato_cooked_1 = recipe.DescribeNewCookAction(15, tomato_chopped_1.GetLatestState(), 150, 600);
            Food tomato_cooked_2 = recipe.DescribeNewCookAction(16, tomato_chopped_2.GetLatestState(), 150, 600);
            Food potato_cooked = recipe.DescribeNewCookAction(17, potato_chopped.GetLatestState(), 150, 600);

            //// Recipe task list end

            // Save recipe
            Recipe.SaveRecipe(recipe);
        }
    }
}