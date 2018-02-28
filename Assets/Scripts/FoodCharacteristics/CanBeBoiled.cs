using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeBoiled : FoodCharacteristic
{
    public int requiredBoilingTime;
    bool onCanBoil;
    GameObject canBoilObject;
    public bool boilingStarted;
    public bool boilingStopped;
    public Material boiledMaterial;

    private void Awake()
    {
        boilingStarted = false;
        boilingStopped = false;
        onCanBoil = false;
    }

    IEnumerator Boil()
    {
        boilingStarted = true;
        yield return new WaitForSeconds(requiredBoilingTime);
        GetComponent<Renderer>().material = boiledMaterial;
        foreach(FoodStatus f in GetComponentsInChildren<FoodStatus>())
        {
            f.SetIsBoiled(true);
        }
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
        StartCoroutine("Boil");
    }

    private void StopBoiling()
    {
        StopCoroutine("Boil");
        boilingStopped = true;
        boilingStarted = false;
    }
}
