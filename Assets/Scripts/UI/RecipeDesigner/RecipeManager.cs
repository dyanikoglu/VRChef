﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour {
    public GameObject emptyStepRef;
    public GameObject emptyGroupRef;
    public GameObject newStepButtonRef;
    public GameObject newGroupButtonRef;
    public GameObject actionPopupRef;

    private List<Step> steps;
    private List<GroupFromSteps> groups;

    private int totalStepCount = 0;
    private int outputNameSequence = 1;
    private char groupNameSequence = 'A';
    public int spacingY = -40;

    private void Start()
    {
        steps = new List<Step>();
        groups = new List<GroupFromSteps>();
    }

    public void CreateNewStep()
    {
        // Do not block UI, run in seperate thread.
        StartCoroutine(_CreateNewStep());
    }

    private IEnumerator _CreateNewStep()
    {
        yield return null;

        GameObject newStepObject = GameObject.Instantiate(emptyStepRef);
        Step newStep = newStepObject.GetComponent<Step>();

        newStep.SetStepNumber(++totalStepCount);

        // Set new parent as GUI panel
        newStepObject.transform.SetParent(this.transform, false);

        // Push down new step object
        RectTransform rt = newStepObject.GetComponent<RectTransform>();
        Vector3 offset = emptyStepRef.GetComponent<RectTransform>().anchoredPosition3D;
        offset.y += totalStepCount * spacingY;
        rt.anchoredPosition3D = offset;

        // Assign gameobject name
        newStep.name = "Step_" + totalStepCount;

        // Disable toggle
        newStep.toggleRef.GetComponent<Toggle>().interactable = false;

        // Push down the new step button
        Vector3 buttonOffset = newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D;
        buttonOffset.y += spacingY;
        newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D = buttonOffset;

        //Activate the step object, fix toggle checkmark
        newStepObject.SetActive(true);
        newStepObject.GetComponent<Step>().toggleRef.enabled = true;

        steps.Add(newStep);
    }

    // Push step element upwards in UI
    public void PushElement(RectTransform rt, Vector3 vec)
    {
        Vector3 offset = rt.anchoredPosition3D;
        offset += vec;
        rt.anchoredPosition3D = offset;
    }

    // Create a new foodgroup from selected steps
    public void CreateGroupFromSelectedSteps()
    {
        float avgYPos = 0;
        float minY = 10000;
        float maxY = -10000;
        List<FoodState> outputsToBeGrouped = new List<FoodState>();

        GameObject newGroup = GameObject.Instantiate(emptyGroupRef);
        newGroup.GetComponent<GroupFromSteps>().boundedSteps = new List<Step>();

        foreach (Step s in steps)
        {
            if (s.GetToggle())
            {
                s.SetHasGroup(true);
                s.SetToggle(false);
                s.toggleRef.GetComponent<Toggle>().interactable = false;

                //Import single output into list
                if(s.GetOutput() is FoodState)
                {
                    outputsToBeGrouped.Add(((FoodState)(s.GetOutput())));
                }

                // Import a whole output group into list
                else if(s.GetOutput() is FoodStateGroup)
                {
                    FoodStateGroup groupToBeImported = (FoodStateGroup)(s.GetOutput());
                    foreach (FoodState fs in groupToBeImported.foodStates)
                    {
                        outputsToBeGrouped.Add(fs);
                    }
                }

                // Add this step as bounded step to group
                newGroup.GetComponent<GroupFromSteps>().boundedSteps.Add(s);

                Vector3 pos = s.GetComponent<RectTransform>().anchoredPosition3D;
                if(pos.y > maxY)
                {
                    maxY = pos.y + 25;
                }

                if(pos.y < minY)
                {
                    minY = pos.y + 25;
                }

                s.groupConnectorRef.SetActive(true);
            }
        }

        avgYPos = (minY + maxY) / 2.0f;

        newGroup.transform.SetParent(this.transform, false);

        Vector3 groupPos = newGroup.GetComponent<RectTransform>().anchoredPosition3D;
        groupPos.y = avgYPos;
        newGroup.GetComponent<RectTransform>().anchoredPosition3D = groupPos;

        Vector2 newDelta = new Vector2((maxY - minY) / 10.0f, 10) ;
        newGroup.GetComponent<GroupFromSteps>().verticalLineRef.sizeDelta = newDelta;

        // Set generated list as group members
        newGroup.GetComponent<GroupFromSteps>().GetFoodStateGroup().SetFoodStateGroup(outputsToBeGrouped);

        // Set names
        string groupStateName = GetNewGroupName();
        newGroup.GetComponent<GroupFromSteps>().GetFoodStateGroup().GetComponent<Text>().text = "Group_" + groupStateName;
        newGroup.name = "Group_" + groupStateName;

        // Add this group to groups list
        groups.Add(newGroup.GetComponent<GroupFromSteps>());

        newGroup.SetActive(true);
    }

    // Show action settings popup
    public ActionPopup EnableActionPopup(string actionName, List<string> paramNames, List<int> paramValues)
    {
        actionPopupRef.GetComponent<ActionPopup>().headerRef.text = actionName;
        ActionPopup actionPopup = actionPopupRef.GetComponent<ActionPopup>();

        // Add parameter settings for this action type
        for (int i = 0; i < paramValues.Count; i++)
        {
            GameObject newParamObj = GameObject.Instantiate(actionPopup.parameterPrefab);
            newParamObj.transform.SetParent(actionPopup.parametersRef.transform, false);
            Vector3 pos = newParamObj.GetComponent<RectTransform>().anchoredPosition3D;
            pos.y = actionPopup.paramStartY + (i * actionPopup.paramIntervalY);
            newParamObj.GetComponent<RectTransform>().anchoredPosition3D = pos;

            ActionParameter ap = newParamObj.GetComponent<ActionParameter>();
            ap.headerRef.text = paramNames[i];
            ap.SetParamIndex(i);

            // TODO Should I do something with slider min-max value, or leave them at some default bound?

            ap.sliderRef.value = paramValues[i];
        }

        actionPopupRef.SetActive(true);

        return actionPopup;
    }

    public void DisableActionPopup()
    {
        ActionPopup actionPopup = actionPopupRef.GetComponent<ActionPopup>();

        // Reverse iteration, clear parameter objects
        for (int i = actionPopup.parametersRef.transform.childCount - 1; i >= 0; i--)
        {
            DestroyItem(actionPopup.parametersRef.transform.GetChild(i).gameObject);
        }

        actionPopupRef.SetActive(false);
    }
    
    public bool CheckGroupEligibility(Step s)
    {
        // If this step is already grouped, we cannot regroup it.
        if(s.GetHasGroup())
        {
            return false;
        }

        // If output is used in another step, we can't group this step anymore.
        if(s.GetOutput() is FoodState)
        {
            FoodState fs = (FoodState)s.GetOutput();

            if(fs.clone != null)
            {
                return false;
            }
        }

        else if (s.GetOutput() is FoodStateGroup)
        {
            FoodStateGroup fsg = (FoodStateGroup)s.GetOutput();

            if (fsg.clone != null)
            {
                return false;
            }
        }

        // There is no output
        else
        {
            return false;
        }

        // It can be grouped
        return true;
    }

    // Set new group button as visible if 2 or more steps are selected
    public void NewGroupButtonVisibility()
    {
        List<Step> tickedSteps = new List<Step>();
        float avgYPos = 0;
        foreach(Step s in steps)
        {
            if(s.GetToggle())
            {
                tickedSteps.Add(s);
                avgYPos += s.GetComponent<RectTransform>().anchoredPosition3D.y + 25;
            }
        }

        avgYPos /= (float)tickedSteps.Count;

        if (tickedSteps.Count >= 2)
        {
            newGroupButtonRef.SetActive(true);
            Vector3 pos = newGroupButtonRef.GetComponent<RectTransform>().anchoredPosition3D;
            pos = new Vector3(pos.x, avgYPos, pos.z);
            newGroupButtonRef.GetComponent<RectTransform>().anchoredPosition3D = pos;
        }

        else
        {
            newGroupButtonRef.SetActive(false);
        }
    }

    public void RemoveGroup(FoodStateGroup fsg)
    {
        // Mark references of this group as dirty
        if (fsg.clone)
        {
            MarkRefsAsDirty(fsg.clone);
            DestroyItem(fsg.clone.gameObject);
        }

        GroupFromSteps gfs = fsg.transform.parent.parent.GetComponent<GroupFromSteps>();

        // Destroy main group object
        groups.Remove(gfs);
        DestroyItem(gfs.gameObject);

        // Destroy grouping lines
        foreach (Step s in gfs.boundedSteps)
        {
            s.hasGroup = false;
            s.toggleRef.GetComponent<Toggle>().interactable = true;
            s.groupConnectorRef.SetActive(false);
        }

        // Regenerate Steps
        RegenerateSteps();
    }

    public void CheckGroupConsistency()
    {
        for(int i = groups.Count - 1; i >= 0; i--)
        {
            foreach (Step s in groups[i].boundedSteps)
            {
                if (s.GetOutput() == null)
                {
                    RemoveGroup(groups[i].GetFoodStateGroup());
                }
            }
        }
    }

    // Completely remove the step.
    public void RemoveStep(Step s)
    {
        StepChanged(s);

        int removedStepNo = s.GetStepNumber();

        foreach (Step step in steps)
        {
            int currentStepNumber = step.GetStepNumber();
            if (currentStepNumber > removedStepNo)
            {
                // Push up each step by difference with removed step number
                PushElement(step.GetComponent<RectTransform>(), new Vector3(0, -spacingY, 0));
                step.SetStepNumber(currentStepNumber - 1);
            }
        }
        PushElement(newStepButtonRef.GetComponent<RectTransform>(), new Vector3(0, -spacingY, 0));

        steps.Remove(s);
        Destroy(s.gameObject);
        totalStepCount--;

        // RegenerateSteps();
    }

    public void StepChanged(Step s)
    {
        s.SetDirty(true);

        // Check for input item conditions
        if (s.GetInput() is FoodState)
        {
            FoodState input = (FoodState)(s.GetInput());
            if (ItemClonedIntoPreviousStep(input, s.GetStepNumber()))
            {
                DestroyItem(input.gameObject);
            }
        }

        else if(s.GetInput() is FoodStateGroup)
        {
            FoodStateGroup input = (FoodStateGroup)(s.GetInput());
            if (GroupClonedIntoPreviousStep(input, s.GetStepNumber()))
            {
                DestroyItem(input.gameObject);
            }
        }

        // Wrong type of item is dropped into input zone
        else
        {
            if (s.inputZoneRef.transform.childCount == 1)
            {
                DestroyItem(s.inputZoneRef.transform.GetChild(0).gameObject);
            }
        }


        // Check for output item conditions
        if(s.GetOutput() is FoodState)
        {
            FoodState outputToBeRemoved = (FoodState)(s.GetOutput());
            MarkRefsAsDirty(outputToBeRemoved.clone);
            DestroyItem(outputToBeRemoved.gameObject);
        }

        else if(s.GetOutput() is FoodStateGroup)
        {
            FoodStateGroup outputToBeRemoved = (FoodStateGroup)(s.GetOutput());
            MarkRefsAsDirty(outputToBeRemoved.clone);
            DestroyItem(outputToBeRemoved.gameObject);
        }

        // Wrong type of item is dropped into action zone
        if(s.GetPseudoAction() == null)
        {
            if (s.actionZoneRef.transform.childCount == 1)
            {
                DestroyItem(s.actionZoneRef.transform.GetChild(0).gameObject);
            }
        } 

        // Finally recalculate outputs of each step
        RegenerateSteps();
    }

    // This function returns true if item given as parameter is cloned into a previous step.
    public bool ItemClonedIntoPreviousStep(FoodState fs, int stepNo)
    {
        foreach(Step s in steps)
        {
            if(s.GetOutput() is FoodState)
            {
                FoodState outputFood = (FoodState)(s.GetOutput());
                if(outputFood.clone == fs && stepNo < s.GetStepNumber())
                {
                    return true;
                }
            }
        }

        return false;
    }

    // This function returns true if item given as parameter is cloned into a previous step.
    public bool GroupClonedIntoPreviousStep(FoodStateGroup fsg, int stepNo)
    {
        // Check if clone is before last step in bounded steps in food group.
        foreach (GroupFromSteps g in groups)
        {
            FoodStateGroup foodGroup = g.GetFoodStateGroup();
            if (foodGroup.clone == fsg && stepNo < g.GetLastStepNumber())
            {
                return true;
            }
        }

        // Check if clone is before owner step of output object.
        foreach (Step s in steps)
        {
            if (s.GetOutput() is FoodStateGroup)
            {
                FoodStateGroup outputGroup = (FoodStateGroup)(s.GetOutput());
                if (outputGroup.clone == fsg && stepNo < s.GetStepNumber())
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Destroy function specific to our item objects(actions & foods) in recipe UI.
    public void DestroyItem(GameObject o)
    {
        if (o.GetComponent<Text>())
        {
            Destroy(o.GetComponent<Text>());
        }

        Destroy(o);
    }

    // Assume we removed the item given as parameter, destroy recursively the clone and generated outputs from this clone on recipe UI.
    public void MarkRefsAsDirty(FoodState foodState)
    {
        if (foodState == null)
        {
            return;
        }

        foreach (Step s in steps)
        {
            if ((FoodState)(s.GetInput()) == foodState)
            {
                s.SetDirty(true);

                if (s.GetOutput() is FoodState)
                {
                    FoodState outputToBeRemoved = (FoodState)(s.GetOutput());
                    FoodState inputToBeRemoved = (FoodState)(s.GetInput());

                    MarkRefsAsDirty(outputToBeRemoved.clone);
                    DestroyItem(outputToBeRemoved.gameObject);
                    DestroyItem(inputToBeRemoved.gameObject);
                    break;
                }

                else if (s.GetOutput() is FoodStateGroup)
                {
                    FoodStateGroup outputToBeRemoved = (FoodStateGroup)(s.GetOutput());
                    FoodStateGroup inputToBeRemoved = (FoodStateGroup)(s.GetInput());

                    MarkRefsAsDirty(outputToBeRemoved.clone);
                    DestroyItem(outputToBeRemoved.gameObject);
                    DestroyItem(inputToBeRemoved.gameObject);
                    break;
                }
            }
        }
    }

    public void MarkRefsAsDirty(FoodStateGroup foodStateGroup)
    {
        if (foodStateGroup == null)
        {
            return;
        }

        foreach (Step s in steps)
        {
            if (s.GetInput() is FoodStateGroup && (FoodStateGroup)(s.GetInput()) == foodStateGroup)
            {
                s.SetDirty(true);

                if (s.GetOutput() is FoodState)
                {
                    FoodState outputToBeRemoved = (FoodState)(s.GetOutput());
                    FoodState inputToBeRemoved = (FoodState)(s.GetInput());

                    MarkRefsAsDirty(outputToBeRemoved.clone);
                    DestroyItem(outputToBeRemoved.gameObject);
                    DestroyItem(inputToBeRemoved.gameObject);
                    break;
                }

                else if (s.GetOutput() is FoodStateGroup)
                {
                    FoodStateGroup outputToBeRemoved = (FoodStateGroup)(s.GetOutput());
                    FoodStateGroup inputToBeRemoved = (FoodStateGroup)(s.GetInput());

                    MarkRefsAsDirty(outputToBeRemoved.clone);
                    DestroyItem(outputToBeRemoved.gameObject);
                    DestroyItem(inputToBeRemoved.gameObject);
                    break;
                }
            }
        }
    }

    public string GetNewGroupName()
    {
        string s = groupNameSequence++.ToString();
        return s;
    }

    public string GetNewOutputName()
    {
        string s = outputNameSequence++.ToString();
        return s;
    }

    public void RegenerateSteps() 
    {
        StartCoroutine(_RegenerateSteps());
    }

    private IEnumerator _RegenerateSteps()
    {
        // Some delay for consistency of data.
        yield return new WaitForSeconds(0.1f);

        foreach (Step s in steps)
        {
            // If this step is marked as dirty, recalculate it's output again.
            if (s.IsDirty())
            {
                s.GenerateOutput();
                s.SetDirty(false);
            }
        }

        // Check if any created group affected from this action
        CheckGroupConsistency();
    }
}