using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour {

    public AudioClip coverOpen;
    public AudioClip coverCloseFast;
    public AudioClip coverCloseSlow;

    private bool isClosed = true;
	
	// Update is called once per frame
	void Update () {
        if(isClosed && transform.localRotation.eulerAngles.x < 89.9f)
        {
            isClosed = false;
            GetComponent<AudioSource>().clip = coverOpen;
            GetComponent<AudioSource>().Play();
        }

        else if(!isClosed && transform.localRotation.eulerAngles.x > 89.9f)
        {
            isClosed = true;
            GetComponent<AudioSource>().clip = coverCloseFast;
            GetComponent<AudioSource>().Play();
        }
    }
}