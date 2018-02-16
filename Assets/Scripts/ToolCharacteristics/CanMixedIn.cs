using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CanMixedIn : ToolCharacteristic {
    private bool collided;
    public Obi.ObiEmitter emitter;
    private int rotateCount;

    // Use this for initialization
    void Start () {
        collided=false;
        rotateCount=0;
        ObiEmitterMaterialFluid material = (ObiEmitterMaterialFluid)emitter.gameObject.GetComponent<ObiEmitter>().EmitterMaterial;
        material.atmosphericPressure = (float)0;
        emitter.Solver.UpdateActiveParticles();
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanBeMixed>() != null && !collided)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collided = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanMix>() != null)
        {
            for (int i=1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).transform.Rotate(Vector3.up * 400 * Time.deltaTime, Space.World); 
            }
            ObiEmitterMaterialFluid material = (ObiEmitterMaterialFluid)emitter.gameObject.GetComponent<ObiEmitter>().EmitterMaterial;
            material.atmosphericPressure = (float)10;
            emitter.Solver.UpdateActiveParticles();
            rotateCount++;
            if (rotateCount > 200)
            {
                gameObject.transform.GetChild(0).gameObject.transform.position = gameObject.transform.position;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                emitter.enabled = false;
                for (int i = 1; i < gameObject.transform.childCount; i++)
                {
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                    material.atmosphericPressure = (float)0;
                    emitter.Solver.UpdateActiveParticles();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CanMix>() != null)
        {
            ObiEmitterMaterialFluid material = (ObiEmitterMaterialFluid)emitter.gameObject.GetComponent<ObiEmitter>().EmitterMaterial;
            material.atmosphericPressure = (float)0;
            emitter.Solver.UpdateActiveParticles();
        }
    }



}
