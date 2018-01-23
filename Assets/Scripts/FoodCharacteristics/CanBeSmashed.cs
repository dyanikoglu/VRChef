using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : MonoBehaviour {
    public GameObject smashable;
    public GameObject smashed;
    private int smashCount;

    void Start()
    {
        smashable.SetActive(true);
        smashed.SetActive(false);
        smashCount = 0;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<CanSmash>() != null)
        {
            if (smashCount == 5)
            {
                smashable.SetActive(false);
                smashed.transform.position = smashable.transform.position;
                smashed.transform.rotation = smashable.transform.rotation;
                smashed.SetActive(true);
                smashCount++;
            }
            else if (smashCount == 10){
                smashed.SetActive(false);
                smashable.transform.position = smashed.transform.position;
                smashable.transform.rotation = smashed.transform.rotation;
                smashable.SetActive(true);
            }
            else
            {
                smashCount++;
            }
            
        }
    }

}
