using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detects chopping states of choppable objects
public class SharpArea : MonoBehaviour {

    public CanChop canChopRef;

    private void Start()
    {
        if(canChopRef == null)
        {
            canChopRef = transform.parent.GetComponent<CanChop>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        // Freeze this object when knife collides.
        if (comp && canChopRef.IsToolAvailable() && comp.ChopAvailability() && !comp.GetStartedChopping())
        {
            comp.SetStartedChopping(true);
            comp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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