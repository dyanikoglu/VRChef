using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CanTake: ToolCharacteristic {

    // Use this for initialization
    ParticleSystem dust;
    public static bool full = false;
	void Start () {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        dust = GetComponentInChildren<ParticleSystem>();
    }
    GameObject poured;
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space") && GetIsGrabbed() && full)
        {
            Vector3 pos = dust.transform.position;
            pos.y = gameObject.transform.position.y;
            pos.x = gameObject.transform.position.x;
            dust.transform.position = pos; 
            dust.Play(); 
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            full = false;
        }

        
	}
}
