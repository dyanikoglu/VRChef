using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRTK;

public class CanBeSqueezed : FoodCharacteristic
{

    public Texture2D original;
    public Texture2D squeezedTexture;
    public ParticleSystem particleLauncher;
    public AudioClip juiceSound;

    AudioSource source;
    bool onlyOnce = true;

    private bool canSpin;
    private GameObject currentSqueezer;
    private float rotationAngle;
    private bool finished;
    private MeshCollider meshCollider;

    // Use this for initialization
    void Start()
    {
        canSpin = false;
        finished = false;
        rotationAngle = 0;

        meshCollider = gameObject.AddComponent<MeshCollider>() as MeshCollider;
        meshCollider.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
        meshCollider.skinWidth = 0.01f;
        meshCollider.convex = true;

        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = juiceSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is CapsuleCollider)
        {
            transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
            transform.position = other.transform.position;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>() != null)
        {
            currentSqueezer = collision.gameObject;

            canSpin = true;
            currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;

            currentSqueezer.GetComponent<Rigidbody>().isKinematic = true;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>() != null)
        {
            canSpin = false;
            currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;

            currentSqueezer.GetComponent<Rigidbody>().isKinematic = false;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.Get(OVRInput.Button.Four) || OVRInput.Get(OVRInput.Button.Two)))
        {
            if (canSpin && !finished)
            {
                GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;
                transform.Rotate(Vector3.up * 200 * Time.deltaTime, Space.Self);
                rotationAngle += 200 * Time.deltaTime;
                particleLauncher.Emit(40);

                if (onlyOnce)
                {
                    source.Play();
                    onlyOnce = false;
                }                

                if (rotationAngle >= 360 * 3)
                {
                    GetComponent<Renderer>().materials[1].mainTexture = squeezedTexture;
                    rotationAngle = 0;
                    finished = true;
                    currentSqueezer.GetComponent<CanSqueeze>().AddWater();
                }
            }
            else
            {
                source.loop = false;
                source.Stop();
                onlyOnce = true;
            }
        }
        else
        {
            source.Stop();
            onlyOnce = true;
            source.loop = true;
            GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
        }

    }
}
