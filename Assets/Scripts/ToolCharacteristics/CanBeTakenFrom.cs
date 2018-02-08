using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CanBeTakenFrom : ToolCharacteristic {


	void Start () {
        CanTake [] spoons = FindObjectsOfType<CanTake>();
        foreach(CanTake spoon in spoons)
        {
            Physics.IgnoreCollision(gameObject.transform.parent.gameObject.GetComponent<Collider>(), spoon.GetComponent<Collider>());
            Physics.IgnoreCollision(gameObject.transform.parent.gameObject.GetComponent<Collider>(), spoon.transform.GetChild(0).gameObject.GetComponent<Collider>());
        }
    }
	
	// Update is called once per frame
	void Update () {


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanTake>() != null && !CanTake.full)
        {
            Vector3 pos = gameObject.transform.position;
            pos.y = pos.y - (float)0.015;
            if (pos.y <= gameObject.transform.parent.transform.position.y - (gameObject.transform.parent.gameObject.GetComponent<Renderer>().bounds.size.y)/2)
            {
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                CanTake.full = true;
                Destroy(gameObject);
            }
            else
            {
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                CanTake.full = true;
                gameObject.transform.position = pos;
            }
            
        }
    }
}
