using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : MonoBehaviour {
    public GameObject smashable;
    public GameObject smashed;
    public AudioClip smashSound;

    private int smashCount;
    GameObject _smashed;
    AudioSource source;
    bool isBoiled=true;
    bool isPeeled = true;
    void Start()
    {
        smashCount = 0;
        source = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<CanSmash>() != null && isBoiled && isPeeled)
        {
            AudioSource.PlayClipAtPoint(smashSound, transform.position);

            if (smashCount == 4)
            {
                _smashed=Instantiate(smashed, smashable.transform.position,smashed.transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                smashCount++;
            }
            
        }
    }

}
