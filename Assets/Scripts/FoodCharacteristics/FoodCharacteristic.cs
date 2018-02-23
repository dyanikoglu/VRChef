using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class FoodCharacteristic : MonoBehaviour
{
    private bool _isChoppedPiece = false;
    private bool _isFried = false;
    private bool _isSqueezed = false;
    private bool _isPeeled = false;

    // Keep adding required characteristic status booleans
    //...
    //...
    //...

    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }

    public bool GetIsChoppedPiece()
    {
        return _isChoppedPiece;
    }

    public void SetIsChoppedPiece(bool isChoppedPiece)
    {
        this._isChoppedPiece = isChoppedPiece;
    }

    public void SetIsFried(bool flag)
    {
        _isFried = flag;
    }

    public bool GetIsFried()
    {
        return _isFried;
    }

    public bool GetIsSqueezed()
    {
        return _isSqueezed;
    }

    public void SetIsSqueezed(bool isSqueezed)
    {
        this._isSqueezed = isSqueezed;
    }

    public bool GetIsPeeled()
    {
        return _isPeeled;
    }

    public void SetIsPeeled(bool isPeeled)
    {
        this._isPeeled = isPeeled;
    }
}
