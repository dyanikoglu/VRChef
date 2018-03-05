using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour {

    public GameObject emptyStepRef;
    public GameObject newStepButtonRef;

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

        // Push down the new step button
        Vector3 buttonOffset = newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D;
        buttonOffset.y += spacingY;
        newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D = buttonOffset;

        steps.Add(newStep);
    }

    public void PushElement(RectTransform rt, Vector3 vec)
    {
        Vector3 offset = rt.anchoredPosition3D;
        offset += vec;
        rt.anchoredPosition3D = offset;
    }

    public void RemoveStep(Step s)
    {
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

        FixMissingReferences();
    }

    private void FixMissingReferences()
    {
        foreach(Step s in steps)
        {
            FoodState inputFoodState = s.inputZoneRef.GetComponentInChildren<FoodState>();
            if (inputFoodState && inputFoodState.GetOrigin() == null)
            {
                // If this step uses output from removed step, remove this step too.
                RemoveStep(s);
                break;
            }
        }
    }

    public void RegenerateSteps() 
    {
        recipe = new RecipeModule.Recipe("Test Recipe");
        foreach (Step s in steps)
        {
            s.GenerateOutput(recipe);
        }

        FixMissingReferences();
    }
}
