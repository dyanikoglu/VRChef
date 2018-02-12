using UnityEngine;
using System.Collections;

namespace Obi{

	[System.Flags]
	public enum ParticleData{
		NONE = 0,
		ACTIVE_STATUS = 1 << 0,
		ACTOR_ID = 1 << 1,
		POSITIONS = 1 << 2,
		VELOCITIES = 1 << 3,
		INV_MASSES = 1 << 4,
		VORTICITIES = 1 << 5,
		SOLID_RADII = 1 << 6,
		PHASES = 1 << 7,
		REST_POSITIONS = 1 << 8,
		COLLISION_MATERIAL = 1 << 9,
		ALL = ~0
	}

	/**
   	 * Interface for components that want to benefit from the simulation capabilities of an ObiSolver.
	 */
	public interface IObiSolverClient
	{
		bool AddToSolver(object info);
		bool RemoveFromSolver(object info);
		void PushDataToSolver(ParticleData data = ParticleData.NONE);
		void PullDataFromSolver(ParticleData data = ParticleData.NONE);
	}
}

