using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : MonoBehaviour {
    public GameObject potato;
    public GameObject mashedPotato;

    void Start()
    {
        potato.SetActive(true);
        mashedPotato.SetActive(false);
    }

    void OnTriggerEnter(Collider canSmash)
    {
        if (canSmash.gameObject.CompareTag("Smashable"))
        {
            potato.SetActive(false);
            mashedPotato.transform.position = potato.transform.position;
            mashedPotato.transform.rotation = potato.transform.rotation;
            mashedPotato.SetActive(true);
        }
    }
    
}
