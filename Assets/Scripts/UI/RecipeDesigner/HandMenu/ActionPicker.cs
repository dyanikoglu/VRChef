using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPicker : MonoBehaviour {
    public GameObject viewportContentRef;
    public GameObject dummyPickupZoneRef;

    public int itemCountPerRow;
    public int spacingX;
    public int spacingY;
    public int startX;
    public int startY;

    void Start()
    {
        int currentX = startX;
        int currentY = startY;
        int loopCount = 0;

        
        foreach (RecipeModule.Action.ActionType actionType in Enum.GetValues(typeof(RecipeModule.Action.ActionType)))
        {
            GameObject newPickupZone = GameObject.Instantiate(dummyPickupZoneRef);
            newPickupZone.transform.SetParent(viewportContentRef.transform, false);

            string actionName = actionType.ToString();

            newPickupZone.GetComponentInChildren<Text>().text = actionName;
            RectTransform rt = newPickupZone.GetComponent<RectTransform>();

            PseudoAction newPseudoAction = newPickupZone.transform.GetChild(0).gameObject.AddComponent<PseudoAction>();

            switch (actionType)
            {
                case RecipeModule.Action.ActionType.Boil:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.Break:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.Chop:
                    newPseudoAction.SetAsChop();
                    break;
                case RecipeModule.Action.ActionType.Cook:
                    newPseudoAction.SetAsCook();
                    break;
                case RecipeModule.Action.ActionType.Fry:
                    newPseudoAction.SetAsFry();
                    break;
                case RecipeModule.Action.ActionType.Mix:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.Peel:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.PutTogether:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.Smash:
                    // TODO
                    break;
                case RecipeModule.Action.ActionType.Squeeze:
                    // TODO
                    break;
                default:
                    // TODO
                    break;
            }


            Vector3 newPos = new Vector3(startX + spacingX * (loopCount % itemCountPerRow), startY + (((loopCount) / itemCountPerRow) * spacingY), 0);
            rt.anchoredPosition3D = newPos;

            newPickupZone.gameObject.name = actionName + "PickupZone";
            newPseudoAction.gameObject.name = actionName;

            loopCount++;
        }
    }
}
