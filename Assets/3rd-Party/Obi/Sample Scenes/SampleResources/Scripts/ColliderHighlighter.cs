using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class ColliderHighlighter : MonoBehaviour {

 	ObiSolver solver;

	void Awake(){
		solver = GetComponent<Obi.ObiSolver>();
	}

	void OnEnable () {
		solver.OnCollision += Solver_OnCollision;
	}

	void OnDisable(){
		solver.OnCollision -= Solver_OnCollision;
	}
	
	void Solver_OnCollision (object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
	{
		foreach(Oni.Contact c in e.contacts)
		{
			// make sure this is an actual contact:
			if (c.distance < 0.01f)
			{
				// get the collider:
				Collider collider = ObiCollider.idToCollider[c.other] as Collider;

				// make it blink:
				Blinker blinker = collider.GetComponent<Blinker>();

				if (blinker)
					blinker.Blink();
			}
		}
	}
}
