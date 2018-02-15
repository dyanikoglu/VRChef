using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Obi;

public class CanChop : ToolCharacteristic {
    private bool _canChop = true;
    private Vector3 _prevFramePosition;
    private float _translationAmount;

    public bool chopOnlyWhileToolOnHand = true;
    public float preventChoppingDelayOnGrab = 0.5f;
    public SharpArea sharpAreaRef;
    public GameObject fluidEmitterRef;

    public void Start()
    {
        _prevFramePosition = transform.position;
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += OnGrab;

        if (!fluidEmitterRef)
        {
            Debug.LogError("Fluid Emitter Ref. can't be null in " + this.gameObject.name);
        }
    }

    public void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        // Put knife into delay for preventing immediate choppings due to teleportation of knife.
        StartCoroutine(GrabDelay());
    }

    IEnumerator GrabDelay()
    {
        _canChop = false;

        yield return new WaitForSeconds(preventChoppingDelayOnGrab);

        _canChop = true;
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

    // Is this tool currently available to cut some stuff?
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
        // Update position values of object. Required for detecting knife's movement status.
        _translationAmount = (transform.position - _prevFramePosition).magnitude;
        _prevFramePosition = transform.position;
    }

    // Returns if knife is visibly moving
    public bool GetIsMoving()
    {
        return _translationAmount >= 0.0005f;
    }
}