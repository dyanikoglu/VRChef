using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CanPourFluid : MonoBehaviour {

    public ObiEmitter emitter;
    public float pouringSpeed;
    public bool isBowl = false;

    private int pourWaterCalledAt = 0;

	// Use this for initialization
	void Start () {
        emitter.speed = 0f;
    }
	
	// Update is called once per frame
	void Update () {

        if (isBowl)
        { 
            if ((transform.rotation.eulerAngles.x <= 360 && transform.rotation.eulerAngles.x >= 320) ||
                (transform.rotation.eulerAngles.x <= 15 && transform.rotation.eulerAngles.x >= 0))
            {
                emitter.speed = pouringSpeed;
            }
            else
            {
                emitter.speed = 0f;
            }

            if(emitter.ActiveParticles % transform.parent.GetComponentInChildren<CanSqueeze>().GetNumParticle() == 0 && emitter.ActiveParticles != 0 && pourWaterCalledAt != emitter.ActiveParticles)            
            {
                pourWaterCalledAt = emitter.ActiveParticles;
                transform.parent.GetComponentInChildren<CanSqueeze>().PourWater();
            }
        }
        else
        {
            if ((transform.rotation.eulerAngles.x <= 270 && transform.rotation.eulerAngles.x >= 90) ||
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
}
