using UnityEngine;
using System.Collections;
 
public class ObjectDragger : MonoBehaviour 
{
 
	private Vector3 screenPoint;
	private Vector3 offset;

	bool dragged = false;

	private Vector3 newPosition;
	private Rigidbody body;

	void Awake(){
		body = gameObject.GetComponent<Rigidbody>();
		newPosition = transform.position;
	}
	 
	 void OnMouseDown()
	 {
	     screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
	     offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	 }
	 
	 void OnMouseDrag()
	 {
		dragged = true;
	 }

	 void FixedUpdate(){
		if (dragged){

			dragged = false;

			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			newPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

			if (body != null)
				body.velocity = (newPosition - transform.position) / Time.deltaTime;
		}
	 }

	 void LateUpdate(){
		transform.position = newPosition;
	 }
 
 }