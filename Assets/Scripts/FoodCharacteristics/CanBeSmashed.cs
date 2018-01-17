using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : MonoBehaviour {
    public GameObject potato;
    public GameObject mashedPotato;
    public int smashCount;

    void Start()
    {
        potato.SetActive(true);
        mashedPotato.SetActive(false);
        smashCount = 0;
    }

    void OnTriggerEnter(Collider canSmash)
    {
        if (canSmash.gameObject.CompareTag("Smashable"))
        {
            Debug.Log("smashed");
            potato.SetActive(false);
            mashedPotato.transform.position = potato.transform.position;
            mashedPotato.transform.rotation = potato.transform.rotation;
            mashedPotato.SetActive(true);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "silicone_spatula")
        {
            if (smashCount >= 5)
            {
                potato.SetActive(false);
                mashedPotato.transform.position = potato.transform.position;
                mashedPotato.transform.rotation = potato.transform.rotation;
                mashedPotato.SetActive(true);
            }
            else
            {
                smashCount = smashCount + 1;
            }
            
        }
    }

}
