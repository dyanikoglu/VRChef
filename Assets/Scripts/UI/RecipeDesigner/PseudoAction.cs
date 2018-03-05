using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoAction : MonoBehaviour {
    private RecipeModule.Action.ActionType actionType;

    /* Cook: [0] -> 0: Overcooked, 1: Cooked, 2: Underdone | [1] -> requiredHeat | [2] -> requiredTime
     * Fry: [0] -> 0: Overfried, 1: Fried, 2: Underdone | [1] -> requiredHeat | [2] -> requiredTime
     * Chop: [0] -> 0: Small Piece, 1: Middle Piece, 2: Big Piece
     * ...
     * ..
     * .
     * 
     */
    private List<int> parameterValues;
    
    private List<string> parameterNames;

    public void SetAsChop(RecipeModule.Chop.PieceVolumeSize pieceVolumeSize = RecipeModule.Chop.PieceVolumeSize.Middle)
    {
        this.actionType = RecipeModule.Action.ActionType.Chop;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)pieceVolumeSize);
    }

    public void SetAsFry(RecipeModule.Fry.FryType fryType = RecipeModule.Fry.FryType.Fried, int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Fry;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)fryType);
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    }

    public void SetAsCook(RecipeModule.Cook.CookType cookType = RecipeModule.Cook.CookType.Cooked, int requiredHeat = 150, int requiredTime = 60)
    {
        this.actionType = RecipeModule.Action.ActionType.Cook;
        this.parameterNames = new List<string>();
        this.parameterValues = new List<int>();
        this.parameterValues.Add((int)cookType);
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
}
