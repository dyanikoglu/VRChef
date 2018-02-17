using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

// Detects chopping states of choppable objects
public class SharpArea : MonoBehaviour {

    private ObiEmitter _obiEmitter;
    private float velocity = 0;
    private Vector3 prevFrameLocation;
    private ObiParticleRenderer _obiParticleRenderer;

    public CanChop canChopRef;
    public Collider colliderAreaRef;
    public GameObject sharpAreaRef;
    public GameObject nonSharpAreaRef;
    
    private void Start()
    {
        prevFrameLocation = transform.position;

        if (canChopRef == null)
        {
            canChopRef = transform.parent.GetComponent<CanChop>();
        }

        if (canChopRef.fluidEmitterRef != null)
        {
            _obiEmitter = canChopRef.fluidEmitterRef.GetComponent<ObiEmitter>();
            _obiParticleRenderer = canChopRef.fluidEmitterRef.GetComponent<ObiParticleRenderer>();
        }

        if(!colliderAreaRef)
        {
            Physics.IgnoreCollision(colliderAreaRef, this.GetComponent<Collider>());
        }
    }

    private void FixedUpdate()
    {
        velocity = Vector3.Distance(transform.position, prevFrameLocation);
        prevFrameLocation = transform.position;
    }

    private IEnumerator FluidSpawn()
    {
        _obiEmitter.speed = canChopRef.spawnedFluidSpeed;
        yield return new WaitForSeconds(canChopRef.fluidSpawnTimeInterval);
        _obiEmitter.speed = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        CanBeChopped comp = other.gameObject.GetComponent<CanBeChopped>();

        if (comp && velocity > comp.minKnifeVelocityToChop && canChopRef.IsToolAvailable() && comp.ChopAvailability())
        {

            // If knife is not at a state that can slice the object (e.g, hand is holding it on reverse, or player is trying to cut object with non-sharp area), do not cut the object.
            //if(Vector3.Distance(sharpAreaRef.transform.position, other.gameObject.transform.position) < Vector3.Distance(nonSharpAreaRef.transform.position, other.gameObject.transform.position))
            //{
            //    return;
            //}
            ////

            comp.BeginSlice(transform.position, transform.up);

            if (comp.spawnFluid)
            {
                _obiParticleRenderer.particleColor = comp.spawnFluidColor;
                StartCoroutine(FluidSpawn());
            }
        }
    }
}