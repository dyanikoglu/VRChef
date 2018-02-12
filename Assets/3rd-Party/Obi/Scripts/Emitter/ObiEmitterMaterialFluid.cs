using UnityEngine;
using System.Collections;
using System;

namespace Obi
{
	public class ObiEmitterMaterialFluid : ObiEmitterMaterial
	{	
		// fluid parameters:
		public float smoothing = 1.5f;
		public float viscosity = 0.05f;			/**< viscosity of the fluid particles.*/
		public float surfaceTension = 0.1f;	/**< surface tension of the fluid particles.*/
	
		// gas parameters:
		public float buoyancy = -1.0f; 						/**< how dense is this material with respect to air?*/
		public float atmosphericDrag = 0;					/**< amount of drag applied by the surrounding air to particles near the surface of the material.*/
		public float atmosphericPressure = 0;				/**< amount of pressure applied by the surrounding air particles.*/
		public float vorticity = 0.0f;						/**< amount of baroclinic vorticity injected.*/
		
		// elastoplastic parameters:
		//public float elasticRange; 		/** radius around a particle in which distance constraints are created.*/
		//public float plasticCreep;		/**< rate at which a deformed plastic material regains its shape*/
		//public float plasticThreshold;	/**< amount of stretching stress that a elastic material must undergo to become plastic.*/

		public void OnValidate(){
	
			resolution = Mathf.Max(0.001f,resolution);
			restDensity =  Mathf.Max(0.001f,restDensity);
			smoothing = Mathf.Max(1,smoothing);
			viscosity = Mathf.Max(0,viscosity);
			atmosphericDrag = Mathf.Max(0,atmosphericDrag);
	
		}
	
		public override Oni.FluidMaterial GetEquivalentOniMaterial(Oni.SolverParameters.Mode mode)
		{
			Oni.FluidMaterial material = new Oni.FluidMaterial();

			// smoothing radius must be at least twice the particle radius, so that particle centers can reach each other. That's why the size is not multiplied by 0.5f.
			material.smoothingRadius = GetParticleSize(mode) * smoothing; 
			material.restDensity = restDensity;
			material.viscosity = viscosity;
			material.surfaceTension = surfaceTension;
			material.buoyancy = buoyancy;
			material.atmosphericDrag = atmosphericDrag;
			material.atmosphericPressure = atmosphericPressure;
			material.vorticity = vorticity;
			return material;
		}
	}
}

