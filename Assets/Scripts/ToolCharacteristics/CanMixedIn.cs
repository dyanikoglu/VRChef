using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanMixedIn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CanBeMixed[] ingredients = FindObjectsOfType<CanBeMixed>();
        foreach (CanBeMixed ingredient in ingredients)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), ingredient.GetComponent<Collider>());
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
