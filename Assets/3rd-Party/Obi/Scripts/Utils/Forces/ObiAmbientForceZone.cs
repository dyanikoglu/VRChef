using UnityEngine;
using System;

namespace Obi
{
	public class ObiAmbientForceZone : ObiExternalForce
	{

		public override void ApplyForcesToActor(ObiActor actor){

			Matrix4x4 l2sTransform;
			if (actor.Solver.simulateInLocalSpace)
				l2sTransform = actor.Solver.transform.worldToLocalMatrix * transform.localToWorldMatrix;
			else 
				l2sTransform = transform.localToWorldMatrix;
			
			Vector4 force = l2sTransform.MultiplyVector(Vector3.forward * (intensity + GetTurbulence(turbulence)));
			force[3] = actor.UsesCustomExternalForces ? 1 : 0;

			Oni.AddParticleExternalForce(actor.Solver.OniSolver,ref force,actor.particleIndices,actor.particleIndices.Length);

		}

		public void OnDrawGizmosSelected(){

			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(0,0.7f,1,1);

			// arrow body:
			ObiUtils.DrawArrowGizmo(0.5f + GetTurbulence(1),0.2f,0.3f,0.2f);
		}
	}
}

