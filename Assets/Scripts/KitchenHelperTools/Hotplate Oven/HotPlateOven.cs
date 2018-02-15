using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPlateOven : MonoBehaviour {

    public GameObject[] plates;         // this oven has 4 hot plates
    public GameObject[] buttons;        // one button controlling each plate's heat
    public bool[] activePlateIndices;

    // Update is called once per frame
    void Update ()
    {
        // check if user switchs on or off one of plates
		for(int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.localRotation.eulerAngles.z <= 95 && buttons[i].transform.localRotation.eulerAngles.z > 85)
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(true);
                plates[i].GetComponent<Renderer>().material.color = Color.red;
                activePlateIndices[i] = true;
            }
            else
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(false);
                plates[i].GetComponent<Renderer>().material.color = Color.white;
                activePlateIndices[i] = false;
            }
        }
	}
}
