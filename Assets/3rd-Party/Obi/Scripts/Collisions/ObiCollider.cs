using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

	/**
	 * Add this component to any Collider that you want to be considered by Obi.
	 */
	[ExecuteInEditMode]
	[RequireComponent(typeof(Collider))]
	public class ObiCollider : ObiColliderBase
	{

		/**
		 * Creates an OniColliderTracker of the appropiate type.
   		 */
		protected override void CreateTracker(){ 

			if (unityCollider is SphereCollider)
				tracker = new ObiSphereShapeTracker((SphereCollider)unityCollider);
			else if (unityCollider is BoxCollider)
				tracker = new ObiBoxShapeTracker((BoxCollider)unityCollider);
			else if (unityCollider is CapsuleCollider)
				tracker = new ObiCapsuleShapeTracker((CapsuleCollider)unityCollider);
			else if (unityCollider is CharacterController)
				tracker = new ObiCapsuleShapeTracker((CharacterController)unityCollider);
			else if (unityCollider is TerrainCollider)
				tracker = new ObiTerrainShapeTracker((TerrainCollider)unityCollider);
			else if (unityCollider is MeshCollider)
				tracker = new ObiMeshShapeTracker((MeshCollider)unityCollider);
			else 
				Debug.LogWarning("Collider type not supported by Obi.");

		}
	
		protected override bool IsUnityColliderEnabled(){
			return ((Collider)unityCollider).enabled;
		}

		protected override void UpdateColliderAdaptor(){

			adaptor.Set((Collider)unityCollider,phase, thickness);

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

			unityCollider = GetComponent<Collider>(); 

			if (unityCollider == null)
				return;

			base.Awake();
		}

	}
}

