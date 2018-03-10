using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

// Detects chopping states of choppable objects
public class SharpArea : MonoBehaviour {

    private ObiEmitter _obiEmitter;
    private float _velocity = 0;
    private Vector3 _prevFrameLocation;
    private ObiParticleRenderer _obiParticleRenderer;

    public CanChop canChopRef;
    public Collider colliderAreaRef;
    public GameObject sharpAreaRef;
    public GameObject nonSharpAreaRef;
    
    private void Start()
    {
        _prevFrameLocation = transform.position;

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
        _velocity = Vector3.Distance(transform.position, _prevFrameLocation);
        _prevFrameLocation = transform.position;
    }

    private IEnumerator FluidSpawn()
    {
        _obiEmitter.speed = canChopRef.spawnedFluidSpeed;
        yield return new WaitForSeconds(canChopRef.fluidSpawnTimeInterval);
        _obiEmitter.speed = 0;
    }

    private IEnumerator CheckIfStillInsideMesh(CanBeChopped comp, Collider sharpArea, Collider other)
    {
        yield return new WaitForSeconds(canChopRef.intersectionCheckCooldown);
        
        if(!sharpArea.bounds.Intersects(other.bounds))
        {
            comp.BeginSlice(transform.position, transform.up);

            if (comp.spawnFluid)
            {
                canChopRef.fluidEmitterRef.transform.position = other.transform.position;
                _obiParticleRenderer.particleColor = comp.spawnFluidColor;
                StartCoroutine(FluidSpawn());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CanBeChopped comp = other.gameObject.GetComponent<CanBeChopped>();

        if (CheckConditions(other, comp))
        {
            //Kinda hack. If knife is not at a state that it can slice the object(e.g, hand is holding it on reverse, or player is trying to cut object with non - sharp area), do not cut the object.
            Vector3 meshCenter = other.GetComponent<Renderer>().bounds.center; // Get mesh's center position in world coordinates
            if (Vector3.Distance(sharpAreaRef.transform.position, meshCenter) < Vector3.Distance(nonSharpAreaRef.transform.position, meshCenter))
            {
                return;
            }

            //Check if knife is still intersecting with victim mesh after a cooldown. Then, slice the mesh if conditions are met.
            StartCoroutine(CheckIfStillInsideMesh(comp, GetComponent<Collider>(), other));
        }
    }

    private bool CheckConditions(Collider other, CanBeChopped comp)
    {
        return comp && canChopRef.IsToolAvailable() && comp.ChopAvailability();
    }
}