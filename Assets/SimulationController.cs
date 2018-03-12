using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecipeModule;

public class SimulationController : MonoBehaviour {

    public List<Action> actions;
    Recipe recipeToControl;
    int currentActionIndex = 0;

    public class ChoppingControl
    {
        public int currentNumberOfPieces;
        public GameObject choppedObject;

        public ChoppingControl(GameObject choppedObject, int currentNumberOfPieces)
        {
            this.currentNumberOfPieces = currentNumberOfPieces;
            this.choppedObject = choppedObject;
        }
    }

    public List<ChoppingControl> chopControls;

    private void Start()
    {
        chopControls = new List<ChoppingControl>();
    }

    public void SetRecipeToControl(Recipe r)
    {
        recipeToControl = r;
        actions = r.GetActions();
    }

    public void OnOperationDone(FoodCharacteristic fc, OperationEventArgs e)
    {
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
                    // if user starts to chop a new object, save it
                    if (!chopControls.Exists(item => item.choppedObject.GetInstanceID() ==  fc.GetInstanceID()))
                    {
                        chopControls.Add(new ChoppingControl(fc.gameObject, 1)); // we started with 1 because to get 2 pieces at the end
                    }




                    // give feedback, i.e. update checklist.

                    //find which food is being cut, update it's currentNumberOfPieces' by 1. 
                    // if current food's chopping parameters is satisfied, then increment the index
                    currentActionIndex++;
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
