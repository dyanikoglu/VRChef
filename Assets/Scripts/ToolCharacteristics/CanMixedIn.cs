using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanMixedIn : ToolCharacteristic {
    private bool collided;
	// Use this for initialization
	void Start () {
        //collided=false;
    }
	
	// Update is called once per frame
	void Update () {
		/*if(!collided && GetIsGrabbed())
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }*/
	}

    private void OnCollisionEnter(Collision collision)
    {
        /*Debug.Log("collison");
        if(collision.gameObject.GetComponent<CanBeMixed>()!= null && !collided)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collided = true;
        }*/
    }
}
