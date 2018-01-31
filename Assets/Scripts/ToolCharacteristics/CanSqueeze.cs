using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSqueeze : ToolCharacteristic
{

    public GameObject bowl;
    BoxCollider[] myColliders;
    //1. on bowl, 2.on others
    CapsuleCollider capsuleCollider;
    private int childCountForWater;

    // Use this for initialization
    void Start()
    {
        myColliders = GetComponents<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        childCountForWater = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("enter");
        Debug.Log(collision.gameObject);
        if (collision.gameObject == bowl || collision.gameObject.GetComponent<CanBeSqueezed>() != null)
        {
            myColliders[0].enabled = true;
            myColliders[1].enabled = false;
            capsuleCollider.isTrigger = true;
        }
        /*else if (collision.gameObject.GetComponent<CanBeSqueezed>() != null)
        {
            myColliders[0].enabled = true;
            myColliders[1].enabled = false;
            capsuleCollider.enabled = true;
        }*/
        else if(collision.gameObject.GetComponent<CanBeSqueezed>() == null)
        {
            myColliders[0].enabled = false;
            myColliders[1].enabled = true;
            capsuleCollider.isTrigger = false;
        }
    }

    public void AddWater()
    {
        if(childCountForWater < 3)
        {
            bowl.transform.GetChild(childCountForWater++).gameObject.SetActive(true);
        }

    }
}
