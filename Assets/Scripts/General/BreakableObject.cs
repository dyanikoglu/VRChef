using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

public class BreakableObject : MonoBehaviour {

    public GameObject[] brokenModelPrefabs;
    public AudioClip[] breakSounds;
    public float[] breakingThresholds;


    GameObject brokenObject;
    bool isBroken = false;
    AudioSource source;

    float lastVelocityValue = 0f;

    // Use this for initialization
    void Start () {

        if (GetComponent<MeshFilter>())
        {
            if (GetComponent<MeshFilter>().sharedMesh.vertexCount > 255)
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }

        else if (GetComponent<Egg>() && GetComponent<Egg>().isCracked)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        }
        
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 0.3f;
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        source = gameObject.AddComponent<AudioSource>();

        gameObject.AddComponent<VRTK_InteractableObject>().isGrabbable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = breakingThresholds.Length - 1; i >= 0; i--)
        {
            if(GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                if(lastVelocityValue >= breakingThresholds[i] / 2)
                {
                    Break(i);
                    break;
                }
            }
            else
            { 
                if (collision.relativeVelocity.magnitude >= breakingThresholds[i] && !isBroken)
                {
                    Break(i);
                    break;
                }
            }
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

    void Break(int index)
    {
        // to create breaking effect, instantiate a broken version of model at the same place and with same rotation
        if (GetComponent<Egg>())
        {

            // half cracked egg -> full cracked egg
            if (GetComponent<BreakableObject>().breakingThresholds.Length == 1)
            {
                Instantiate(GetComponent<Egg>().insideEggPrefab, transform.position, GetComponent<Egg>().insideEggPrefab.transform.rotation);
                brokenObject = Instantiate(brokenModelPrefabs[index], transform.position + new Vector3(0, 0.1f, 0), brokenModelPrefabs[index].transform.rotation);
            }
            else
            {
                // egg -> full cracked egg
                if (index == 1)
                {
                    Instantiate(GetComponent<Egg>().insideEggPrefab, transform.position, GetComponent<Egg>().insideEggPrefab.transform.rotation);
                    brokenObject = Instantiate(brokenModelPrefabs[index], transform.position + new Vector3(0, 0.1f, 0), brokenModelPrefabs[index].transform.rotation);
                }
                // egg -> half cracked egg
                else
                {
                    brokenObject = Instantiate(brokenModelPrefabs[index], transform.position, brokenModelPrefabs[index].transform.rotation);

                    brokenObject.AddComponent<PullFromBothSides_GrabAction>().ungrabDistance = 0.2f;

                    BreakableObject breakableObject = brokenObject.AddComponent<BreakableObject>();

                    breakableObject.brokenModelPrefabs = new GameObject[1];
                    breakableObject.brokenModelPrefabs[0] = brokenModelPrefabs[1];

                    breakableObject.breakingThresholds = new float[1];
                    breakableObject.breakingThresholds[0] = (breakingThresholds[1] + breakingThresholds[0]) / 2;

                    breakableObject.breakSounds = new AudioClip[1];
                    breakableObject.breakSounds[0] = breakSounds[1];

                    Egg crackedEgg = brokenObject.AddComponent<Egg>();
                    crackedEgg.insideEggPrefab = GetComponent<Egg>().insideEggPrefab;
                    crackedEgg.isCracked = true;

                }
            }
        }
        else
        {
            brokenObject = Instantiate(brokenModelPrefabs[index], transform.position, brokenModelPrefabs[index].transform.rotation);
        }

        int fragmentCount = brokenObject.transform.childCount;


        foreach (Transform child in brokenObject.transform)
        {
            if(brokenObject.GetComponent<Egg>() && brokenObject.GetComponent<Egg>().isCracked)
            {
                break;
            }

            if (child.gameObject.GetComponent<MeshFilter>())
            {
                if (child.gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount > 255)
                {
                    BoxCollider collider = child.gameObject.AddComponent<BoxCollider>();
                }
                else
                {
                    MeshCollider collider = child.gameObject.AddComponent<MeshCollider>();
                    collider.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh | MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.EnableMeshCleaning;
                    collider.skinWidth = 0.001f;
                    collider.convex = true;
                }
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
        AudioSource.PlayClipAtPoint(breakSounds[index], transform.position);

        isBroken = true;
    }
}
