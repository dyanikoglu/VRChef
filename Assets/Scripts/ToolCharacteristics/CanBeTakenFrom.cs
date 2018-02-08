using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeTakenFrom : MonoBehaviour {


	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanTake>() != null && CanTake.full==false)
        {
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            CanTake.full = true;
            Vector3 pos = gameObject.transform.position;
            pos.y = pos.y - (float)0.015;
            gameObject.transform.position = pos;
        }
    }
}
