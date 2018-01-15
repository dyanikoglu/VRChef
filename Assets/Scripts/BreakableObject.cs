using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour {

    public GameObject brokenModelPrefab;
    public AudioClip breakSound;

    GameObject brokenPlate;
    bool isBroken = false;
    AudioSource source;

    // Use this for initialization
    void Start () {
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        source = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
        if (collision.relativeVelocity.magnitude >= 5 && !isBroken)
        {
            Broke();
        }
    }

    // Update is called once per frame
    void Update () {

    }

    void Broke()
    {
        brokenPlate = Instantiate(brokenModelPrefab, transform.position, brokenModelPrefab.transform.rotation);
        foreach (Transform child in brokenPlate.transform)
        {
            //Debug.Log(child.name);
            MeshCollider childCollider = child.gameObject.AddComponent<MeshCollider>();
            if (childCollider)
            {
                childCollider.convex = true;
            }
            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
            if (childRigidbody)
            {
                childRigidbody.useGravity = true;
                childRigidbody.isKinematic = false;
                childRigidbody.drag = 30.0f;
            }
        }

        Destroy(gameObject);
        
        // play breaking sound 
        AudioSource.PlayClipAtPoint(breakSound, transform.position);

        isBroken = true;


    }
}
