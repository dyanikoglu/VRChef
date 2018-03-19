using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RecipeModule
{
    public class AddActionsTaskList : MonoBehaviour
    {
        public GameObject taskUI;
        public string recipe;
        // Use this for initialization
        void Start()
        {
            Recipe r = Recipe.LoadRecipe(recipe);
            List<Action> actions = r.GetActions();
            GameObject a;
            
            foreach( Action action in actions)
            {
                Vector3 pos = taskUI.transform.position;
                pos.y = pos.y - 20;
                taskUI.transform.position = pos;
                a = Instantiate(taskUI,taskUI.transform.position,taskUI.transform.rotation);
                a.transform.GetChild(0).gameObject.SetActive(false);
                a.transform.SetParent(gameObject.transform);
                a.GetComponent<Text>().text = action.GetActionType() + " "+ action.GetInvolvedFood().GetFoodIdentifier();
            }
            Vector3 pos1 = taskUI.transform.position;
            pos1.y = 360;
            taskUI.transform.position = pos1;
        }

        public void SetStepCompleted(int stepNumber)
        {
            print(stepNumber);
            gameObject.transform.GetChild(stepNumber + 1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
