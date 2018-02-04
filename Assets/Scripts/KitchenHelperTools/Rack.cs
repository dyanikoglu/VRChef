using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Rack : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if (!this.GetComponent<Joint>() && other.transform.parent)
        {
            VRTK_InteractableObject vrtk_io = other.transform.parent.GetComponent<VRTK_InteractableObject>();
            if (vrtk_io && !vrtk_io.IsGrabbed())
            {
                if (other.GetComponent<RackHang>() && other.GetComponent<RackHang>().canHangOnRack)
                {
                    HingeJoint joint = gameObject.AddComponent<HingeJoint>();
                    other.transform.parent.transform.position = this.transform.position;
                    joint.connectedBody = other.transform.parent.GetComponent<Rigidbody>();
                    joint.axis = new Vector3(0, 0, 0);
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (this.GetComponent<Joint>())
        {
            if (other.GetComponent<RackHang>() && other.GetComponent<RackHang>().canHangOnRack)
            {
                Destroy(this.GetComponent<Joint>());
            }
        }
    }
}
