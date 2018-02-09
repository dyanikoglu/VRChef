using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeCooked : FoodCharacteristic {
    private int _requiredCookTime;
    private Material[] _affectedMaterials;

    public int requiredCookTime = 5; // in minutes
    public float cookingUpdateFreq = 0.5f; // in seconds
    public float cookingStartDelay = 0; // in seconds
    public float cookEffectDelta = 0.01f;
    public Color cookEffectTint = Color.white;
	// Use this for initialization
	void Start () {
        _requiredCookTime = requiredCookTime * 60;
        _affectedMaterials = GetComponent<Renderer>().materials;

        BeginCook(2);
    }

    private IEnumerator CookingTimer(int cookForMinutes)
    {
        yield return new WaitForSeconds(cookForMinutes * 60);
        CancelInvoke("CookingUpdate");
    }

    public void BeginCook(int cookForMinutes)
    {
        StartCoroutine(CookingTimer(cookForMinutes));
        InvokeRepeating("CookingUpdate", cookingStartDelay, cookingUpdateFreq);
        _affectedMaterials[0].SetColor("_WetTint", cookEffectTint);
    }

    // Add option for selecting multiple materials
    private void CookingUpdate()
    {
        if (_affectedMaterials[0].GetFloat("_WetWeight") < 1f)
        {
            _affectedMaterials[0].SetFloat("_WetWeight", _affectedMaterials[0].GetFloat("_WetWeight") + cookEffectDelta);
        }
    }
}