using System;
using RecipeModule;

public class OperationEventArgs : EventArgs
{
    public RecipeModule.Action.ActionType OperationType { get; set; }

    // also get operation parameters like chopping size
    // how? maybe move parameter enums from action classes to here.

    public OperationEventArgs(RecipeModule.Action.ActionType type)
    {
        OperationType = type;
    }

}
