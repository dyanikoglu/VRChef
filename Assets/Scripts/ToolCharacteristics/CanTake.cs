using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CanTake: ToolCharacteristic {

    // Use this for initialization
    public ParticleSystem dust;
    public static bool full = false;
    Vector3 _rotation;
   

    void Start () {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        _rotation = gameObject.transform.rotation.eulerAngles;        
    }

    // Update is called once per frame
    void Update () {
        if(Math.Abs(_rotation.x- gameObject.transform.rotation.eulerAngles.x )>180 && full)
        {
            dust.transform.position = gameObject.transform.GetChild(0).gameObject.transform.position;
            dust.transform.eulerAngles = new Vector3(75, 0, 0);
            GameObject child = gameObject.transform.GetChild(0).gameObject;
            GameObject newObject = Instantiate(child,child.transform.position, child.transform.rotation);
            dust.Play();
            child.SetActive(false);
            newObject.transform.parent = null;
            newObject.AddComponent<Rigidbody>();
            full = false;
            newObject.AddComponent<CanBeMixed>();
            newObject.AddComponent<CanBeMerged>();
        }

	}

}
