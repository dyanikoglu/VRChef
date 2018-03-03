﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour {

    public GameObject emptyStepRef;
    public GameObject newStepButtonRef;
    private int totalStepCount = 0;

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
        offset.y -= totalStepCount * 40;
        rt.anchoredPosition3D = offset;

        // Assign gameobject name
        newStep.name = "Step_" + totalStepCount;

        // Push down the new step button
        Vector3 buttonOffset = newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D;
        buttonOffset.y -= 40;
        newStepButtonRef.GetComponent<RectTransform>().anchoredPosition3D = buttonOffset;
    }

    private void Attach(GameObject parent, GameObject child)
    {
        
    }
}
