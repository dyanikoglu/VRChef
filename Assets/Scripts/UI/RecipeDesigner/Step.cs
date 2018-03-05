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
    public GameObject groupConnectorRef;

    public StepManager stepManager;

    // This step needs to be regenerated if it's true
    public bool dirty = false;

    // Denotes that if this step is already grouped. Grouped steps can't be grouped again.
    public bool outputGrouped = false;

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

    public void SetDirty(bool dirty)
    {
        this.dirty = dirty;
    }

    public bool IsDirty()
    {
        return dirty;
    }

    // Generate output of this step, register it to recipe data structure.
    public void GenerateOutput(RecipeModule.Recipe recipe)
    {
        if (GetInput() == null || GetPseudoAction() == null)
        {
            // Remove existing ouput object
            if (outputZoneRef.transform.childCount == 1)
            {
                GameObject oldOutputObject = outputZoneRef.transform.GetChild(0).gameObject;
                Destroy(oldOutputObject.GetComponent<Text>());
                Destroy(oldOutputObject);
            }

            return;
        }

        if(GetInput() is FoodState)
        {
            RecipeModule.Food f = ((FoodState)GetInput()).GetFood();

            List<int> parameters;
            RecipeModule.Food outputFood;

            string append = "";

            switch (GetPseudoAction().GetActionType())
            {
                case RecipeModule.Action.ActionType.Boil:
                    append = "BO";
                    outputFood = null;
                    break;
                case RecipeModule.Action.ActionType.Break:
                    append = "BR";
                    outputFood = recipe.DescribeNewBreakAction(GetStepNumber(), f);
                    break;
                case RecipeModule.Action.ActionType.Chop:
                    append = "CH";
                    outputFood = recipe.DescribeNewChopAction(GetStepNumber(), f, 0, (RecipeModule.Chop.PieceVolumeSize)GetPseudoAction().GetParameterValues()[0]);
                    break;
                case RecipeModule.Action.ActionType.Cook:
                    append = "CO";
                    parameters = GetPseudoAction().GetParameterValues();
                    outputFood = recipe.DescribeNewCookAction(GetStepNumber(), f, parameters[0], parameters[1], (RecipeModule.Cook.CookType)parameters[2]);
                    break;
                case RecipeModule.Action.ActionType.Fry:
                    append = "FR";
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

            outputObject.GetComponent<VRTK_UIDraggableItem>().duplicateOnDrag = true;
            outputObject.GetComponent<VRTK_UIDraggableItem>().cantDuplicateAfterDrag = true;
            outputObject.GetComponent<VRTK_UIDraggableItem>().removeOnDropEmptyZone = false;

            outputObject.GetComponent<Text>().color = Color.red;
            outputObject.GetComponent<Text>().text += f.GetFoodIdentifier() + append;

            // Remove existing ouput object
            if (outputZoneRef.transform.childCount == 1) {
                GameObject oldOutputObject = outputZoneRef.transform.GetChild(0).gameObject;
                Destroy(oldOutputObject.GetComponent<Text>());
                Destroy(oldOutputObject);
            }

            // Swap output with new one
            outputObject.transform.SetParent(outputZoneRef.transform, false);
            outputObject.transform.GetComponent<VRTK_UIDraggableItem>().enabled = true;
        }

        else if(GetInput() is FoodGroup)
        {
            // TODO Implement FoodGroup
        }

        outputZoneRef.transform.parent.gameObject.SetActive(true);
    }

    // Get input of this step
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

    // Get output generated from this step
    public object GetOutput()
    {
        if (outputZoneRef.GetComponentInChildren<FoodState>())
        {
            return outputZoneRef.GetComponentInChildren<FoodState>();
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

    // Fired when a new item is put into this step, or a existing item is removed from this step.
    public void StepChanged()
    {
        this.SetDirty(true);
        stepManager.StepChanged(this);
    }

    // Remove this step from recipe
    public void Remove()
    {
        stepManager.RemoveStep(this);
    }

    // Get action of this step
    public PseudoAction GetPseudoAction()
    {
        return actionZoneRef.GetComponentInChildren<PseudoAction>();
    }
}