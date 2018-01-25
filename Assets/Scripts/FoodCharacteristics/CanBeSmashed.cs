using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : MonoBehaviour {
    public GameObject smashable;
    public GameObject smashed1;
    public GameObject smashed2;
    private int smashCount;

    void Start()
    {
        smashable.SetActive(true);
        smashed1.SetActive(false);
        smashed2.SetActive(false);
        smashCount = 0;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<CanSmash>() != null)
        {
            if (smashCount == 3)
            {
                smashable.SetActive(false);
                smashed1.transform.position = smashable.transform.position;
                smashed1.transform.rotation = smashable.transform.rotation;
                smashed1.SetActive(true);
                smashCount++;
            }
            else if (smashCount == 10){
                smashed1.SetActive(false);
                //smashed2.transform.position = smashed1.transform.position;
                smashed2.SetActive(true);
            }
            else
            {
                smashCount++;
            }
            
        }
    }

}
