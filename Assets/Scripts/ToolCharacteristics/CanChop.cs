using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CanChop : ToolCharacteristic {
    private bool _canChop = true;
    private Vector3 _prevFramePosition;
    private float _translationAmount;

    public bool chopOnlyWhileToolOnHand = true;
    public float preventChoppingDelayOnGrab = 0.5f;

    public void Start()
    {
        _prevFramePosition = transform.position;

        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += OnGrab;
    }

    public void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        _canChop = false;

        StartCoroutine(GrabDelay());
    }

    IEnumerator GrabDelay()
    {
        yield return new WaitForSeconds(preventChoppingDelayOnGrab);

        _canChop = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        // Freeze this object when knife collides.
        if (comp && IsToolAvailable() && comp.ChopAvailability())
        {
            comp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        if (comp && IsToolAvailable() && comp.ChopAvailability() && GetIsMoving())
        {
            int rand = Random.Range(0, comp.choppingSoundBoard.Length);

            if (rand == comp.GetPrevPlayedSFX())
            {
                rand++;
                rand = rand % comp.choppingSoundBoard.Length;
            }

            PlayChoppingSound(comp.choppingSoundBoard[rand]);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        CanBeChopped comp = collision.collider.gameObject.GetComponent<CanBeChopped>();

        // Unfreeze this object when knife collision ends, also begin slicing process.
        if (comp && IsToolAvailable() && comp.ChopAvailability())
        {
            // Block new chopping requests until current one ends.
            comp.SetOnDelay(false);

            comp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            // Slice object using knife's current position & rotation
            comp.BeginSlice(transform.position, transform.up);
        }
    }

    public void PlayChoppingSound(AudioClip ac)
    {
        AudioSource auds = GetComponent<AudioSource>();
        if (!auds.isPlaying)
        {
            auds.clip = ac;
            auds.Play();
        }
    }

    public bool IsToolAvailable()
    {
        if(!_canChop)
        {
            return false;
        }

        if (chopOnlyWhileToolOnHand && !GetIsGrabbed())
        {
            return false;
        }


        return true;
    }

    public void FixedUpdate()
    {
        _translationAmount = (transform.position - _prevFramePosition).magnitude;
        _prevFramePosition = transform.position;
    }

    public bool GetIsMoving()
    {
        return _translationAmount >= 0.0005f;
    }

    // TODO Override OnGrab event, and add time delay for chopping stuff on first grab.
}