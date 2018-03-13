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

    public delegate void OperationDoneEventHandler(FoodCharacteristic fc, OperationEventArgs e);
    public virtual event OperationDoneEventHandler OperationDone;

    public virtual void OnOperationDone(RecipeModule.Action.ActionType type)
    {
        if (OperationDone != null)
        {
            OperationEventArgs operationEventArgs = new OperationEventArgs(type);
            OperationDone(GetComponent<FoodCharacteristic>(), operationEventArgs);
        }
    }

}
