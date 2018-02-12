using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class FluidJetController : MonoBehaviour {

	ObiEmitter emitter;
	public float emissionSpeed = 10;
	public float moveSpeed = 2;

	// Use this for initialization
	void Start () {
		emitter = GetComponentInChildren<ObiEmitter>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.W)){
			emitter.speed = emissionSpeed;
		}else{
			emitter.speed = 0;
		}

		if (Input.GetKey(KeyCode.A)){
			transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.D)){
			transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
		}

	}
}
