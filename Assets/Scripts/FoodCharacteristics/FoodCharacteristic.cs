using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public abstract class FoodCharacteristic : MonoBehaviour
{
    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }
    /*
    public delegate void OperationDoneEventHandler(FoodCharacteristic fc, OperationEventArgs e);
    public virtual event OperationDoneEventHandler OperationDone;

    public virtual void OnOperationDone(RecipeModule.Action.ActionType type)
    {
        OperationEventArgs operationEventArgs = new OperationEventArgs(type);

        if (OperationDone != null)
        {
            OperationDone(GetComponent<FoodCharacteristic>(), operationEventArgs);
        }
    }



    public void OnMyUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        OnOperationDone(RecipeModule.Action.ActionType.PutTogether);
    }
        */


    public void Start()
    {
        //if (GetComponent<VRTK_InteractableObject>())
        //{
        //    GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += OnMyUngrabbed;
        //}
    }
}
