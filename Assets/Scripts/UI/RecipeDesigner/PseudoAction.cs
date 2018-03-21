using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoAction : MonoBehaviour {
    private RecipeModule.Action.ActionType actionType;

    /* Cook: [0] -> requiredHeat | [1] -> requiredTime
     * Fry: [0] -> requiredHeat | [1] -> requiredTime
     * Chop: [0] -> 0: Piece Count
     * ...
     * ..
     * .
     * 
     */

    public List<int> parameterValues;
    
    private List<string> parameterNames;

    // Indicates that that action won't impact the input, output will be the same with input.
    private bool emptyAction = false;

    public void SetAsChop(int pieceCount = 4)
    {
        this.actionType = RecipeModule.Action.ActionType.Chop;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Piece Size");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(pieceCount);
    }

    public void SetAsFry(int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Fry;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    }

    public void SetAsCook(int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Cook;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    } 

    public void SetAsPeel()
    {
        this.actionType = RecipeModule.Action.ActionType.Peel;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
    }

    public void SetAsSmash()
    {
        this.actionType = RecipeModule.Action.ActionType.Smash;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
    }

    public void SetAsSqueeze()
    {
        this.actionType = RecipeModule.Action.ActionType.Squeeze;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
    }

    public void SetAsBreak()
    {
        this.actionType = RecipeModule.Action.ActionType.Break;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
    }

    public void SetAsBoil(int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Boil;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    }

    public void SetAsEmptyAction()
    {
        this.actionType = RecipeModule.Action.ActionType.Empty;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
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

    public void Clone(PseudoAction pa)
    {
        this.parameterValues = pa.GetParameterValues();
        this.parameterNames = pa.GetParameterNames();
        this.actionType = pa.actionType;
    }
}
