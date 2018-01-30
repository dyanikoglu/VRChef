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
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject == bowl)
        {
            myColliders[0].enabled = true;
            myColliders[1].enabled = false;
            capsuleCollider.enabled = false;
        }
        else if(collision.gameObject.GetComponent<CanBeSqueezed>() == null)
        {
            myColliders[0].enabled = false;
            myColliders[1].enabled = true;
            capsuleCollider.enabled = true;
        }
    }

    public void addWater()
    {
        if(childCountForWater < 3)
        {
            bowl.transform.GetChild(childCountForWater++).gameObject.SetActive(true);
        }

    }
}
