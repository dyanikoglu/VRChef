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

    public bool parsed = false;

    public RecipeManager recipeManager;

    // This step needs to be regenerated if it's true
    public bool dirty = false;

    // Denotes that if this step is already grouped. Grouped steps can't be grouped again.
    public bool hasGroup = false;

    // Template for generating new output food object in each step
    public GameObject dummyOutputSingleFood;

    // Template for generating new output foodgroup object in each step
    public GameObject dummyOutputFoodGroup;

    public void SetStepNumber(int stepNumber)
    {
        if(stepNumber == -1)
        {
            this.stepNumberRef.text = "-";
            return;
        }

        this.stepNumberRef.text = stepNumber.ToString();
    }

    public bool GetHasGroup()
    {
        return hasGroup;
    }

    public void SetHasGroup(bool hasGroup)
    {
        this.hasGroup = hasGroup;
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

    public void OpenActionPopup()
    {
        // If there is an action
        if(GetPseudoAction())
        {
            PseudoAction pa = GetPseudoAction();
            ActionPopup ap = recipeManager.ShowActionPopup(pa.GetActionType().ToString(), pa.GetParameterNames(), pa.GetParameterValues(), pa.GetParameterMins(), pa.GetParameterMaxs());

            // If ap is not available, return
            if(ap == null)
            {
                return;
            }

            ap.SetPseudoActionRef(pa);
        }
    }

    // Generate output of this step, register it to recipe data structure.
    public bool GenerateOutput()
    {
        // Input or action is null, halt.
        if (GetInput() == null || GetPseudoAction() == null)
        {
            // Remove existing output object if it exists
            if (outputZoneRef.transform.childCount == 1)
            {
                GameObject oldOutputObject = outputZoneRef.transform.GetChild(0).gameObject;
                Destroy(oldOutputObject.GetComponent<Text>());
                Destroy(oldOutputObject);
            }

            return false;
        }

        // If input is single object
        if(GetInput() is FoodState)
        {
            // Create new output food object
            GameObject outputObject = GameObject.Instantiate(dummyOutputSingleFood);

            // Set new output name
            string stateName = "";
            if(GetPseudoAction().GetActionType() == RecipeModule.Action.ActionType.Empty)
            {
                stateName = ((FoodState)GetInput()).gameObject.GetComponent<Text>().text;
            }
            else
            {
                stateName = "Output_" + recipeManager.GetNewOutputName();
            }

            outputObject.name = stateName;
            outputObject.GetComponent<Text>().text = stateName;
            outputObject.GetComponent<Text>().color = Color.red;

            // Remove existing ouput object
            if (outputZoneRef.transform.childCount == 1) {
                GameObject oldOutputObject = outputZoneRef.transform.GetChild(0).gameObject;
                Destroy(oldOutputObject.GetComponent<Text>());
                Destroy(oldOutputObject);
            }

            // Enable selecting this step for grouping
            toggleRef.GetComponent<Toggle>().enabled = true;

            // Swap output with new one
            outputObject.transform.SetParent(outputZoneRef.transform, false);
            outputObject.transform.GetComponent<VRTK_UIDraggableItem>().enabled = true;
        }

        // If input is food group
        else if(GetInput() is FoodStateGroup)
        {
            FoodStateGroup inputGroup = (FoodStateGroup)GetInput();
            List<FoodState> outputFoods = new List<FoodState>();

            // Create new list from input objects in group
            foreach (FoodState fs in inputGroup.foodStates)
            {
                outputFoods.Add(fs);
            }

            // Create new foodgroup object
            GameObject outputObject = GameObject.Instantiate(dummyOutputFoodGroup);
            outputObject.GetComponent<FoodStateGroup>().SetFoodStateGroup(outputFoods);

            // Set new output group name
            string stateName;
            if (GetPseudoAction().GetActionType() == RecipeModule.Action.ActionType.Empty)
            {
                stateName = inputGroup.gameObject.GetComponent<Text>().text;
            }
            else
            {
                stateName = "Output_" + recipeManager.GetNewOutputName();
            }

            outputObject.name = stateName;
            outputObject.GetComponent<Text>().text = stateName;
            outputObject.GetComponent<Text>().color = Color.red;

            // Remove existing ouput object
            if (outputZoneRef.transform.childCount == 1)
            {
                GameObject oldOutputObject = outputZoneRef.transform.GetChild(0).gameObject;
                Destroy(oldOutputObject.GetComponent<Text>());
                Destroy(oldOutputObject);
            }

            // Enable selecting this step for grouping
            toggleRef.GetComponent<Toggle>().enabled = true;

            // Swap output with new one
            outputObject.transform.SetParent(outputZoneRef.transform, false);
            outputObject.transform.GetComponent<VRTK_UIDraggableItem>().enabled = true;
        }

        outputZoneRef.transform.parent.gameObject.SetActive(true);

        CheckGroupEligibility();

        return true;
    }

    // Checks required conditions for grouping of this step. If conditions are not met, reverts toggle back to false.
    public void CheckGroupEligibility()
    {
        if(!recipeManager.CheckGroupEligibility(this))
        {
            // Not eligible for grouping, set toggle as false back.
            SetToggle(false);
            toggleRef.interactable = false;
        }

        else
        {
            toggleRef.interactable = true;
        }

        // Refresh new group button
        recipeManager.NewGroupButtonVisibility();
    }


    // Get input of this step
    public object GetInput()
    {
        if(inputZoneRef.GetComponentInChildren<FoodState>())
        {
            return inputZoneRef.GetComponentInChildren<FoodState>();
        }

        else if(inputZoneRef.GetComponentInChildren<FoodStateGroup>())
        {
            return inputZoneRef.GetComponentInChildren<FoodStateGroup>();
        }

        else
        {
            // Unknown input type
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

        else if (outputZoneRef.GetComponentInChildren<FoodStateGroup>())
        {
            return outputZoneRef.GetComponentInChildren<FoodStateGroup>();
        }

        else
        {
            // Unknown output type
            return null;
        }
    }

    // Fired when a new item is put into this step, or a existing item is removed from this step.
    public void StepChanged()
    {
        recipeManager.StepChanged(this);
    }

    // Remove this step from recipe
    public void Remove()
    {
        recipeManager.RemoveStep(this);
    }

    // Get action of this step
    public PseudoAction GetPseudoAction()
    {
        return actionZoneRef.GetComponentInChildren<PseudoAction>();
    }
}