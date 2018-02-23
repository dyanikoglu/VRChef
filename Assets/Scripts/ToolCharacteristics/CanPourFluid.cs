using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CanPourFluid : MonoBehaviour {

    public ObiEmitter emitter;
    public float pouringSpeed;

	// Use this for initialization
	void Start () {
        emitter.speed = 0f;
    }
	
	// Update is called once per frame
	void Update () {
	    if( (transform.rotation.eulerAngles.x <= 270 && transform.rotation.eulerAngles.x >= 90)	|| 
            (transform.rotation.eulerAngles.z <= 270 && transform.rotation.eulerAngles.z >= 90))
        {
            emitter.speed = pouringSpeed;
        }
        else
        {
            emitter.speed = 0f;
        }
    }
}
