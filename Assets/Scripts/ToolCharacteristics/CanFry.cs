using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFry : MonoBehaviour {

    public bool canFry = false;
    GameObject firstCollidedObject;
    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (!canFry && collision.transform.parent)
        {
            Debug.Log("stay "+ collision.collider.name);
            firstCollidedObject = collision.collider.transform.parent.gameObject;
            canFry = OnOven(collision.collider.transform.parent);
        }

        //Debug.Log("stay "+ canFry);
    }
    /*
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.parent)
        {
            Debug.Log("exit " + collision.collider.name);
            bool dummy = OnOven(collision.transform.parent);
            canFry = false;
        }

        //Debug.Log(dummy);

        //Debug.Log("exit " + canFry);

    }
    */
    bool OnOven(Transform parent)
    {
        bool isPlate = false;
        while (parent != null)
        {
            //Debug.Log(parent.name);
            if (parent.GetComponent<HotPlateOven>())
            {
                HotPlateOven hotPlateOven = parent.GetComponent<HotPlateOven>();
                for (int i = 0; i < hotPlateOven.plates.Length; i++)
                {
                    Debug.Log(hotPlateOven.plates[i].name + " - " + firstCollidedObject.name + " - " + hotPlateOven.activePlateIndices[i]);
                    if (hotPlateOven.plates[i].name == firstCollidedObject.name && hotPlateOven.activePlateIndices[i])
                    {
                        isPlate = true;
                        return isPlate;
                    }
                    Debug.Log(isPlate + "*");
                }
                
            }
            parent = parent.transform.parent;
        }
        Debug.Log("i am gonna return: " + isPlate);
        return isPlate;
    }

    public bool GetCanFry()
    {
        return canFry;
    }
}
