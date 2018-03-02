using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public abstract class FoodCharacteristic : MonoBehaviour
{
    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }

    public delegate void OperationDoneEventHandler(object obj, OperationEventArgs e);
    public virtual event OperationDoneEventHandler OperationDone;

    protected virtual void OnOperationDone(RecipeModule.Action.ActionType type)
    {
        if (OperationDone != null)
        {
            OperationEventArgs operationEventArgs = new OperationEventArgs(type);
            OperationDone(this, operationEventArgs);
        }
    }

    //public abstract void PublishEvent();
}
