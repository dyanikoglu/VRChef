using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPlateOven : MonoBehaviour {

    public GameObject[] plates;         // this oven has 4 hot plates
    public GameObject[] buttons;        // one button controlling each plate's heat
    public bool[] activePlateIndices;
    
    GameObject[] canFryObjectsOnMe;

    private void Start()
    {
        canFryObjectsOnMe = new GameObject[4];
    }

    // Update is called once per frame
    void Update ()
    {
        // check if user switchs on or off one of plates
		for(int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.localRotation.eulerAngles.z <= 95 && buttons[i].transform.localRotation.eulerAngles.z > 65)
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(true);
                plates[i].GetComponent<Renderer>().material.color = Color.red;
                activePlateIndices[i] = true;

                if (canFryObjectsOnMe[i])
                {
                    canFryObjectsOnMe[i].GetComponent<CanFry>().canFry = true;
                }
            }
            else
            {
                plates[i].transform.GetChild(0).gameObject.SetActive(false);
                plates[i].GetComponent<Renderer>().material.color = Color.white;
                activePlateIndices[i] = false;

                if (canFryObjectsOnMe[i])
                {
                    canFryObjectsOnMe[i].GetComponent<CanFry>().canFry = false;
                }
            }
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            //TODO: is there a contact 0?
            string name = collision.contacts[0].thisCollider.name;
            int index = Int32.Parse(name[name.Length - 1] + "");
            canFryObjectsOnMe[index] = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            //TODO: is there a contact 0?
            string name = collision.contacts[0].thisCollider.name;
            int index = Int32.Parse(name[name.Length - 1]+"");
            canFryObjectsOnMe[index] = null;
        }
    }
}
