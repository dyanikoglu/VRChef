using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWithParent : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.IgnoreCollision(GetComponent<Collider>(), transform.parent.GetComponent<Collider>(), true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
