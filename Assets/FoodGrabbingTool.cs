using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGrabbingTool : MonoBehaviour {
    
    public GameObject grabbingArea;
    public GameObject grabbedFood;

	void Update ()
    {
        //Debug.Log("local rotation" + grabbingArea.transform.localRotation.eulerAngles);
        //Debug.Log("rotation" + grabbingArea.transform.rotation.eulerAngles);
        if (grabbedFood)
        {
            //grabbedFood.GetComponent<Rigidbody>().isKinematic = true;
            //grabbedFood.GetComponent<Rigidbody>().useGravity = false;

            grabbedFood.transform.position = grabbingArea.transform.position + new Vector3(0, 0.01f, 0);
            grabbedFood.transform.eulerAngles = new Vector3(15f, grabbedFood.transform.eulerAngles.y, grabbedFood.transform.eulerAngles.z);

            bool canHold = (grabbingArea.transform.eulerAngles.x <= 25 && grabbingArea.transform.eulerAngles.x > 0) || 
                           (grabbingArea.transform.eulerAngles.x <= 360 && grabbingArea.transform.eulerAngles.x >= 320) ||
                           (grabbingArea.transform.eulerAngles.z <= 25 && grabbingArea.transform.eulerAngles.z > 0) || 
                           (grabbingArea.transform.eulerAngles.z <= 360 && grabbingArea.transform.eulerAngles.z >= 320);

            if (!canHold)
            {
                StartCoroutine("Release");
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            if (grabbedFood)
            {
                StartCoroutine("Release");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<FoodCharacteristic>())
        {
            if (!grabbedFood)
            {
                grabbedFood = collision.gameObject;
                //Debug.Log(collision.gameObject.name);
            }
        }
    }

    IEnumerator Release()
    {
        grabbedFood = null;
        Collider[] myColliders = GetComponents<Collider>();
        foreach (Collider c in myColliders)
        {
            c.enabled = false;
        }
        yield return new WaitForSeconds(5f);

        foreach (Collider c in myColliders)
        {
            c.enabled = true;
        }

    }
}
