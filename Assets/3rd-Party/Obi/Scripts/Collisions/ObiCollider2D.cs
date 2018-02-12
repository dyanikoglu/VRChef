using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

	/**
	 * Add this component to any Collider that you want to be considered by Obi.
	 */
	[ExecuteInEditMode]
	[RequireComponent(typeof(Collider2D))]
	public class ObiCollider2D : ObiColliderBase
	{

		/**
		 * Creates an OniColliderTracker of the appropiate type.
   		 */
		protected override void CreateTracker(){ 

			if (unityCollider is CircleCollider2D)
				tracker = new ObiCircleShapeTracker2D((CircleCollider2D)unityCollider);
			else if (unityCollider is BoxCollider2D)
				tracker = new ObiBoxShapeTracker2D((BoxCollider2D)unityCollider);
			else if (unityCollider is CapsuleCollider2D)
				tracker = new ObiCapsuleShapeTracker2D((CapsuleCollider2D)unityCollider);
			else if (unityCollider is EdgeCollider2D)
				tracker = new ObiEdgeShapeTracker2D((EdgeCollider2D)unityCollider);
			else 
				Debug.LogWarning("Collider2D type not supported by Obi.");

		}
	
		protected override bool IsUnityColliderEnabled(){
			return ((Collider2D)unityCollider).enabled;
		}

		protected override void UpdateColliderAdaptor(){
			adaptor.Set((Collider2D)unityCollider,phase, thickness);

			foreach(ObiSolver solver in solvers){
				if (solver.simulateInLocalSpace){

					adaptor.SetSpaceTransform(solver.transform);

					if (solvers.Count > 1){
						Debug.LogWarning("ObiColliders used by ObiSolvers simulating in local space cannot be shared by multiple solvers."+
								 		 "Please duplicate the collider if you want to use it in other solvers.");
						return;
					}
				}
			}
		}

		protected override void Awake(){

			unityCollider = GetComponent<Collider2D>(); 

			if (unityCollider == null)
				return;

			base.Awake();
		}

	}
}

