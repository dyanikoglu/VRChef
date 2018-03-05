using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class Step : MonoBehaviour {
    public Text stepNumberRef;
    public GameObject inputZoneRef;
    public GameObject actionZoneRef;
    public GameObject outputZoneRef;
    public Toggle toggleRef;

    // Required for generating new output food object in each step
    public GameObject dummyIOFoodObject;

    public void SetStepNumber(int stepNumber)
    {
        this.stepNumberRef.text = stepNumber.ToString();
    }

    public int GetStepNumber()
    {
        return Int32.Parse(stepNumberRef.text);
    }

    public void SetToggle(bool toggle)
    {
        this.toggleRef.isOn = toggle;
    }

    public bool GetToggle()
    {
        return toggleRef.isOn;
    }

    public void GenerateOutput(RecipeModule.Recipe recipe)
    {
        if(GetInput() == null || GetPseudoAction() == null || outputZoneRef.transform.childCount != 0)
        {
            return;
        }

        if(GetInput() is FoodState)
        {
            RecipeModule.Food f = ((FoodState)GetInput()).GetFood();

            List<int> parameters;
            RecipeModule.Food outputFood;

            switch (GetPseudoAction().GetActionType())
            {
                case RecipeModule.Action.ActionType.Boil:
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.Break:
                    outputFood = recipe.DescribeNewBreakAction(GetStepNumber(), f);
                    break;
                case RecipeModule.Action.ActionType.Chop:
                    outputFood = recipe.DescribeNewChopAction(GetStepNumber(), f, 0, (RecipeModule.Chop.PieceVolumeSize)GetPseudoAction().GetParameterValues()[0]);
                    break;
                case RecipeModule.Action.ActionType.Cook:
                    parameters = GetPseudoAction().GetParameterValues();
                    outputFood = recipe.DescribeNewCookAction(GetStepNumber(), f, parameters[0], parameters[1], (RecipeModule.Cook.CookType)parameters[2]);
                    break;
                case RecipeModule.Action.ActionType.Fry:
                    parameters = GetPseudoAction().GetParameterValues();
                    outputFood = recipe.DescribeNewFryAction(GetStepNumber(), f, parameters[0], parameters[1], (RecipeModule.Fry.FryType)parameters[2]);
                    break;
                case RecipeModule.Action.ActionType.Mix:
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.Peel:
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.PutTogether:
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.Smash:
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.Squeeze:
                    outputFood = null;
                    break;
                default:
                    outputFood = null;
                    break;
            }

            GameObject outputObject = GameObject.Instantiate(dummyIOFoodObject);
            outputObject.GetComponent<FoodState>().SetFood(outputFood);



            outputObject.transform.SetParent(outputZoneRef.transform, false);
            VRTK_UIDraggableItem draggableItem = outputObject.AddComponent<VRTK_UIDraggableItem>();
            draggableItem.forwardOffset = 0.05f;
            draggableItem.restrictToDropZone = true;
        }

        else if(GetInput() is FoodGroup)
        {
            // TODO Implement FoodGroup
        }

        outputZoneRef.transform.parent.gameObject.SetActive(true);
    }

    public object GetInput()
    {
        if(inputZoneRef.GetComponentInChildren<FoodState>())
        {
            return inputZoneRef.GetComponentInChildren<FoodState>();
        }

        else if(inputZoneRef.GetComponentInChildren<FoodGroup>())
        {
            return inputZoneRef.GetComponentInChildren<FoodGroup>();
        }

        else
        {
            return null;
        }
    }

    public object GetOutput()
    {
        if (outputZoneRef.GetComponentInChildren<FoodStatus>())
        {
            return outputZoneRef.GetComponentInChildren<FoodStatus>();
        }

        else if (outputZoneRef.GetComponentInChildren<FoodGroup>())
        {
            return outputZoneRef.GetComponentInChildren<FoodGroup>();
        }

        else
        {
            return null;
        }
    }

    public PseudoAction GetPseudoAction()
    {
        return actionZoneRef.GetComponentInChildren<PseudoAction>();
    }
}