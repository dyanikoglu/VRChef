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

    IEnumerator FluidSpawn()
    {
        _obiEmitter.speed = 0.1f;
        yield return new WaitForSeconds(0.1f);
        _obiEmitter.speed = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        // Freeze this object when knife collides.
        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && !comp.GetStartedChopping())
        {
            comp.SetStartedChopping(true);
            comp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            if (comp.spawnFluid)
            {
                _obiParticleRenderer.particleColor = comp.spawnFluidColor;
                StartCoroutine(FluidSpawn());
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && canChopRef.GetIsMoving())
        {
            int rand = Random.Range(0, comp.choppingSoundBoard.Length);

            if (rand == comp.GetPrevPlayedSFX())
            {
                rand++;
                rand = rand % comp.choppingSoundBoard.Length;
            }

            if (comp.spawnFluid)
            {
                StartCoroutine(FluidSpawn());
            }

            canChopRef.PlayChoppingSound(comp.choppingSoundBoard[rand]);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        // Unfreeze this object when knife collision ends, also begin slicing process.
        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && comp.GetStartedChopping())
        {
            comp.SetStartedChopping(false);

            // Block new chopping requests until current one ends.
            comp.SetOnDelay(false);

            comp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            // Slice object using knife's current position & rotation
            comp.BeginSlice(transform.position, transform.up);
        }
    }
}