using UnityEngine;
using System.Collections;

public class WaveGenerator : MonoBehaviour {

	public float amplitude = 2;
	public float frequency = 2;

	float angle = 0;
	float originalX = 0;

	void Start(){
		originalX = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		
		angle += Time.deltaTime * frequency;
		Vector3 pos = transform.position;
		pos.x = originalX + Mathf.Sin(angle) * amplitude;
		transform.position = pos;
	}
}
