using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeDropandSmash : MonoBehaviour {
    public GameObject smashable;
    public GameObject smashed;

    // Use this for initialization
    void Start () {
        smashable.SetActive(true);
        smashed.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 3 && gameObject==smashable)
        {
            smashable.SetActive(false);
            smashed.transform.position = smashable.transform.position;
            smashed.transform.rotation = smashable.transform.rotation;
            smashed.SetActive(true);
        }
   
    }
}
