using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanShuffleFood : ToolCharacteristic {

    public GameObject shuffleArea;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("girdi "+collision.collider.name);        
        //collision.gameObject.transform.parent = gameObject.transform;
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("çıktı");        
    }
}
