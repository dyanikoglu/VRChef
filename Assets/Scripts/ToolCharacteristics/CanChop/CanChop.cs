using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

// Main class for objects that can chop meshes.
public class CanChop : ToolCharacteristic {
    private bool _canChop = true;
    private Vector3 _prevFramePosition;
    private float _translationAmount;

    // Chop object only if this object is in grabbed state.
    public bool chopOnlyWhileToolOnHand = true;

    // After grabbing this object, chopping actions will be prevented for 'preventChoppingDelayOnGrab' seconds.
    public float preventChoppingDelayOnGrab = 0.5f;

    // If knife changed it's position as 'minTranslationAmountForMoving' in world coordinates minimum, we will say that knife is moving.
    public float minTranslationAmountForMoving = 0.0005f;

    // After 'intersectionCheckCooldown' seconds the OnTriggerExit event fired, check if knife is still intersecting with victim object. 
    // If so, do not slice the object.
    public float intersectionCheckCooldown = 0.1f;

    public SharpArea sharpAreaRef;
    public GameObject fluidEmitterRef;
    public float spawnedFluidSpeed = 0.05f;
    public float fluidSpawnTimeInterval = 0.25f;

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

    private void FixedUpdate()
    {
        // Update position values of object. Required for detecting knife's movement status.
        _translationAmount = (transform.position - _prevFramePosition).magnitude;
        _prevFramePosition = transform.position;
    }

    // Returns true if knife is visibly moving
    public bool GetIsMoving()
    {
        return _translationAmount >= minTranslationAmountForMoving;
    }
}