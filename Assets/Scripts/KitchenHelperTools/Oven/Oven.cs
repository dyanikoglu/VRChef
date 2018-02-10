using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Oven : MonoBehaviour {

    public Counter counterRef;

	// Use this for initialization
	void Start () {
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OnUse;
    }

    public void OnUse(object sender, InteractableObjectEventArgs e)
    {
        counterRef.SetCounter(3605);
        counterRef.StartCounter();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
