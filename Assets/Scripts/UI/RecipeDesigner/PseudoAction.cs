using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoAction : MonoBehaviour {
    public RecipeModule.Action.ActionType actionType;

    /* Cook: [0] -> requiredHeat | [1] -> requiredTime
     * Fry: [0] -> requiredTime
     * Boil: [0] -> requiredTime
     * Chop: [0] -> Piece Count
     * ...
     * ..
     * .
     * 
     */

    public List<int> parameterValues;
    
    // Names of parameters that will be showed up in action popup
    public List<string> parameterNames;

    // Possible minimum values of parameters in action popup slider
    public List<int> parameterMinimums;

    // Possible maximum values of parameters in action popup slider
    public List<int> parameterMaximums;

    public void SetAsChop(int pieceCount = 3)
    {
        this.actionType = RecipeModule.Action.ActionType.Chop;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Piece Size");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(pieceCount);

        this.parameterMinimums = new List<int>();
        this.parameterMinimums.Add(2);
        this.parameterMaximums = new List<int>();
        this.parameterMaximums.Add(20);
    }

    public void SetAsFry(int requiredTime = 30)
    {
        this.actionType = RecipeModule.Action.ActionType.Fry;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredTime);

        this.parameterMinimums = new List<int>();
        this.parameterMinimums.Add(1);
        this.parameterMaximums = new List<int>();
        this.parameterMaximums.Add(300);
    }

    public void SetAsCook(int requiredHeat = 150, int requiredTime = 30)
    {
        this.actionType = RecipeModule.Action.ActionType.Cook;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);

        this.parameterMinimums = new List<int>();
        this.parameterMinimums.Add(80);
        this.parameterMinimums.Add(1);

        this.parameterMaximums = new List<int>();
        this.parameterMaximums.Add(240);
        this.parameterMaximums.Add(300);
    } 

    public void SetAsPeel()
    {
        this.actionType = RecipeModule.Action.ActionType.Peel;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();

        this.parameterMinimums = new List<int>();
        this.parameterMaximums = new List<int>();
    }

    public void SetAsSmash()
    {
        this.actionType = RecipeModule.Action.ActionType.Smash;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();

        this.parameterMinimums = new List<int>();
        this.parameterMaximums = new List<int>();
    }

    public void SetAsSqueeze()
    {
        this.actionType = RecipeModule.Action.ActionType.Squeeze;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();

        this.parameterMinimums = new List<int>();
        this.parameterMaximums = new List<int>();
    }

    public void SetAsBreak()
    {
        this.actionType = RecipeModule.Action.ActionType.Break;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();

        this.parameterMinimums = new List<int>();
        this.parameterMaximums = new List<int>();
    }

    public void SetAsBoil(int requiredTime = 30)
    {
        this.actionType = RecipeModule.Action.ActionType.Boil;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredTime);

        this.parameterMinimums = new List<int>();
        this.parameterMinimums.Add(1);
        this.parameterMaximums = new List<int>();
        this.parameterMaximums.Add(300);
    }

    public void SetAsEmptyAction()
    {
        this.actionType = RecipeModule.Action.ActionType.Empty;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();

        this.parameterMinimums = new List<int>();
        this.parameterMaximums = new List<int>();
    }

    public RecipeModule.Action.ActionType GetActionType()
    {
        return actionType;
    }

    public List<int> GetParameterValues()
    {
        return parameterValues;
    }

    public List<string> GetParameterNames()
    {
        return parameterNames;
    }

    public void SetParameterNames(List<string> parameterNames)
    {
        this.parameterNames = parameterNames;
    }

    public void SetParameterValues(List<int> parameterValues)
    {
        this.parameterValues = parameterValues;
    }

    public List<int> GetParameterMins()
    {
        return parameterMinimums;
    }

    public List<int> GetParameterMaxs()
    {
        return parameterMaximums;
    }

    public void Clone(PseudoAction pa)
    {
        this.parameterValues = pa.GetParameterValues();
        this.parameterNames = pa.GetParameterNames();
        this.actionType = pa.actionType;
    }
}
