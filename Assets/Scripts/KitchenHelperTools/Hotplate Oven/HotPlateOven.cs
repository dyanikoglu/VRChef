using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPlateOven : MonoBehaviour {

    public GameObject[] plates;         // this oven has 4 hot plates
    public GameObject[] buttons;        // one button controlling each plate's heat
    public bool[] activePlateIndices;
    
    public GameObject[] canHeatObjectsOnMe;

    private void Start()
    {
        canHeatObjectsOnMe = new GameObject[4];
    }

    // Update is called once per frame
    void Update ()
    {
        // check if user switchs on or off one of plates
		for(int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.localRotation.eulerAngles.z <= 120 && buttons[i].transform.localRotation.eulerAngles.z > 65)
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(true);
                plates[i].GetComponent<Renderer>().material.color = Color.red;
                activePlateIndices[i] = true;

                if (canHeatObjectsOnMe[i])
                {
                    if (canHeatObjectsOnMe[i].GetComponent<CanFry>())
                    {
                        canHeatObjectsOnMe[i].GetComponent<CanFry>().canFry = true;
                    }
                    if (canHeatObjectsOnMe[i].GetComponent<CanBoil>())
                    {
                        canHeatObjectsOnMe[i].GetComponent<CanBoil>().canBoil = true;
                    }
                }
            }
            else
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(false);
                plates[i].GetComponent<Renderer>().material.color = Color.white;
                activePlateIndices[i] = false;

                if (canHeatObjectsOnMe[i])
                {
                    if (canHeatObjectsOnMe[i].GetComponent<CanFry>())
                    {
                        canHeatObjectsOnMe[i].GetComponent<CanFry>().canFry = false;
                    }
                    if (canHeatObjectsOnMe[i].GetComponent<CanBoil>())
                    {
                        canHeatObjectsOnMe[i].GetComponent<CanBoil>().canBoil = false;
                    }
                }
            }
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>() || collision.gameObject.GetComponent<CanBoil>())
        {
            string name = collision.contacts[0].thisCollider.name;
            int index = Int32.Parse(name[name.Length - 1] + "");
            canHeatObjectsOnMe[index] = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        for(int i = 0; i < canHeatObjectsOnMe.Length; i++)
        {
            if(canHeatObjectsOnMe[i] == collision.gameObject)
            {
                canHeatObjectsOnMe[i] = null;
                if (collision.gameObject.GetComponent<CanFry>())
                {
                    collision.gameObject.GetComponent<CanFry>().canFry = false;
                }
                else if (collision.gameObject.GetComponent<CanBoil>())
                {
                    collision.gameObject.GetComponent<CanBoil>().canBoil = false;
                }
            }
        }
    }
}
