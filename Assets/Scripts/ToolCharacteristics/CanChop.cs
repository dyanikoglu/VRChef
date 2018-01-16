using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanChop : ToolCharacteristic {
    public bool chopOnlyWhileToolOnHand = true;

    public bool IsToolAvailable()
    {
        if (chopOnlyWhileToolOnHand && !GetIsGrabbed())
        {
            return false;
        }

        return true;
    }

    // TODO Override OnGrab event, and add time delay for chopping stuff on first grab.
}
