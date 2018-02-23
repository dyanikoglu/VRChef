using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class FoodCharacteristic : MonoBehaviour
{
    private bool _isChoppedPiece = false;
    private bool _isFried = false;

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
}
