using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeDropandSmash : FoodCharacteristic {
    public GameObject smashable;
    public GameObject smashed;
    public GameObject smashedJuice;
    public AudioClip smashSound;

    GameObject _smashed;
    GameObject _smashedJuice;
    AudioSource source;
    // Use this for initialization
    void Start () {
        base.Start();

        source = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 3 && gameObject==smashable && gameObject.GetComponent<FoodStatus>().GetIsChoppedPiece()==false && collision.gameObject.GetComponent<CanBeChopped>()==null && collision.gameObject.GetComponent<CanChop>()==null)
        {
            _smashed = Instantiate(smashed, transform.position, smashed.transform.rotation);
            _smashedJuice = Instantiate(smashedJuice, transform.position, smashedJuice.transform.rotation);
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(smashSound, transform.position);
            GetComponent<FoodStatus>().SetIsSmashed(true);
        }
   
    }
}
