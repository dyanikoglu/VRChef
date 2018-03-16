using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecipeModule;

public class SimulationController : MonoBehaviour {

    public List<Action> actions;
    Recipe recipeToControl;
    int currentActionIndex = 0;

    public List<int> choppedObjects;
    public int numChoppedPieces = 0;
    public int numSlices = 0;

    public void SetRecipeToControl(Recipe r)
    {
        recipeToControl = r;
        actions = r.GetActions();
    }

    public void OnOperationDone(FoodCharacteristic fc, OperationEventArgs e)
    {
        //Debug.Log("operation done");
        switch (e.OperationType)
        {
            case Action.ActionType.Boil:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Boil)
                {
                    // give feedback, i.e. update checklist.
                    currentActionIndex++;
                }
                break;

            // this is unnecessary i think. 
            case Action.ActionType.Break:
                break;

            case Action.ActionType.Chop:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Chop)
                {
                    if ( (actions[currentActionIndex].GetInvolvedFood().GetFoodIdentifier() == fc.GetComponent<FoodStatus>().foodIdentifier)
                        && !(choppedObjects.Contains(fc.transform.root.GetInstanceID())) )
                    {
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
                            currentActionIndex++;
                            numChoppedPieces = 0;
                            numSlices = 0;
                            choppedObjects.Add(fc.transform.root.GetInstanceID());
                        }
                    }
                }
                break;

            case Action.ActionType.Cook:
                break;

            case Action.ActionType.Fry:
                if(actions[currentActionIndex].GetActionType() == Action.ActionType.Fry)
                {
                    // also obtain parameters and check if operation is valid
                    // give feedback, i.e. update checklist.
                    currentActionIndex++;
                }
                break;

            case Action.ActionType.Mix:
                break;

            case Action.ActionType.Peel:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Peel)
                {
                    // give feedback, i.e. update checklist.
                    currentActionIndex++;
                }
                break;

            case Action.ActionType.PutTogether:
                break;

            // waiting for smash process to be done.
            case Action.ActionType.Smash:
                break;

            case Action.ActionType.Squeeze:
                if (actions[currentActionIndex].GetActionType() == Action.ActionType.Squeeze)
                {
                    // give feedback, i.e. update checklist.
                    currentActionIndex++;
                }
                break;
        }

        if(currentActionIndex == actions.Count)
        {
            // display a message saying that user has succesfully completed the recipe. And finish the scene
        } 
    }
}
