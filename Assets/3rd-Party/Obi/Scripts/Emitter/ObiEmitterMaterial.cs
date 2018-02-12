using UnityEngine;
using System;

namespace Obi{

/**
 * Base class for emitter materials, which hold information about the physical properties of the substance emitted by an emitter.
 */
public abstract class ObiEmitterMaterial : ScriptableObject
{
	public class MaterialChangeEventArgs : EventArgs{

		public MaterialChanges changes;

		public MaterialChangeEventArgs(MaterialChanges changes){
			this.changes = changes;
		}
	}

	[Flags]
	public enum MaterialChanges{
		PER_MATERIAL_DATA = 0,
		PER_PARTICLE_DATA = 1 << 0
	}

	public float resolution = 1;
	public float restDensity = 1000;		/**< rest density of the material.*/

	private EventHandler<MaterialChangeEventArgs> onChangesMade;
	public event EventHandler<MaterialChangeEventArgs> OnChangesMade {
	
	    add {
	        onChangesMade -= value;
	        onChangesMade += value;
	    }
	    remove {
	        onChangesMade -= value;
	    }
	}

	public void CommitChanges(MaterialChanges changes){
		if (onChangesMade != null)
				onChangesMade(this,new MaterialChangeEventArgs(changes));
	}

	/** 
     * Returns the diameter (2 * radius) of a single particle of this material.
     */
	public float GetParticleSize(Oni.SolverParameters.Mode mode){
		return 1f / (10 * Mathf.Pow(resolution,1/(mode == Oni.SolverParameters.Mode.Mode3D ? 3.0f : 2.0f)));
	}

	/** 
     * Returns the mass (in kilograms) of a single particle of this material.
     */
	public float GetParticleMass(Oni.SolverParameters.Mode mode){
		return restDensity * Mathf.Pow(GetParticleSize(mode),mode == Oni.SolverParameters.Mode.Mode3D ? 3 : 2);
	}

	public abstract Oni.FluidMaterial GetEquivalentOniMaterial(Oni.SolverParameters.Mode mode);
}
}

