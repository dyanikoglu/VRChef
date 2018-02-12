using UnityEngine;
using System;

namespace Obi
{
	public class ObiSphericalForceZone : ObiExternalForce
	{

		public float radius = 5;
		public bool radial = true;

		public void OnEnable(){
			foreach(ObiSolver solver in affectedSolvers)
				solver.RequireRenderablePositions();
		}

		public void OnDisable(){
			foreach(ObiSolver solver in affectedSolvers)
				solver.RelinquishRenderablePositions();
		}

		public override void ApplyForcesToActor(ObiActor actor){

			Matrix4x4 l2sTransform;
			if (actor.Solver.simulateInLocalSpace)
				l2sTransform = actor.Solver.transform.worldToLocalMatrix * transform.localToWorldMatrix;
			else 
				l2sTransform = transform.localToWorldMatrix;
			
			Vector4 directionalForce = l2sTransform.MultiplyVector(Vector3.forward * (intensity + GetTurbulence(turbulence)));

			float sqrRadius = radius * radius;

			// Allocate forces array:
			Vector4[] forces = new Vector4[actor.particleIndices.Length];
			Vector4 center = new Vector4(transform.position.x,transform.position.y,transform.position.z);

			// Calculate force intensity for each actor particle:
			for (int i = 0; i < forces.Length; ++i){

				Vector4 distanceVector = actor.Solver.renderablePositions[actor.particleIndices[i]] - center;

				float sqrMag = distanceVector.sqrMagnitude;
				float falloff = Mathf.Clamp01((sqrRadius - sqrMag) / sqrRadius);

				if (radial)
					forces[i] = distanceVector/(Mathf.Sqrt(sqrMag) + float.Epsilon) * falloff * intensity;
				else
					forces[i] = directionalForce * falloff;

				forces[i][3] = actor.UsesCustomExternalForces ? 1 : 0;
			}			

			Oni.AddParticleExternalForces(actor.Solver.OniSolver,forces,actor.particleIndices,actor.particleIndices.Length);

		}

		public void OnDrawGizmosSelected(){
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(0,0.7f,1,1);
			Gizmos.DrawWireSphere(Vector3.zero,radius);

			float turb = GetTurbulence(1);

			if (!radial){
				ObiUtils.DrawArrowGizmo(radius + turb,radius*0.2f,radius*0.3f,radius*0.2f);
			}else{
				Gizmos.DrawLine(new Vector3(0,0,-radius*0.5f)*turb,new Vector3(0,0,radius*0.5f)*turb);
				Gizmos.DrawLine(new Vector3(0,-radius*0.5f,0)*turb,new Vector3(0,radius*0.5f,0)*turb);
				Gizmos.DrawLine(new Vector3(-radius*0.5f,0,0)*turb,new Vector3(radius*0.5f,0,0)*turb);
			}
		}
	}
}

