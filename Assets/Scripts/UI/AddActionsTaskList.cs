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
                a = Instantiate(taskUI, taskUI.transform.position, taskUI.transform.rotation);
                a.transform.GetChild(0).gameObject.SetActive(true);
                a.transform.SetParent(gameObject.transform);
                if (action.GetActionType().ToString().Equals("Boil"))
                {
                    Boil boil = (Boil)action;
                    a.GetComponent<Text>().text = "Boil " + boil.GetInvolvedFood().GetFoodIdentifier()+" "+boil.GetRequiredTime()+" seconds";
                }
                else if (action.GetActionType().ToString().Equals("Chop"))
                {
                    Chop chop = (Chop)action;
                    a.GetComponent<Text>().text = "Chop " + chop.GetInvolvedFood().GetFoodIdentifier() + " to " + chop.GetRequiredPieceCount() + " pieces";
                }
                else if (action.GetActionType().ToString().Equals("Cook"))
                {
                    Cook cook = (Cook)action;
                    a.GetComponent<Text>().text = "Cook " + cook.GetInvolvedFood().GetFoodIdentifier() + " " + cook.GetRequiredTime() + " seconds in " + cook.GetRequiredHeat() + " celcius";
                }
                else if (action.GetActionType().ToString().Equals("Fry"))
                {
                    Fry fry = (Fry)action;
                    a.GetComponent<Text>().text = "Fry " + fry.GetInvolvedFood().GetFoodIdentifier() + " " + fry.GetRequiredTime() + " seconds";
                }
                else if (action.GetActionType().ToString().Equals("PutTogether"))
                {
                    PutTogether puttogether = (PutTogether)action;
                    a.GetComponent<Text>().text = "Put Together " + puttogether.GetInvolvedFood().GetFoodIdentifier() + " and " + puttogether.GetDestinationFood().GetFoodIdentifier();
                }
                else
                {
                    a.GetComponent<Text>().text = action.GetActionType() + " " + action.GetInvolvedFood().GetFoodIdentifier();
                }
                
            }
            Vector3 pos1 = taskUI.transform.position;
            pos1.y = 360;
            taskUI.transform.position = pos1;
        }

        public void SetStepCompleted(int stepNumber)
        {
            gameObject.transform.GetChild(stepNumber + 1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
