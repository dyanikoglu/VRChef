using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour {

    public GameObject brokenModelPrefab;
    public AudioClip breakSound;

    GameObject brokenObject;
    bool isBroken = false;
    AudioSource source;

    public float breakingThreshold = 5.0f;

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
    }

    // Update is called once per frame
    void Update () {

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
                childCollider.skinWidth = 0.02f;
            }
            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
            if (childRigidbody)
            {
                // give a realistic weight to fragments equally.
                childRigidbody.mass = GetComponent<Rigidbody>().mass / fragmentCount * 2;
                // apply physics
                childRigidbody.useGravity = true;
                childRigidbody.isKinematic = false;
                // after breaking, fragments are exposed forces and therefore they are kind of slipping
                // to get rid of this effect, set velocity of fragment 0.
                childRigidbody.velocity = Vector3.zero;
                childRigidbody.angularVelocity = Vector3.zero;
                // Sometimes although their velocity is zero they are turning around. To prevent 
                // this freeze their rotation. 
                childRigidbody.freezeRotation = true;
            }
            Debug.Log(childRigidbody.angularVelocity);
        }
        // remove the original object
        Destroy(gameObject);
        
        // play breaking sound 
        AudioSource.PlayClipAtPoint(breakSound, transform.position);

        isBroken = true;
    }
}
