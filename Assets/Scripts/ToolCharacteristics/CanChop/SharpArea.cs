using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

// Detects chopping states of choppable objects
public class SharpArea : MonoBehaviour {

    private ObiEmitter _obiEmitter;
    private ObiParticleRenderer _obiParticleRenderer;

    public CanChop canChopRef;

    private void Start()
    {
        if(canChopRef == null)
        {
            canChopRef = transform.parent.GetComponent<CanChop>();
        }

        if (canChopRef.fluidEmitterRef != null)
        {
            _obiEmitter = canChopRef.fluidEmitterRef.GetComponent<ObiEmitter>();
            _obiParticleRenderer = canChopRef.fluidEmitterRef.GetComponent<ObiParticleRenderer>();
        }
    }

    private IEnumerator FluidSpawn()
    {
        _obiEmitter.speed = canChopRef.spawnedFluidSpeed;
        yield return new WaitForSeconds(canChopRef.fluidSpawnTimeInterval);
        _obiEmitter.speed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CanBeChopped comp = other.gameObject.GetComponent<CanBeChopped>();

        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && !comp.GetStartedChopping())
        {
            comp.SetStartedChopping(true);

            if (comp.spawnFluid)
            {
                _obiParticleRenderer.particleColor = comp.spawnFluidColor;
                StartCoroutine(FluidSpawn());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CanBeChopped comp = other.gameObject.GetComponent<CanBeChopped>();

        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && canChopRef.GetIsMoving() && comp.choppingSoundBoard.Length != 0)
        {
            int rand = Random.Range(0, comp.choppingSoundBoard.Length);

            if (rand == comp.GetPrevPlayedSFX())
            {
                rand++;
                rand = rand % comp.choppingSoundBoard.Length;
            }

            canChopRef.PlayChoppingSound(comp.choppingSoundBoard[rand]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CanBeChopped comp = other.gameObject.GetComponent<CanBeChopped>();

        // Unfreeze this object when knife collision ends, also begin slicing process.
        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && comp.GetStartedChopping())
        {
            comp.SetStartedChopping(false);

            // Block new chopping requests until current one ends.
            comp.SetOnDelay(false);

            // Slice object using knife's current position & rotation
            comp.BeginSlice(transform.position, transform.up);
        }
    }
}