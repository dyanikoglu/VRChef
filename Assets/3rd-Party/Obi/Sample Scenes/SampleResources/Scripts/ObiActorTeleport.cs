using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class ObiActorTeleport : MonoBehaviour {

    public ObiActor actor;

	   void Update () {
        if (Input.anyKeyDown)
            StartCoroutine(Teleport());
	   }

    IEnumerator Teleport(){

        actor.enabled = false;

        // change position here. I'm just teleporting the actor somewhere inside a sphere of radius 2:
        actor.transform.position = UnityEngine.Random.insideUnitSphere * 2;

        yield return new WaitForFixedUpdate();
        actor.enabled = true;

    }
}
