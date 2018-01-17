using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CanBeSqueezed : FoodCharacteristic
{

    private string myTag = "Squeezable";
    public Texture2D original;
    public Texture2D squeezedTexture;
    private bool canSpin;
    private GameObject currentSqueezer;

    private void Awake()
    {
        base.createTag(myTag);
    }

    // Use this for initialization
    void Start()
    {
        gameObject.tag = myTag;
        canSpin = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide");
        if (collision.gameObject.CompareTag("Squeezer"))
        {
            currentSqueezer = collision.gameObject;
            Debug.Log("with squeezer");
            GetComponent<Renderer>().materials[1].mainTexture = squeezedTexture;
           // transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
            canSpin = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Squeezer"))
        {
            Debug.Log("collision end with squeezer");
            canSpin = false;
           // currentSqueezer.GetComponent<CapsuleCollider>().isTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if ((OVRInput.Get(OVRInput.Button.Four) || OVRInput.Get(OVRInput.Button.Two)))
        {
            if (canSpin) //&& GetIsGrabbed())
            {
                //currentSqueezer.GetComponent<CapsuleCollider>().isTrigger = false;
                //transform.eulerAngles = new Vector3(180, transform.eulerAngles.y, 0);
                transform.Rotate(Vector3.up * Time.deltaTime, 5);
            }
        }

       
    }
}
