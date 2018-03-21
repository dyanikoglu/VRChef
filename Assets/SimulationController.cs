using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecipeModule;

public class SimulationController : MonoBehaviour {

    public List<Action> actions;
    Recipe recipeToControl;
    public int currentActionIndex;

    public List<int> choppedObjects;
    public int numChoppedPieces;
    public int numSlices;

    public List<GameObject> foodsInRecipe;
    public int foodIndex;

    public GameObject taskListUI;

    public void SetRecipeToControl(Recipe r)
    {
        recipeToControl = r;
        actions = r.GetActions();
    }

    private void Start()
    {
        currentActionIndex = 0;
        numChoppedPieces = 0;
        numSlices = 0;
        foodIndex = 0;

        choppedObjects = new List<int>();
        foodsInRecipe = new List<GameObject>();
    }

    public void OnOperationDone(FoodCharacteristic fc, OperationEventArgs e)
    {
        switch (e.OperationType)
        {
            case Action.ActionType.Boil:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Boil)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;   
                    }
                }
                break;

            // this case will be run for only eggs. Therefore, checking that if broken thing has a "FoodStatus" is needed.
            case Action.ActionType.Break:
                if ( fc.GetComponent<FoodStatus>() && actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                {
                    taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                    currentActionIndex++;
                }
                break;

            case Action.ActionType.Chop:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Chop)
                {
                    if ( (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                        && !(choppedObjects.Contains(fc.transform.root.GetInstanceID())) )
                    {

                        Debug.Log(fc.transform.parent.gameObject);

                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe.Add(fc.transform.parent.gameObject);
                            foodIndex++;
                        }

                        //Debug.Log(fc.GetComponent<FoodStatus>().foodIdentifier);
                        numSlices++;
                        if (numSlices == 1)
                        {
                            numChoppedPieces = 2;
                        }
                        else
                        {
                            numChoppedPieces++;
                        }
                        
                        Chop chop = (Chop)actions[currentActionIndex];
                        // if current food's chopping parameters is satisfied, then set current action as done, i.e. increment index
                        if (numChoppedPieces == chop.GetRequiredPieceCount())
                        {
                            //Debug.Log(numChoppedPieces+ " - " + numSlices);
                            taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                            currentActionIndex++;
                            numChoppedPieces = 0;
                            numSlices = 0;
                            choppedObjects.Add(fc.transform.root.GetInstanceID());
                        }
                    }
                }
                break;

            /* for cook and fry, convenience of process has been determined in relevant script. There is no need to check parameters here again. */
            case Action.ActionType.Cook:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Cook)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;
                    }
                }
                break;

            case Action.ActionType.Fry:
                if(actions[currentActionIndex].GetActionType() == Action.ActionType.Fry)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;
                    }
                }
                break;

            // mix script has not been completed yet.
            case Action.ActionType.Mix:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Mix)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;
                    }
                }
                break;

            case Action.ActionType.Peel:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Peel)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;
                    }
                }
                break;

            case Action.ActionType.PutTogether:
                //Debug.Log("I ungrabbed sth");
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.PutTogether)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        bool isCloseEnough = true;
                        PutTogether pt = (PutTogether)actions[currentActionIndex];
                        GameObject destinationGameObject = foodsInRecipe[pt.GetDestinationFoodIndex()];
                        //Debug.Log(destinationGameObject.name);
                        //Debug.Log(fc.name);
                        for(int i = 0; i < fc.transform.parent.childCount; i++)
                        {
                            float distance = Vector3.Distance(fc.transform.parent.GetChild(i).position, destinationGameObject.transform.position);
                            Debug.Log(distance);
                            if(distance >= .15f)
                            {
                                isCloseEnough = false;
                                break;
                            }
                        }
                        if (isCloseEnough)
                        {
                            taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                            currentActionIndex++;
                        }
                        //float distance = Vector3.Distance(fc.gameObject.transform.position, destinationGameObject.transform.position);
                        //Debug.Log(distance);
                    }
                }
                break;

            case Action.ActionType.Smash:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Smash)
                {
                    if (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                    {
                        if (foodIndex == currentActionIndex)
                        {
                            foodsInRecipe[foodIndex] = fc.gameObject;
                            foodIndex++;
                        }
                        taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                        currentActionIndex++;
                    }
                }
                break;

            case Action.ActionType.Squeeze:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Squeeze)
                {
                    if (foodIndex == currentActionIndex)
                    {
                        foodsInRecipe[foodIndex] = fc.gameObject;
                        foodIndex++;
                    }
                    taskListUI.GetComponent<AddActionsTaskList>().SetStepCompleted(currentActionIndex);
                    currentActionIndex++;
                }
                break;
        }
    }
}
