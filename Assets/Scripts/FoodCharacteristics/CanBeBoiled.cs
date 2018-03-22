using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeBoiled : FoodCharacteristic
{
    public float requiredBoilingTime;
    bool onCanBoil;
    GameObject canBoilObject;
    public bool boilingStarted;
    public bool boilingStopped;
    public Color boilEffectTint = Color.white;
    public Material boiledMaterial;
    float blend = 0;


    private void Awake()
    {
        boilingStarted = false;
        boilingStopped = false;
        onCanBoil = false;
    }

    IEnumerator Boil()
    {
        boilingStarted = true;
        float fadeValue = 0f;

        while (blend < 0.5f)
        {
            fadeValue = Time.deltaTime / requiredBoilingTime;
            blend += fadeValue;

            //GetComponent<Renderer>().material.SetColor("_WetTint", boilEffectTint);
            GetComponent<Renderer>().material.SetFloat("_WetWeight", GetComponent<Renderer>().material.GetFloat("_WetWeight") + fadeValue);
            yield return new WaitForSeconds(fadeValue);
            //yield return new WaitForSeconds(requiredBoilingTime);
        }

        //GetComponent<Renderer>().material = boiledMaterial;

        foreach (FoodStatus f in GetComponentsInChildren<FoodStatus>())
        {
            f.SetIsBoiled(true);
        }
        //GetComponent<FoodStatus>().SetIsBoiled(true);

        boilingStarted = false;
        boilingStopped = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanBoil>())
        {
            onCanBoil = true;
            canBoilObject = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanBoil>())
        {
            onCanBoil = false;
        }
    }

    private void Update()
    {
        if (onCanBoil)
        {
            if (!GetComponent<FoodStatus>().GetIsBoiled() && !boilingStarted && canBoilObject.GetComponent<CanBoil>().GetCanBoil() && canBoilObject.GetComponent<CanBoil>().isWaterBoiled)
            {
                StartBoiling();
            }
            else if (!canBoilObject.GetComponent<CanBoil>().GetCanBoil())
            {
                StopBoiling();
            }
        }
        else
        {
            if (boilingStarted && !boilingStopped)
            {
                StopBoiling();
            }
        }
    }

    private void StartBoiling()
    {
        boilingStopped = false;
        boilingStarted = true;
        GetComponent<Renderer>().material.SetColor("_WetTint", boilEffectTint);
        StartCoroutine("Boil");
    }

    private void StopBoiling()
    {
        StopCoroutine("Boil");
        boilingStopped = true;
        boilingStarted = false;
    }
}
