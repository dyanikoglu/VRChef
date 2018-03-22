using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

namespace RecipeModule { 
    public class Recipe
    {
        [FullSerializer.fsProperty]
        private List<Food> _initialFoods;
        [FullSerializer.fsProperty]
        private List<Action> _actions;
        [FullSerializer.fsProperty]
        private int totalStepCount = 0;
        [FullSerializer.fsProperty]
        private string recipeName;

        public Recipe(string recipeName)
        {
            this.recipeName = recipeName;
            _initialFoods = new List<Food>();
            _actions = new List<Action>();
        }

        public Recipe()
        {
            this.recipeName = "";
            _initialFoods = new List<Food>();
            _actions = new List<Action>();
        }

        // XOR Decrypt/Encryption for saved files
        private static string XOREncryptDecrypt(string s, string key)
        {
            string result = ""; ;
            for (int i = 0; i < s.Length; i++)
            {
                result += (char)(s[i] ^ key[i % key.Length]);
            }
            return result;
        }

        // Saves the recipe that given as parameter
        public static void SaveRecipe(Recipe recipeToBeSaved)
        {
            // Serialize this recipe
            string recipeData = StringSerializationAPI.Serialize(typeof(Recipe), recipeToBeSaved);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;

            // If save file exists, open it
            if (File.Exists(Application.dataPath + "/Recipes/" + recipeToBeSaved.recipeName + ".vrcr"))
            {
                file = File.OpenWrite(Application.dataPath + "/Recipes/" + recipeToBeSaved.recipeName + ".vrcr");
            }
            // Save file doesn't exist, create new one.
            else
            {
                file = File.Create(Application.dataPath + "/Recipes/" + recipeToBeSaved.recipeName + ".vrcr");
            }

            bf.Serialize(file, recipeData);
            file.Close();
        }
       

        // Loads and returns the recipe with given name as parameter
        public static Recipe LoadRecipe(string recipeName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;

            if (File.Exists(Application.dataPath + "/Recipes/" + recipeName + ".vrcr"))
            {
                file = File.OpenRead(Application.dataPath + "/Recipes/" + recipeName + ".vrcr");
            }
            else
            {
                return null;
            }

            Recipe loadedRecipe = (Recipe) StringSerializationAPI.Deserialize(typeof(Recipe), (string)bf.Deserialize(file));
            file.Close();

            return loadedRecipe;
        }

        public void ReorderActions()
        {
            int maxStepNumber = -1;
            foreach (Action a in _actions)
            {
                if (a.GetStepNumber() > maxStepNumber)
                {
                    maxStepNumber = a.GetStepNumber();
                }
            }

            Debug.Log("Max: " + maxStepNumber);


            bool found = false;
            for (int i = 1; i <= maxStepNumber; i++)
            {
                found = false;
                foreach (Action a in _actions)
                {
                    if (a.GetStepNumber() == i)
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    foreach (Action a in _actions)
                    {
                        if (a.GetStepNumber() > i)
                        {
                            Debug.Log("Reduced Step " + a.GetStepNumber());
                            a.SetStepNumber(a.GetStepNumber() - 1);
                            Debug.Log("New Step Number: " + a.GetStepNumber());
                        }
                    }
                }
            }

            Debug.Log("Step reduction completed");

            maxStepNumber = -1;
            foreach (Action a in _actions)
            {
                if (a.GetStepNumber() > maxStepNumber)
                {
                    maxStepNumber = a.GetStepNumber();
                }
            }

            for (int i = 1; i <= maxStepNumber; i++)
            {
                int stepCountWithSameNo = 0;

                foreach (Action a in _actions)
                {
                    if (a.GetStepNumber() == i)
                    {
                        stepCountWithSameNo++;
                    }
                }

                if(stepCountWithSameNo > 1)
                {
                    foreach (Action a in _actions)
                    {
                        if (a.GetStepNumber() > i)
                        {
                            a.SetStepNumber(a.GetStepNumber() + stepCountWithSameNo - 1);
                        }
                    }

                    int increment = 0;

                    foreach (Action a in _actions)
                    {
                        if (a.GetStepNumber() == i)
                        {
                            a.SetStepNumber(a.GetStepNumber() + increment++);
                        }
                    }

                    maxStepNumber += stepCountWithSameNo - 1;
                }
            }


            maxStepNumber = -1;
            foreach (Action a in _actions)
            {
                if (a.GetStepNumber() > maxStepNumber)
                {
                    maxStepNumber = a.GetStepNumber();
                }
            }

            Debug.Log(maxStepNumber);

            // Create reordered action list
            List<Action> newActions = new List<Action>(_actions.Count);

            for(int i=1;i<=maxStepNumber;i++)
            {
                foreach(Action a in _actions)
                {
                    if(a.GetStepNumber() == i)
                    {
                        newActions.Add(a);
                    }
                }
            }

            this._actions = newActions;
        }

