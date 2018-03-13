using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ToolCharacteristic : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetIsGrabbed()
    {
        return GetComponent<VRTK_InteractableObject>().IsGrabbed();
    }
}
