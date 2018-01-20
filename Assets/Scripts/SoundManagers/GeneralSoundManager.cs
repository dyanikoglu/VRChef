using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GeneralSoundManager : MonoBehaviour {

    public AudioClip[] dropSoundBoard;
    public AudioClip[] grabSoundBoard;

    public AudioSource grabAudioSource;
    public AudioSource dropAudioSource;


    // Use this for initialization
    void Start () {
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += OnGrab;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        if (!grabAudioSource.isPlaying)
        {
            grabAudioSource.clip = grabSoundBoard[Random.Range(0, grabSoundBoard.Length)];
            grabAudioSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO Add ignore collision tags for specific object types (e.g fruits can't play drop sound when collided with knifes)

        // DROPPING SOUND EFFECT
        float dropVelocity = GetComponent<Rigidbody>().velocity.magnitude;
        
        if (!GetComponent<VRTK_InteractableObject>().IsGrabbed() && dropVelocity > 0.15f)
        {
            if (!dropAudioSource.isPlaying && dropSoundBoard.Length != 0)
            {
                dropAudioSource.clip = dropSoundBoard[Random.Range(0, dropSoundBoard.Length)];
                dropAudioSource.volume = Mathf.Clamp(dropVelocity / 10f, 0f, 1f);
                dropAudioSource.Play();
            }
        }
    }
}
