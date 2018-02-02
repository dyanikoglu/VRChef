using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class AutoGrab : VRTK_ObjectAutoGrab
{
    bool onlyOnce = true;

    private void Update()
    {
        if (onlyOnce)
        {
            if (isActiveAndEnabled)
            {
                base.OnEnable();
                onlyOnce = false;
            }
        }

    }
}
