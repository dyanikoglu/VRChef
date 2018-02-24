using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class FoodCharacteristic : MonoBehaviour
{
    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }
}
