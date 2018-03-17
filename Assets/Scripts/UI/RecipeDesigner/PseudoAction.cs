using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoAction : MonoBehaviour {
    private RecipeModule.Action.ActionType actionType;

    /* Cook: [0] -> 0: Overcooked, 1: Cooked, 2: Underdone | [1] -> requiredHeat | [2] -> requiredTime
     * Fry: [0] -> 0: Overfried, 1: Fried, 2: Underdone | [1] -> requiredHeat | [2] -> requiredTime
     * Chop: [0] -> 0: Piece Count
     * ...
     * ..
     * .
     * 
     */

    private List<int> parameterValues;
    
    private List<string> parameterNames;

    public void SetAsChop(int pieceCount = 4)
    {
        this.actionType = RecipeModule.Action.ActionType.Chop;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Piece Size");
        this.parameterValues = new List<int>();
        this.parameterValues.Add(pieceCount);
    }

    public void SetAsFry(RecipeModule.Fry.FryType fryType = RecipeModule.Fry.FryType.Fried, int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Fry;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Frying Type");
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)fryType);
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    }

    public void SetAsCook(RecipeModule.Cook.CookType cookType = RecipeModule.Cook.CookType.Cooked, int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Cook;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Cook Type");
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)cookType);
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

    public void SetAsBoil(RecipeModule.Boil.BoilType boilType = RecipeModule.Boil.BoilType.Boiled, int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Boil;
        this.parameterNames = new List<string>();
        this.parameterNames.Add("Boiling Type");
        this.parameterNames.Add("Required Heat");
        this.parameterNames.Add("Required Time");
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)boilType);
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
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
