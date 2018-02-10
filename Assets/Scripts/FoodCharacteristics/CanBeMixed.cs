using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeMixed : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanMixedIn>() != null)
        {
           gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanMixedIn>() != null)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
