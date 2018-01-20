﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanChop : ToolCharacteristic {
    public bool chopOnlyWhileToolOnHand = true;

    public int currentChoppingObjectCount = 0;

    public bool IsToolAvailable()
    {
        if (chopOnlyWhileToolOnHand && !GetIsGrabbed())
        {
            return false;
        }

        return true;
    }

    public void FixedUpdate()
    {
        print(GetComponent<Rigidbody>().velocity);
    }

    public bool GetCurrentChoppingState()
    {
        return currentChoppingObjectCount != 0;
    }

    // TODO Override OnGrab event, and add time delay for chopping stuff on first grab.
}
