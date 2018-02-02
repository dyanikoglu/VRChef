using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!this.GetComponent<Joint>())
        {
            if (other.GetComponent<ToolCharacteristic>() && other.GetComponent<ToolCharacteristic>().canHangOnRack)
            {
                SpringJoint joint = gameObject.AddComponent<SpringJoint>();
                joint.connectedBody = other.GetComponent<Rigidbody>();
                joint.spring = 100;
                joint.damper = 0;
                joint.tolerance = 0.001f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.GetComponent<Joint>())
        {
            if (other.GetComponent<ToolCharacteristic>() && other.GetComponent<ToolCharacteristic>().canHangOnRack)
            {
                Destroy(this.GetComponent<Joint>());
            }
        }
    }
}