        public Food DescribeNewChopAction(int stepNumber, GameObject foodObject, int requiredPieceCount)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Chop action = new Chop(stepNumber, f, requiredPieceCount);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewChopAction(int stepNumber, string identifier, int requiredPieceCount)
        {
            Food f = new Food(identifier);
            _initialFoods.Add(f);

            Chop action = new Chop(stepNumber, f, requiredPieceCount);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewChopAction(int stepNumber, Food foodToBeChopped, int requiredPieceCount)
        {
            Chop action = new Chop(stepNumber, foodToBeChopped, requiredPieceCount);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewCookAction(int stepNumber, GameObject foodObject, float requiredHeat, float requiredTime)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Cook action = new Cook(stepNumber, f, requiredHeat, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewCookAction(int stepNumber, string identifier, float requiredHeat, float requiredTime)
        {
            Food f = new Food(identifier);
            _initialFoods.Add(f);

            Cook action = new Cook(stepNumber, f, requiredHeat, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewCookAction(int stepNumber, Food foodToBeCooked, float requiredHeat, float requiredTime)
        {
            Cook action = new Cook(stepNumber, foodToBeCooked, requiredHeat, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPeelAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Peel action = new Peel(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewPeelAction(int stepNumber, string identifier)
        {
            Food f = new Food(identifier);
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
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Squeeze action = new Squeeze(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSqueezeAction(int stepNumber, string identifier)
        {
            Food f = new Food(identifier);
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
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Smash action = new Smash(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewSmashAction(int stepNumber, string identifier)
        {
            Food f = new Food(identifier);
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

        public Food DescribeNewFryAction(int stepNumber, GameObject foodObject, float requiredTime)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Fry action = new Fry(stepNumber, f, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewFryAction(int stepNumber, string identifier, float requiredTime)
        {
            Food f = new Food(identifier);
            _initialFoods.Add(f);

            Fry action = new Fry(stepNumber, f, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewFryAction(int stepNumber, Food foodToBeFried, float requiredTime)
        {
            Fry action = new Fry(stepNumber, foodToBeFried, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBreakAction(int stepNumber, GameObject foodObject)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Break action = new Break(stepNumber, f);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBreakAction(int stepNumber, string identifier)
        {
            Food f = new Food(identifier);
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

        public Food DescribeNewPutTogetherAction(int stepNumber, Food foodToBePutTogether, Food destinationFood, int foodNo)
        {
            PutTogether action = new PutTogether(stepNumber, foodToBePutTogether, destinationFood, foodNo);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        
        public Food DescribeNewBoilAction(int stepNumber, GameObject foodObject, float requiredTime)
        {
            Food f = new Food(foodObject.GetComponent<FoodStatus>().foodIdentifier);
            _initialFoods.Add(f);

            Boil action = new Boil(stepNumber, f, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBoilAction(int stepNumber, string identifier, float requiredTime)
        {
            Food f = new Food(identifier);
            _initialFoods.Add(f);

            Boil action = new Boil(stepNumber, f, requiredTime);

            _actions.Add(action);

            totalStepCount++;

            return action.GetResultedFood();
        }

        public Food DescribeNewBoilAction(int stepNumber, Food foodToBeBoiled, float requiredTime)
        {
            Boil action = new Boil(stepNumber, foodToBeBoiled, requiredTime);

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

        public void SetRecipeName(string recipeName)
        {
            this.recipeName = recipeName;
        }

        public string GetRecipeName()
        {
            return recipeName;
        }

        #endregion
    }
}
