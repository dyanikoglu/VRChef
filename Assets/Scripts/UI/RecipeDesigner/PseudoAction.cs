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

    private void Start()
    {
        parameterNames = new List<string>();
        parameterValues = new List<int>();
    }

    public void SetAsChop(RecipeModule.Chop.PieceVolumeSize pieceVolumeSize)
    {
        this.actionType = RecipeModule.Action.ActionType.Chop;
        this.parameterValues.Clear();
        this.parameterValues.Add((int)pieceVolumeSize);
    }

    public void SetAsFry(RecipeModule.Fry.FryType fryType, int requiredHeat, int requiredTime)
    {
        this.actionType = RecipeModule.Action.ActionType.Fry;
        this.parameterValues.Clear();
        this.parameterValues.Add((int)fryType);
        this.parameterValues.Add(requiredHeat);
        this.parameterValues.Add(requiredTime);
    }

    public void SetAsCook(RecipeModule.Cook.CookType cookType, int requiredHeat, int requiredTime)
    {
        this.actionType = RecipeModule.Action.ActionType.Cook;
        this.parameterValues.Clear();
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
