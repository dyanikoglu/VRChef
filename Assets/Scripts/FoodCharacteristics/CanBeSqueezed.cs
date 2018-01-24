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
    private bool canSpin;
    private GameObject currentSqueezer;
    private float rotationAngle;
    private bool finished;

    // Use this for initialization
    void Start()
    {
        //Debug.Log(GetComponent<MeshFilter>().mesh.bounds.extents.y);
        canSpin = false;
        finished = false;
        rotationAngle = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>() != null)
        {
            currentSqueezer = collision.gameObject;            
           // transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
            canSpin = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanSqueeze>() != null)
        {
            canSpin = false;
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
                currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = false;
                //transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
                transform.Rotate(Vector3.up * 200 * Time.deltaTime, Space.Self);
                rotationAngle += 200 * Time.deltaTime;
                particleLauncher.Play();
                Debug.Log(rotationAngle);
                if(rotationAngle >= 360 * 3)
                {
                    GetComponent<Renderer>().materials[1].mainTexture = squeezedTexture;
                    rotationAngle = 0;
                    finished = true;
                    particleLauncher.Stop();
                }
            }
        }
        else
        {
            GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
            if(currentSqueezer != null)
            {
                currentSqueezer.GetComponent<VRTK.VRTK_InteractableObject>().isGrabbable = true;
            }
        }

    }
}
