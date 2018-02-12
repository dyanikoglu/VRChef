using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Obi{

/**
 * Class to hold per-actor information for a kind of constraints.
 *
 * You can only add or remove constraints when the actor is not in the solver. If you need to continously
 * add and remove constraints, the best approach is to reserve a bunch of constraints beforehand and then
 * individually activate/deactivate/update them.
 */
[ExecuteInEditMode]
public abstract class ObiBatchedConstraints : MonoBehaviour, IObiSolverClient 
{	

	public bool visualize = false;
	[NonSerialized] protected ObiActor actor;
	[NonSerialized] protected bool inSolver;

	public ObiActor Actor{
		get{return actor;}
	}

	public bool InSolver{
		get{return inSolver;}
	}

	/*public int ConstraintCount{
		get{return constraintCount;}
	}*/


	/**
	 * Returns a list of all constraint indices involving at least one the provided particle indices.
	 */
	//public abstract List<int> GetConstraintsInvolvingParticle(int particleIndex);

	public abstract Oni.ConstraintType GetConstraintType();

	public abstract List<ObiConstraintBatch> GetBatches();
	public abstract void Clear();

	protected void OnAddToSolver(object info){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.AddToSolver(this);
	}
	protected void OnRemoveFromSolver(object info){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.RemoveFromSolver(this);
	}

	public void PushDataToSolver(ParticleData data = ParticleData.NONE){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.PushDataToSolver(this);
	}
	public void PullDataFromSolver(ParticleData data = ParticleData.NONE){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.PullDataFromSolver(this);
	}
	public void SetActiveConstraints(){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.SetActiveConstraints();
	}

	public void Enable(){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.Enable();
	}
	public void Disable(){
		foreach(ObiConstraintBatch batch in GetBatches())
			batch.Disable();
	}

	public bool AddToSolver(object info){

		if (inSolver || actor == null || !actor.InSolver)
			return false;

		// custom addition code:
		OnAddToSolver(info);

		inSolver = true;

		// push data to solver:
		PushDataToSolver();	

		// set active constraints:
		SetActiveConstraints();

		// enable/disable all batches:
		if (isActiveAndEnabled)
			Enable();
		else 
			Disable();

		return true;

	}

	public bool RemoveFromSolver(object info){

		if (!inSolver || actor == null || !actor.InSolver)
			return false;

		OnRemoveFromSolver(null);

		inSolver = false;

		return true;

	}

	public void GrabActor(){
		actor = GetComponent<ObiActor>();
	}

	public void OnEnable(){
		
		Enable();
		
	}
	
	public void OnDisable(){

		if (actor == null || !actor.InSolver)
			return;

		Disable();
		
	}
	
	public void OnDestroy(){
		RemoveFromSolver(null);
	}
}
}

