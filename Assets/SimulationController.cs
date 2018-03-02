using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecipeModule;

public class SimulationController : MonoBehaviour {

    public List<Action> actions;
    Recipe recipeToControl;

    //foreach (Action a in actions)
    //{
    //    //Debug.Log(a.GetStepNumber() + " - " + a.GetInvolvedFood().GetFoodIdentifier());
    //    //Debug.Log(a.GetActionType() + " - " + a.GetResultedFood().GetFoodIdentifier());
    //}

    public void SetRecipeToControl(Recipe r)
    {
        recipeToControl = r;
        actions = r.GetActions();
    }

    public void OnOperationDone(object obj, OperationEventArgs e)
    {
        switch (e.OperationType)
        {
            case Action.ActionType.Boil:
                break;
            case Action.ActionType.Break:
                break;
            case Action.ActionType.Chop:
                break;
            case Action.ActionType.Cook:
                break;
            case Action.ActionType.Fry:
                // also obtain parameters and check if operation is proper
                Debug.Log("fry action has done in scene and received in Simulation Controller");
                break;
            case Action.ActionType.Mix:
                break;
            case Action.ActionType.Peel:
                break;
            case Action.ActionType.PutTogether:
                break;
            case Action.ActionType.Smash:
                break;
            case Action.ActionType.Squeeze:
                break;
        }
    }
}
