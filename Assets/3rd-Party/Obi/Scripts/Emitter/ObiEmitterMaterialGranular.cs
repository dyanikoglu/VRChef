using UnityEngine;
using System.Collections;
using System;

namespace Obi
{
	public class ObiEmitterMaterialGranular : ObiEmitterMaterial
	{
	
		public float randomness = 0;
	
		public void OnValidate(){
	
			resolution = Mathf.Max(0.001f,resolution);
			restDensity =  Mathf.Max(0.001f,restDensity);
			randomness =  Mathf.Max(0,randomness);
		}
	
		public override Oni.FluidMaterial GetEquivalentOniMaterial(Oni.SolverParameters.Mode mode)
		{
			Oni.FluidMaterial material = new Oni.FluidMaterial();
			material.smoothingRadius = GetParticleSize(mode);
			material.restDensity = restDensity;
			material.viscosity = 0;
			material.surfaceTension = 0;
			material.buoyancy = -1;
			material.atmosphericDrag = 0;
			material.atmosphericPressure = 0;
			material.vorticity = 0;
			return material;
		}
	}
}

