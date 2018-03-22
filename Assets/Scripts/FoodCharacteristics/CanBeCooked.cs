using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeCooked : FoodCharacteristic {
    private int _requiredCookTime;
    private Material[] _affectedMaterials;
    private int _currentHeat = 0;
    private int _neededUpdateCount = 0;
    private float _effectPercentage = 0;
    private float _deltaValue = 0;

    public int requiredCookTime = 300; // in seconds
    public int requiredCookHeat = 150; // in celcius
    public float cookingUpdateFreq = 0.5f; // in seconds
    public float cookingStartDelay = 0; // in seconds
    public Color cookEffectTint = Color.white;

	void Start () {
        base.Start();

        _requiredCookTime = requiredCookTime * 60;
        _affectedMaterials = GetComponent<Renderer>().materials;
    }

    private IEnumerator CookingTimer(int cookForSecs)
    {
        yield return new WaitForSeconds(cookForSecs);
        CancelInvoke("CookingUpdate");
    }

    public void BeginCook(int cookForSecs)
    {
        _effectPercentage = (float)_currentHeat / (float)requiredCookHeat;
        _neededUpdateCount = (int) ((float)requiredCookTime / cookingUpdateFreq);
        _deltaValue =  (0.8f / (float)(_neededUpdateCount)) * _effectPercentage;

        StartCoroutine(CookingTimer(cookForSecs));
        InvokeRepeating("CookingUpdate", cookingStartDelay, cookingUpdateFreq);

        foreach (Material mat in _affectedMaterials)
        {
            mat.SetColor("_WetTint", cookEffectTint);
        }
    }

    public void EndCook()
    {
        StopCoroutine("CookingTimer");
        CancelInvoke("CookingUpdate");
        _currentHeat = 0;

        GetComponent<FoodStatus>().SetIsCooked(true);

        //if(_affectedMaterials[0].GetFloat("_WetWeight") >= 0.7f && _affectedMaterials[0].GetFloat("_WetWeight") <= 0.9f)
        //{
        //    GetComponent<FoodStatus>().SetIsCooked(true);
        //}
    }

    public void SetCurrentHeat(int heat)
    {
        _currentHeat = heat;
    }

    // Add option for selecting/cooking multiple materials
    private void CookingUpdate()
    {
        foreach (Material mat in _affectedMaterials)
        {
            if (mat.GetFloat("_WetWeight") < 1.1f)
            {
                mat.SetFloat("_WetWeight", _affectedMaterials[0].GetFloat("_WetWeight") + _deltaValue);
            }
        }
    }
}