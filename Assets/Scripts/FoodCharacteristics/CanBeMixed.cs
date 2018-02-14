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
        if(collision.gameObject.GetComponent<CanMixedIn>()!=null && gameObject.transform.parent != collision.gameObject.transform)
        {
            gameObject.transform.parent = collision.gameObject.transform;
        }
    }
}
