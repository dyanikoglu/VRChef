using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RecipeModule
{
    public class AddActionsTaskList : MonoBehaviour
    {
        public GameObject taskUI;
        public GameObject objectSpawner;
        int positionY;
        bool isActive;
        Vector3 scale;
        // Use this for initialization
        void Start()
        {
            scale = gameObject.transform.localScale;
            positionY = 150;
            isActive = true;
            Recipe r=objectSpawner.GetComponent<CreateRecipeScene>().GetRecipe();
            List<Action> actions = r.GetActions();
            GameObject a;

            int prevStepNumber = 0;

            foreach ( Action action in actions)
            {
                a = Instantiate(taskUI);
                a.transform.SetParent(gameObject.transform);
                Vector3 pos= gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition;
                pos.y = pos.y - positionY;
                a.GetComponent<RectTransform>().localPosition = pos;
                a.GetComponent<RectTransform>().localRotation = gameObject.transform.GetChild(0).GetComponent<RectTransform>().localRotation;
                a.GetComponent<RectTransform>().localScale = gameObject.transform.GetChild(0).GetComponent<RectTransform>().localScale;
                a.transform.GetChild(0).gameObject.SetActive(false);
                positionY = positionY + 50;

                string header = "    ";
                if (prevStepNumber != action.GetStepNumber()) {
                    header = action.GetStepNumber() + "- ";
                }
                
                if (action.GetActionType().ToString().Equals("Boil"))
                {
                    Boil boil = (Boil)action;
                    a.GetComponent<Text>().text = header + "Boil " + boil.GetInvolvedFood().GetFoodIdentifier()+" "+boil.GetRequiredTime()+" seconds";
                }
                else if (action.GetActionType().ToString().Equals("Chop"))
                {
                    Chop chop = (Chop)action;
                    a.GetComponent<Text>().text = header + "Chop " + chop.GetInvolvedFood().GetFoodIdentifier() + " to " + chop.GetRequiredPieceCount() + " pieces";
                }
                else if (action.GetActionType().ToString().Equals("Cook"))
                {
                    Cook cook = (Cook)action;
                    a.GetComponent<Text>().text = header + "Cook " + cook.GetInvolvedFood().GetFoodIdentifier() + " " + cook.GetRequiredTime() + " seconds in " + cook.GetRequiredHeat() + " celcius";
                }
                else if (action.GetActionType().ToString().Equals("Fry"))
                {
                    Fry fry = (Fry)action;
                    a.GetComponent<Text>().text = header + "Fry " + fry.GetInvolvedFood().GetFoodIdentifier() + " " + fry.GetRequiredTime() + " seconds";
                }
                else if (action.GetActionType().ToString().Equals("PutTogether"))
                {
                    PutTogether puttogether = (PutTogether)action;
                    a.GetComponent<Text>().text = "Put Together " + puttogether.GetInvolvedFood().GetFoodIdentifier() + " and " + puttogether.GetDestinationFood().GetFoodIdentifier();
                }
                else
                {
                    a.GetComponent<Text>().text = header + action.GetActionType() + " " + action.GetInvolvedFood().GetFoodIdentifier();
                }

                prevStepNumber = action.GetStepNumber();
                
            }
        }

        private void Update()
        {
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)|| OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
            {
                if (isActive)
                {
                    StartCoroutine(WaitForNotPressButtonToMakeUnactive());
                }
                else
                {
                    StartCoroutine(WaitForNotPressButtonToMakeActive());

                }
            }
        }

        IEnumerator WaitForNotPressButtonToMakeUnactive()
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(1);
            isActive = false;
        }

        IEnumerator WaitForNotPressButtonToMakeActive()
        {
            gameObject.transform.localScale = scale;
            yield return new WaitForSeconds(1);
            isActive = true;
        }

        public void SetStepCompleted(int stepNumber)
        {
            gameObject.transform.GetChild(stepNumber + 1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
