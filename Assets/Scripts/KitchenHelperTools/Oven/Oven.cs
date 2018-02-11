using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Oven : MonoBehaviour {
    public Timer timerRef;
    public Heat heatRef;
    public GameObject coverRef;
    public GameObject startStopButtonRef;

    private List<GameObject> objectsToBeCooked;
    private bool heatEnabled = false;

	void Start () {
        objectsToBeCooked = new List<GameObject>();
        startStopButtonRef.GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OvenHeatOnOff;
    }

    private void OvenHeatOnOff(object sender, InteractableObjectEventArgs e)
    {
        if (heatEnabled)
        {
            OvenHeatOff();
        }
        else
        {
            OvenHeatOn();
        }
    }

    void Update () {
        // Cover is not closed correctly, stop heating.
        if (heatEnabled && coverRef.transform.localRotation.eulerAngles.x < 89.75f)
        {
            OvenHeatOff();
            return;
        }
	}

    public void OvenHeatOff()
    {
        heatEnabled = false;
        timerRef.StopTimer();

        foreach (GameObject o in objectsToBeCooked)
        {
            CanBeCooked cbc = o.GetComponent<CanBeCooked>();
            cbc.EndCook();
        }
    }

    public void OvenHeatOn()
    {
        heatEnabled = true;
        timerRef.StartTimer();

        foreach (GameObject o in objectsToBeCooked)
        {
            CanBeCooked cbc = o.GetComponent<CanBeCooked>();
            cbc.SetCurrentHeat(heatRef.GetValue());
            cbc.BeginCook(timerRef.GetValue());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CanBeCooked>())
        {
            objectsToBeCooked.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CanBeCooked>())
        {
            objectsToBeCooked.Remove(other.gameObject);
        }
    }

    public int GetCurrentTimerValue()
    {
        return timerRef.GetValue();
    }

    public int GetCurrentHeatValue()
    {
        return heatRef.GetValue();
    }
}