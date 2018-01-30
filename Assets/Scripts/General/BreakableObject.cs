using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BreakableObject : MonoBehaviour {

    public GameObject brokenModelPrefab;
    public AudioClip breakSound;

    GameObject brokenObject;
    bool isBroken = false;
    AudioSource source;

    public float breakingThreshold;
    float lastVelocityValue = 0f;

    // Use this for initialization
    void Start () {
        // add a collider and physics components to object
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();

        collider.convex = true;
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 0.3f;
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        source = gameObject.AddComponent<AudioSource>();


    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.relativeVelocity.magnitude >= breakingThreshold && !isBroken)
        {
            Break();
        }
        else if (collision.gameObject.tag == "Breakable" && lastVelocityValue >= breakingThreshold/2)
        {
            Break();
        }
    }

    void Update () {

        /* when an object is grabbed, it is moved by with hand only instead of moved by physics rules.
         * Therefore we cannot reach velocity of object. To handle this problem we get controllers' velocity.
         * Once user holds 2 objects with 2 hand and collide them, we will use this velocity to decide
         * whether objects will break or not.
         */

        if (GetComponent<VRTK_InteractableObject>().GetGrabbingObject())
        {
            if (GetComponent<VRTK_InteractableObject>().GetGrabbingObject().name == "RightController")
            {
                lastVelocityValue = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude;
            }
            else if (GetComponent<VRTK_InteractableObject>().GetGrabbingObject().name == "LeftController")
            {
                lastVelocityValue = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).magnitude;
            }
        }
    }

    void Break()
    {
        // to create breaking effect, instantiate a broken version of model at the same place and with same rotation
        brokenObject = Instantiate(brokenModelPrefab, transform.position, brokenModelPrefab.transform.rotation);
        int fragmentCount = brokenObject.transform.childCount;

        foreach (Transform child in brokenObject.transform)
        {
            MeshCollider childCollider = child.gameObject.AddComponent<MeshCollider>();
            if (childCollider)
            {
                childCollider.convex = true;
                childCollider.inflateMesh = true;
                childCollider.skinWidth = 0.01f;
            }
            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
            if (childRigidbody)
            {
                // give a realistic weight to fragments equally.
                childRigidbody.mass = GetComponent<Rigidbody>().mass / fragmentCount * 6;
                // apply physics
                childRigidbody.useGravity = true;
                childRigidbody.isKinematic = false;
                // after breaking, fragments are exposed forces and therefore they are kind of slipping
                // to get rid of this effect, set velocity of fragment 0.
                childRigidbody.velocity = Vector3.zero;
                childRigidbody.angularVelocity = Vector3.zero;
            }
        }
        // remove the original object
        Destroy(gameObject);
        
        // play breaking sound 
        AudioSource.PlayClipAtPoint(breakSound, transform.position);

        isBroken = true;
    }
}
