using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodPicker : MonoBehaviour {
    public GameObject viewportContentRef;
    public GameObject dummyPickupZoneRef;

    public GameObject[] foodList;

    public int itemCountPerRow;
    public int spacingX;
    public int spacingY;
    public int startX;
    public int startY;

	void Start () {
        int currentX = startX;
        int currentY = startY;
        int loopCount = 0;
		foreach(GameObject o in foodList)
        {
            GameObject newPickupZone = GameObject.Instantiate(dummyPickupZoneRef);
            newPickupZone.transform.SetParent(viewportContentRef.transform, false);

            string foodName = o.GetComponent<FoodStatus>().foodIdentifier;

            newPickupZone.GetComponentInChildren<Text>().text = foodName;
            RectTransform rt = newPickupZone.GetComponent<RectTransform>();

            FoodState newFoodState = newPickupZone.GetComponentInChildren<FoodState>();

            Vector3 newPos = new Vector3(startX + spacingX * (loopCount % itemCountPerRow), startY + (((loopCount) / itemCountPerRow) * spacingY), 0);
            rt.anchoredPosition3D = newPos;

            newPickupZone.gameObject.name = foodName + "PickupZone";
            newFoodState.gameObject.name = foodName;

            loopCount++;
        }
	}
}
