using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Waiting for implementation of CanBeMixed Food Characteristic
namespace RecipeModule
{
    [FullSerializer.fsObject]
    public class Mix : Action
    {

        public Mix()
        {
            this.actionType = ActionType.Mix;
        }
    }
}
