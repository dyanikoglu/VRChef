using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FoodStatus : MonoBehaviour { 
    private bool _isChoppedPiece = false;
    private bool _isFried = false;
    private bool _isSqueezed = false;
    private bool _isPeeled = false;
    private bool _isHalfSmashed = false;
    private bool _isBurnedAfterFrying = false;
    private bool _isBoiled = false;
    public bool _isSmashed = false;
    public bool _isCooked = false;
    private bool _isGrabbed = false;
    private bool _isMixed = false;

    // Unique identifier of the object. Required for reaching out prefabs of each food.
    public string foodIdentifier = "";
    

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


    public bool GetIsChoppedPiece()
    {
        return _isChoppedPiece;
    }

    public void SetIsChoppedPiece(bool isChoppedPiece)
    {    
        OnOperationDone(RecipeModule.Action.ActionType.Chop);
        this._isChoppedPiece = isChoppedPiece;
    }

    public void SetIsFried(bool flag)
    {
        OnOperationDone(RecipeModule.Action.ActionType.Fry);
        _isFried = flag;
    }

    public bool GetIsFried()
    {
        return _isFried;
    }

    public void SetIsCooked(bool flag)
    {
        Debug.Log("cooked");
        OnOperationDone(RecipeModule.Action.ActionType.Cook);
        _isCooked = flag;
    }

    public bool GetIsCooked()
    {
        return _isCooked;
    }

    public void SetIsBurned(bool flag)
    {
        _isBurnedAfterFrying = flag;
    }

    public bool GetIsBurned()
    {
        return _isBurnedAfterFrying;
    }

    public bool GetIsSqueezed()
    {
        return _isSqueezed;
    }

    public void SetIsSqueezed(bool isSqueezed)
    {
        OnOperationDone(RecipeModule.Action.ActionType.Squeeze);
        this._isSqueezed = isSqueezed;
    }

    public bool GetIsPeeled()
    {
        return _isPeeled;
    }

    public void SetIsPeeled(bool isPeeled)
    {
        OnOperationDone(RecipeModule.Action.ActionType.Peel);
        this._isPeeled = isPeeled;
    }

    public bool GetIsHalfSmashed()
    {
        return _isHalfSmashed;
    }

    public void SetIsHalfSmashed(bool isHalfSmashed)
    {
        this._isHalfSmashed = isHalfSmashed;
    }

    public bool GetIsSmashed()
    {
        return _isSmashed;
    }

    public void SetIsSmashed(bool isSmashed)
    {
        OnOperationDone(RecipeModule.Action.ActionType.Smash);
        this._isSmashed = isSmashed;
    }


    public bool GetIsBoiled()
    {
        return _isBoiled;
    }

    public void SetIsBoiled(bool isBoiled)
    {
        OnOperationDone(RecipeModule.Action.ActionType.Boil);
        this._isBoiled = isBoiled;
    }

    public bool GetIsMixed()
    {
        return _isMixed;
    }

    public void SetIsMixed(bool flag)
    {
        _isMixed = true;
    }
}
