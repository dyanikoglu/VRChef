using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour {

    public GameObject emptyStepRef;
    public GameObject emptyGroupRef;
    public GameObject newStepButtonRef;
    public GameObject newGroupButtonRef;

    public RecipeModule.Recipe recipe;

    private List<Step> steps;

    private int totalStepCount = 0;
    public int spacingY = -40;

    private void Start()
    {
        recipe = new RecipeModule.Recipe("Test Recipe");
        steps = new List<Step>();
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
        newStep.toggleRef.GetComponent<Toggle>().enabled = false;

        // Push down the new step button
        Vector3 buttonOffset = newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D;
        buttonOffset.y += spacingY;
        newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D = buttonOffset;

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
        foreach (Step s in steps)
        {
            if (s.GetToggle())
            {
                s.outputGrouped = true;
                s.SetToggle(false);
                s.toggleRef.GetComponent<Toggle>().enabled = false;

                //Import single output into list
                if(s.GetOutput() is FoodState)
                {
                    outputsToBeGrouped.Add((FoodState)(s.GetOutput()));
                }

                // Import whole group into list
                else if(s.GetOutput() is FoodGroup)
                {
                    FoodGroup groupToBeImported = (FoodGroup)(s.GetOutput());
                    foreach (FoodState fs in groupToBeImported.foodGroup)
                    {
                        outputsToBeGrouped.Add(fs);
                    }
                }

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

        GameObject newGroup = GameObject.Instantiate(emptyGroupRef);
        newGroup.transform.SetParent(this.transform, false);

        Vector3 groupPos = newGroup.GetComponent<RectTransform>().anchoredPosition3D;
        groupPos.y = avgYPos;
        newGroup.GetComponent<RectTransform>().anchoredPosition3D = groupPos;

        Vector2 newDelta = new Vector2((maxY - minY) / 10.0f, 10) ;
        newGroup.GetComponent<GroupFromSteps>().verticalLineRef.sizeDelta = newDelta;

        // Set generated list as group members
        newGroup.GetComponent<GroupFromSteps>().SetFoodGroup(outputsToBeGrouped);

        newGroup.SetActive(true);
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

        RegenerateSteps();
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

        else if(s.GetInput() is FoodGroup)
        {      
            // TODO
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

        else if(s.GetOutput() is FoodGroup)
        {
            //TODO
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

    public void DestroyItem(GameObject o)
    {
        Destroy(o.GetComponent<Text>());
        Destroy(o);
    }

    // Assume we removed the parameter item, remove recursively the clone and generated outputs from this clone on recipe.
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

                else if (s.GetOutput() is FoodGroup)
                {
                    //TODO
                }
            }
        }
    }

    public void RegenerateSteps() 
    {
        StartCoroutine(_RegenerateSteps());
    }

    private IEnumerator _RegenerateSteps()
    {
        // Some delay for consistency of data.
        yield return new WaitForSeconds(0.1f);
        recipe = new RecipeModule.Recipe("Test Recipe");
        foreach (Step s in steps)
        {
            // If this step is marked as dirty, recalculate it's output again.
            if (s.IsDirty())
            {
                s.GenerateOutput(recipe);
                s.SetDirty(false);
            }
        }
    }
}
