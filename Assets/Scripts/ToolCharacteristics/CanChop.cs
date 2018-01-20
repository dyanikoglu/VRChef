using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanChop : ToolCharacteristic {
    public bool chopOnlyWhileToolOnHand = true;

    public int currentChoppingObjectCount = 0;

    private Vector3 prevFramePosition;
    private float translationAmount;

    public void Start()
    {
        prevFramePosition = transform.position;
    }

    public void PlaySound(AudioClip ac)
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
        if (chopOnlyWhileToolOnHand && !GetIsGrabbed())
        {
            return false;
        }

        return true;
    }

    public void FixedUpdate()
    {
        translationAmount = (transform.position - prevFramePosition).magnitude;
        prevFramePosition = transform.position;
    }

    public bool GetIsMoving()
    {
        return translationAmount >= 0.0005f;
    }

    public bool GetCurrentChoppingState()
    {
        return currentChoppingObjectCount != 0;
    }

    // TODO Override OnGrab event, and add time delay for chopping stuff on first grab.
}