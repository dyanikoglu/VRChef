using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CanBeSqueezed : FoodCharacteristic
{
    public Material squeezedMaterial;
    public AudioClip juiceSound;

    AudioSource source;
    bool onlyOnce = true;

    private bool canSpin;
    private GameObject currentSqueezer;
    private float rotationAngle;
    private bool finished;
    private ParticleSystem particleLauncher;

    // Use this for initialization
    void Start()
    {
        base.Start();
        canSpin = false;
        finished = false;
        rotationAngle = 0;

        source = gameObject.GetComponent<AudioSource>();
        source.loop = true;
        source.clip = juiceSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanSqueeze>())
        {
            canSpin = true;
            //    transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
            //    transform.position = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other is CapsuleCollider)
        //{
        //    transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
        //    transform.position = other.transform.position;
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>() != null && GetComponent<FoodStatus>().GetIsChoppedPiece())
        {
            currentSqueezer = collision.gameObject;
            particleLauncher = currentSqueezer.transform.GetChild(0).GetComponent<ParticleSystem>();

            currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;

            currentSqueezer.GetComponent<Rigidbody>().isKinematic = true;
            currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>()) {

            canSpin = false;

            if (GetComponent<FoodStatus>().GetIsChoppedPiece())
            {
                currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
                currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;

                currentSqueezer.GetComponent<Rigidbody>().isKinematic = false;
                currentSqueezer.GetComponent<CanSqueeze>().bowl.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    void Update()
    {
        if (GetComponent<FoodCharacteristic>().GetIsGrabbed() && GetComponent<FoodStatus>().GetIsChoppedPiece())
        {
            if (canSpin && !finished )
            {
                rotationAngle += 80 * Time.deltaTime;
                particleLauncher.Emit(40);

                if (onlyOnce)
                {
                    source.Play();
                    onlyOnce = false;
                }                

                if (rotationAngle >= 360 )
                {
                    var my_materials = GetComponent<Renderer>().materials;
                    my_materials[1] = squeezedMaterial;
                    GetComponent<Renderer>().materials = my_materials;

                    GetComponent<FoodStatus>().SetIsSqueezed(true);
                    rotationAngle = 0;
                    finished = true;
                    currentSqueezer.GetComponent<CanSqueeze>().AddWater();
                    if (GetComponent<CanBeChopped>())
                    {
                        GetComponent<CanBeChopped>().capMaterial = squeezedMaterial;
                    }
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
        }

    }
}
