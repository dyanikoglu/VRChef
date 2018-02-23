using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : FoodCharacteristic {
    public GameObject smashable;
    public GameObject smashed;
    public AudioClip smashSound;

    private int smashCount;
    GameObject _smashed;
    AudioSource source;
    bool isBoiled = true;
    bool isPeeled;
    void Start()
    {
        smashCount = 0;
        source = gameObject.AddComponent<AudioSource>();
        isPeeled= base.GetIsPeeled();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<CanSmash>() != null && isBoiled && isPeeled && col.gameObject.GetComponent<CanMixedIn>()!=null)
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
