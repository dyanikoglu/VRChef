using UnityEngine;
using System;

namespace Obi
{
	public abstract class ObiExternalForce : MonoBehaviour
	{

		public float intensity = 0;
		public float turbulence = 0;
		public float turbulenceFrequency = 1;
		public float turbulenceSeed = 0;
		public ObiSolver[] affectedSolvers;

		public void LateUpdate(){
			foreach(ObiSolver solver in affectedSolvers){
				if (solver != null)
				foreach(ObiActor actor in solver.actors){
					if (actor != null)
						ApplyForcesToActor(actor);
				}
			}
		}

		protected float GetTurbulence(float turbulenceIntensity){
			return Mathf.PerlinNoise(Time.fixedTime * turbulenceFrequency,turbulenceSeed) * turbulenceIntensity;
		}

		public abstract void ApplyForcesToActor(ObiActor actor);	
		
	}
}

