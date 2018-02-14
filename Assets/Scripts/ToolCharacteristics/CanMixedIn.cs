using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CanMixedIn : ToolCharacteristic {
    private bool collided;
    public Obi.ObiEmitter emitter;
    private int rotateCount;
    public GameObject finalObject;
    // Use this for initialization
    void Start () {
        collided=false;
        rotateCount=0;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanBeMixed>() != null && !collided)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collided = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanMix>() != null)
        {
            for (int i=1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).transform.Rotate(Vector3.up * 400 * Time.deltaTime, Space.World); 
            }
            rotateCount++;
            if (rotateCount > 200)
            {
                GameObject newObject = Instantiate(finalObject, transform.position, finalObject.transform.rotation);
                emitter.enabled = false;
                for (int i = 1; i < gameObject.transform.childCount; i++)
                {
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }



}
