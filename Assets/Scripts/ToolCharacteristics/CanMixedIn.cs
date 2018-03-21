using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CanMixedIn : ToolCharacteristic {
    private bool collided;
    public List<Obi.ObiEmitter> emitters;
    private int rotateCount;
    public ParticleSystem dust;

    // Use this for initialization
    void Start () {
        collided=false;
        rotateCount=0;
        Vector3 _scale = gameObject.transform.GetChild(2).transform.localScale;
        _scale.x = 0;
        _scale.y = 0;
        _scale.z = 0;
        gameObject.transform.GetChild(2).transform.localScale = _scale;
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
            for (int i=2; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).gameObject.GetComponent<CanBeMerged>() != null)
                {
                    dust.Play();
                }
                else
                {
                    gameObject.transform.GetChild(i).transform.Rotate(Vector3.up * 400 * Time.deltaTime, Space.World);
                }
            }
            rotateCount++;
            if (rotateCount > 100)
            {
                GetComponent<FoodStatus>().SetIsMixed(true);

                gameObject.transform.GetChild(1).gameObject.transform.position = gameObject.transform.position;
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
                foreach(Obi.ObiEmitter emitter in emitters)
                {
                    emitter.enabled = false;
                }
                for (int i = 2; i < gameObject.transform.childCount; i++)
                {
                    Renderer rend = gameObject.transform.GetChild(i).gameObject.GetComponent<Renderer>();
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                }
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }






}
