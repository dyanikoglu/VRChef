using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Oven : MonoBehaviour {
    public Timer timerRef;
    public Heat heatRef;
    public GameObject coverRef;
    public GameObject startStopButtonRef;

    public AudioClip ovenStart;
    public AudioClip ovenLoop;
    public AudioClip ovenStop;

    public Light heatLight;
    public float heatLightIntensity = 20;

    private List<GameObject> objectsToBeCooked;
    private bool heatEnabled = false;

	void Start () {
        objectsToBeCooked = new List<GameObject>();
        startStopButtonRef.GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OvenHeatOnOff;
        heatLight.intensity = 0;
    }

    private void OvenHeatOnOff(object sender, InteractableObjectEventArgs e)
    {
        if (heatEnabled)
        {
            OvenHeatOff();
        }
        else if(coverRef.transform.localRotation.eulerAngles.x >= 89.75f)
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

        if(heatEnabled && GetCurrentTimerValue() == 0)
        {
            OvenHeatOff();
            return;
        }
	}

    public void OvenHeatOff()
    {
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().clip = ovenStop;
        GetComponent<AudioSource>().Play();

        heatLight.intensity = 0;

        heatEnabled = false;
        timerRef.StopTimer();

        foreach (GameObject o in objectsToBeCooked)
        {
            CanBeCooked cbc = o.GetComponent<CanBeCooked>();
            cbc.EndCook();
        }
    }

    IEnumerator DelayedEffect()
    {
        yield return new WaitForSeconds(4.9f);

        if (heatEnabled)
        {
            GetComponent<AudioSource>().clip = ovenLoop;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }
    }

    public void OvenHeatOn()
    {
        if(GetCurrentTimerValue() < 5)
        {
            return;
        }

        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().clip = ovenStart;
        GetComponent<AudioSource>().Play();
        StartCoroutine(DelayedEffect());

        heatLight.intensity = heatLightIntensity;

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