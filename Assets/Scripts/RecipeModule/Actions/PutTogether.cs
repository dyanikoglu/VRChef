using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Waiting for implementation of CanBePutTogether food characteristic.
namespace RecipeModule
{
    public class PutTogether : Action
    {

        public PutTogether()
        {
            this.actionType = ActionType.PutTogether;
        }
    }
}