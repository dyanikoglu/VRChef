using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CanChop : ToolCharacteristic {
    private bool _canChop = true;
    private Vector3 _prevFramePosition;
    private float _translationAmount;

    public int currentChoppingObjectCount = 0;
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

    public bool GetCurrentChoppingState()
    {
        return currentChoppingObjectCount != 0;
    }

    // TODO Override OnGrab event, and add time delay for chopping stuff on first grab.
}