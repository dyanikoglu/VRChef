using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Obi
{
/**
 * ObiStitcher will create stitch constraints between 2 actors. All actors must be assigned to the same solver.
 * - Add the constraint batch to the solver once all actors have been added.
 * - If any of the actors is removed from the solver, remove the stitcher too.
 * - Stitch constraints can keep n particles together, respecting their masses.
 * - In edit mode, select the actors to be stitched and then select groups of particles and click "stitch". Or, create stitches by closeness.
 */
[ExecuteInEditMode]
[AddComponentMenu("Physics/Obi/Obi Stitcher")]
public class ObiStitcher : MonoBehaviour, IObiSolverClient
{	
	[Serializable]
	public class Stitch{
		public int particleIndex1;
		public int particleIndex2;

		public Stitch(int particleIndex1, int particleIndex2){
			this.particleIndex1 = particleIndex1;
			this.particleIndex2 = particleIndex2;
		}
	}

	[SerializeField][HideInInspector] private List<Stitch> stitches = new List<Stitch>();

	[SerializeField][HideInInspector] private ObiActor actor1 = null;			/**< one of the actors used by the stitcher.*/
	[SerializeField][HideInInspector] private ObiActor actor2 = null;			/**< the second actor used by the stitcher.*/	

	private IntPtr batch;
	private bool inSolver = false;

	public ObiActor Actor1{
		set{
			if (actor1 != value)
			{
				if (actor1 != null)
				{
					actor1.OnAddedToSolver -= Actor_OnAddedToSolver;
					actor1.OnRemovedFromSolver -= Actor_OnRemovedFromSolver;

					if (actor1.InSolver){
						Actor_OnRemovedFromSolver(actor1,new ObiActor.ObiActorSolverArgs(actor1.Solver));
					}
				}

				actor1 = value;

				if (actor1 != null)
				{
					actor1.OnAddedToSolver += Actor_OnAddedToSolver;
					actor1.OnRemovedFromSolver += Actor_OnRemovedFromSolver;

					if (actor1.InSolver){
						Actor_OnAddedToSolver(actor1,new ObiActor.ObiActorSolverArgs(actor1.Solver));
					}
				}
			}
		}
		get{return actor1;}
	}

	public ObiActor Actor2{
		set{
			if (actor2 != value)
			{
				if (actor2 != null)
				{
					actor2.OnAddedToSolver -= Actor_OnAddedToSolver;
					actor2.OnRemovedFromSolver -= Actor_OnRemovedFromSolver;

					if (actor2.InSolver){
						Actor_OnRemovedFromSolver(actor2,new ObiActor.ObiActorSolverArgs(actor2.Solver));
					}
				}

				actor2 = value;

				if (actor2 != null)
				{
					actor2.OnAddedToSolver += Actor_OnAddedToSolver;
					actor2.OnRemovedFromSolver += Actor_OnRemovedFromSolver;

					if (actor2.InSolver){
						Actor_OnAddedToSolver(actor2,new ObiActor.ObiActorSolverArgs(actor2.Solver));
					}
				}
			}
		}
		get{return actor2;}
	}

	public int StitchCount{
		get{return stitches.Count;}
	}

	public IEnumerable<Stitch> Stitches{
		get{return stitches.AsReadOnly();}
	}

	public void OnEnable(){

		if (actor1 != null){
			actor1.OnAddedToSolver += Actor_OnAddedToSolver;
			actor1.OnRemovedFromSolver += Actor_OnRemovedFromSolver;
		}
		if (actor2 != null){
			actor2.OnAddedToSolver += Actor_OnAddedToSolver;
			actor2.OnRemovedFromSolver += Actor_OnRemovedFromSolver;
		}

		if (actor1 != null && actor2 != null)
			Oni.EnableBatch(batch,true);
	}	

	public void OnDisable(){
		Oni.EnableBatch(batch,false);
	}	

	/**
     * Adds a new stitch to the stitcher. Note that unlike calling Clear(), AddStitch does not automatically perform a
	 * PushDataToSolver(). You should manually call it once you're done adding multiple stitches.
	 * @param index of a particle in the first actor. 
     * @param index of a particle in the second actor.
	 * @return constrant index, that can be used to remove it with a call to RemoveStitch.
     */
	public int AddStitch(int particle1, int particle2){
		stitches.Add(new Stitch(particle1, particle2));
		return stitches.Count-1;
	}

	/**
     * Removes. Note that unlike calling Clear(), AddStitch does not automatically perform a
	 * PushDataToSolver(). You should manually call it once you're done adding multiple stitches.
	 * @param constraint index.
     */
	public void RemoveStitch(int index){
		if (index >= 0 && index < stitches.Count)
			stitches.RemoveAt(index);
	}

	public void Clear(){
		stitches.Clear();
		PushDataToSolver(ParticleData.NONE);
	}

	void Actor_OnRemovedFromSolver (object sender, ObiActor.ObiActorSolverArgs e)
	{
		// when any actor is removed from solver, remove stitches.
		this.RemoveFromSolver(null);
	}

	void Actor_OnAddedToSolver (object sender, ObiActor.ObiActorSolverArgs e)
	{
		// when both actors are in the same solver, add stitches.
		if (actor1.InSolver && actor2.InSolver){
			
			if (actor1.Solver != actor2.Solver){
				Debug.LogError("ObiStitcher cannot handle actors in different solvers.");
				return;
			}

			this.AddToSolver(null);
		}
	}

	public bool AddToSolver(object info){

		// create a constraint batch:
		batch = Oni.CreateBatch((int)Oni.ConstraintType.Stitch,false);
		Oni.AddBatch(actor1.Solver.OniSolver,batch,false);

		inSolver = true;

		// push current data to solver:
		PushDataToSolver(ParticleData.NONE);

		// enable/disable the batch:
		if (isActiveAndEnabled)
			OnEnable();
		else 
			OnDisable();

		return true;

	}

	public bool RemoveFromSolver(object info){

		// remove the constraint batch from the solver 
		// (no need to destroy it as its destruction is managed by the solver)
		Oni.RemoveBatch(actor1.Solver.OniSolver,batch);

		// important: set the batch pointer to null, as it could be destroyed by the solver.
		batch = IntPtr.Zero;

		inSolver = false;

		return true;
	}

	public void PushDataToSolver(ParticleData data = ParticleData.NONE){

		if (!inSolver)
			return;

		// set solver constraint data:
		int[] solverIndices = new int[stitches.Count*2];
		float[] stiffnesses = new float[stitches.Count];

		for (int i = 0; i < stitches.Count; i++)
		{
			solverIndices[i*2] = actor1.particleIndices[stitches[i].particleIndex1];
			solverIndices[i*2+1] = actor2.particleIndices[stitches[i].particleIndex2];
			stiffnesses[i] = 0;
		}	

		Oni.SetStitchConstraints(batch,solverIndices,stiffnesses,stitches.Count);

		// set active constraints:
		int[] activeConstraints = new int[stitches.Count];
		for (int i = 0; i < stitches.Count; ++i) 
			activeConstraints[i] = i;
		Oni.SetActiveConstraints(batch,activeConstraints,stitches.Count);

	}

	public void PullDataFromSolver(ParticleData data = ParticleData.NONE){
	}
	
}
}

