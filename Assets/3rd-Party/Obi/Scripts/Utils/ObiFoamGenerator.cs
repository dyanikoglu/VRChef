using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi
{
	/**
	 * Foam generators create diffuse particles in areas where certain conditions meet (high velocity constrasts, high vorticity, low density, high normal values, etc.). These particles
	 * are then advected trough the fluid velocity field.
	 */
	[RequireComponent(typeof(ObiEmitter))]
	public class ObiFoamGenerator : MonoBehaviour
	{
		ObiEmitter emitter;
		public float emissionRate = 500;
		public float randomness = 0.01f;
		public float vorticityThreshold = 10;
		public float densityThreshold = 2000;
		public ParticleAdvector advector;

		private float accumTime = 0;
		private List<Vector4> emitLocations;

		void Start(){}

		void Awake(){
			emitter = GetComponent<ObiEmitter>();
			emitter.OnAddedToSolver += Emitter_OnAddedToSolver;
			emitter.OnRemovedFromSolver += Emitter_OnRemovedFromSolver;
		}

		void Emitter_OnAddedToSolver (object sender, ObiActor.ObiActorSolverArgs e)
		{
			e.Solver.OnFluidUpdated += Solver_OnFluidUpdated;
		}

		void Emitter_OnRemovedFromSolver (object sender, ObiActor.ObiActorSolverArgs e)
		{
			e.Solver.OnFluidUpdated -= Solver_OnFluidUpdated;
		}

		void Solver_OnFluidUpdated (object sender, ObiSolver.ObiFluidEventArgs e)
		{
			if (!isActiveAndEnabled || advector == null || advector.solver == null)
				return;

			if (emitLocations == null)
				emitLocations = new List<Vector4>(emitter.particleIndices.Length);

			accumTime += Time.deltaTime;

			// calculate emission budget for this frame:
			int emitAmount = Mathf.FloorToInt(emissionRate * accumTime);

			// remove used emission time from accumulator:
			accumTime -= emitAmount / emissionRate; 

			if (emitAmount == 0) return;

			ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();

			emitLocations.Clear();

			// loop trough particles and decide where we should emit foam from:
			for (int i = 0; i < emitter.particleIndices.Length; ++i){

				int k = emitter.particleIndices[i];

				float vorticity = e.vorticities[k].magnitude;

				if (vorticity > vorticityThreshold && e.densities[k] < densityThreshold){
					emitLocations.Add(((ObiSolver)sender).renderablePositions[k]);
				}
			}

			// calculate how many particles we must skip each iteration to meet the budget:
			int step = Mathf.Max(1, Mathf.FloorToInt(emitLocations.Count / (float)emitAmount));

			// emit particles:
			if (advector.solver.parameters.mode == Oni.SolverParameters.Mode.Mode3D){
				for(int i = UnityEngine.Random.Range(0,step-1); i < emitLocations.Count; i += step){
					param.position = emitLocations[i] + (Vector4)UnityEngine.Random.insideUnitSphere * randomness;
					advector.Particles.Emit(param,1);
				}
			}else{
				for(int i = UnityEngine.Random.Range(0,step-1); i < emitLocations.Count; i += step){
					param.position = emitLocations[i] + (Vector4)UnityEngine.Random.insideUnitCircle * randomness;
					advector.Particles.Emit(param,1);
				}
			}
		}
	
	}
}

