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

        // Create an action item for each action type in ActionType enum
        foreach (RecipeModule.Action.ActionType actionType in Enum.GetValues(typeof(RecipeModule.Action.ActionType)))
        {
            if(actionType == RecipeModule.Action.ActionType.PutTogether)
            {
                continue;
            }

            GameObject newPickupZone = GameObject.Instantiate(dummyPickupZoneRef);
            newPickupZone.transform.SetParent(viewportContentRef.transform, false);

            string actionName = actionType.ToString();

            newPickupZone.GetComponentInChildren<Text>().text = actionName;
            RectTransform rt = newPickupZone.GetComponent<RectTransform>();

            PseudoAction newPseudoAction = newPickupZone.transform.GetChild(0).gameObject.AddComponent<PseudoAction>();

            switch (actionType)
            {
                case RecipeModule.Action.ActionType.Boil:
                    newPseudoAction.SetAsBoil();
                    break;
                case RecipeModule.Action.ActionType.Break:
                    newPseudoAction.SetAsBreak();
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
                    // Waiting for implementation of Mixing Action
                    break;
                case RecipeModule.Action.ActionType.Peel:
                    newPseudoAction.SetAsPeel();
                    break;
                case RecipeModule.Action.ActionType.Smash:
                    newPseudoAction.SetAsSmash();
                    break;
                case RecipeModule.Action.ActionType.Squeeze:
                    newPseudoAction.SetAsSqueeze();
                    break;
                case RecipeModule.Action.ActionType.Empty:
                    newPseudoAction.SetAsEmptyAction();
                    break;
                default:
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
