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

        gameObject.AddComponent<AudioSource>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is CapsuleCollider)
        {
            Debug.Log("capsule collider");
            meshCollider.skinWidth = 0.01f;
            //transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.EulerAngles(180, transform.rotation.y, 0);
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

            // transform.position = new Vector3(currentSqueezer.transform.position.x, currentSqueezer.transform.position.y, currentSqueezer.transform.position.z);
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
            meshCollider.skinWidth = 0.005f;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.Get(OVRInput.Button.Four) || OVRInput.Get(OVRInput.Button.Two)))
        {
            if (canSpin && !finished) //&& GetIsGrabbed())
            {
                GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;
                transform.Rotate(Vector3.up * 200 * Time.deltaTime, Space.Self);
                rotationAngle += 200 * Time.deltaTime;
                particleLauncher.Emit(40);
                AudioSource.PlayClipAtPoint(juiceSound, transform.position);

                if (rotationAngle >= 360 * 3)
                {
                    AudioSource.PlayClipAtPoint(juiceSound, transform.position);
                    GetComponent<Renderer>().materials[1].mainTexture = squeezedTexture;
                    rotationAngle = 0;
                    finished = true;
                    currentSqueezer.GetComponent<CanSqueeze>().AddWater();
                }
            }
        }
        else
        {
            GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
        }

    }
}
