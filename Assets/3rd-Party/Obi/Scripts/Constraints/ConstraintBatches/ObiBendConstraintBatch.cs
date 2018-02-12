using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Obi{

/**
 * Holds data about bend constraints for an actor.
 */
[Serializable]
public class ObiBendConstraintBatch : ObiConstraintBatch
{

	public enum BendIndexType{
		First = 0,
		Second = 1,
		Pivot = 2
	}

	[HideInInspector] public List<int> bendingIndices = new List<int>();				/**< Distance constraint indices.*/
	[HideInInspector] public List<float> restBends = new List<float>();					/**< Rest distances.*/
	[HideInInspector] public List<Vector2> bendingStiffnesses = new List<Vector2>();	/**< Bend offsets and stiffnesses of distance constraits.*/

	int[] solverIndices = new int[0];

	public ObiBendConstraintBatch(bool cooked, bool sharesParticles) : 
	base(cooked,sharesParticles){
	}

	public ObiBendConstraintBatch(bool cooked, bool sharesParticles, float minYoungModulus, float maxYoungModulus) : 
	base(cooked,sharesParticles,minYoungModulus,maxYoungModulus){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Bending;
	}

	public override void Clear(){
		activeConstraints.Clear();
		bendingIndices.Clear();
		restBends.Clear();
		bendingStiffnesses.Clear();
		constraintCount = 0;	
	}

	public void AddConstraint(int index1, int index2, int index3, float restBend, float bending, float stiffness){
		activeConstraints.Add(constraintCount);
		bendingIndices.Add(index1);
		bendingIndices.Add(index2);
		bendingIndices.Add(index3);
		restBends.Add(restBend);
		bendingStiffnesses.Add(new Vector2(bending,stiffness));
		constraintCount++;
	}

	public void InsertConstraint(int constraintIndex, int index1, int index2, int index3, float restBend, float bending, float stiffness){
		if (constraintIndex < 0 || constraintIndex > ConstraintCount)
			return;
		
		// update active indices:
		for (int i = 0; i < activeConstraints.Count; ++i){
			if (activeConstraints[i] >= constraintIndex)
				activeConstraints[i]++;
		}

		activeConstraints.Add(constraintIndex);

		bendingIndices.Insert(constraintIndex*3,index1);
		bendingIndices.Insert(constraintIndex*3+1,index2);
		bendingIndices.Insert(constraintIndex*3+2,index3);
		restBends.Insert(constraintIndex,restBend);
        bendingStiffnesses.Insert(constraintIndex,new Vector2(bending,stiffness));
		constraintCount++;
	}

	public void SetParticleIndex(int constraintIndex, int particleIndex, BendIndexType type, bool wraparound){
		if (!wraparound){
			if (constraintIndex >= 0 && constraintIndex < ConstraintCount)
				bendingIndices[constraintIndex*3+(int)type] = particleIndex;
		}else
			bendingIndices[(int)ObiUtils.Mod(constraintIndex,ConstraintCount)*3+(int)type] = particleIndex;

	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);
		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		bendingIndices.RemoveRange(index*3,3);
		restBends.RemoveAt(index);
	    bendingStiffnesses.RemoveAt(index);
		constraintCount--;
	}

	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(5);
		
		for (int i = 0; i < ConstraintCount; i++){
			if (bendingIndices[i*3] == particleIndex || bendingIndices[i*3+1] == particleIndex || bendingIndices[i*3+2] == particleIndex) 
				constraints.Add(i);
		}
		
		return constraints;
	}

	public override void Cook()
	{
		batch = Oni.CreateBatch((int)Oni.ConstraintType.Bending,true);
	
		// Send initial data to batch:
		Oni.SetBendingConstraints(batch,bendingIndices.ToArray(),restBends.ToArray(),bendingStiffnesses.ToArray(),ConstraintCount);

		// cook the batch and retrieve new sorted data:
		if (Oni.CookBatch(batch))
		{
			constraintCount = Oni.GetBatchConstraintCount(batch);
			activeConstraints = Enumerable.Range(0, constraintCount).ToList();

			int[] cookedIndices = new int[constraintCount*3]; 
			float[] cookedRestLengths = new float[constraintCount]; 
			Vector2[] cookedStiffnesses = new Vector2[constraintCount]; 

			Oni.GetBendingConstraints(batch,cookedIndices,cookedRestLengths,cookedStiffnesses);

			bendingIndices = new List<int>(cookedIndices);
			restBends = new List<float>(cookedRestLengths);
			bendingStiffnesses = new List<Vector2>(cookedStiffnesses);

			int phaseCount = Oni.GetBatchPhaseCount(batch);
			int[] phases = new int[phaseCount];
			Oni.GetBatchPhaseSizes(batch,phases);
			this.phaseSizes = new List<int>(phases);
		}

		Oni.DestroyBatch(batch);
		batch = IntPtr.Zero;
	}

	protected override void OnAddToSolver(ObiBatchedConstraints constraints){

		// Set solver constraint data:
		solverIndices = new int[bendingIndices.Count];
		for (int i = 0; i < restBends.Count; i++)
		{
			solverIndices[i*3] = constraints.Actor.particleIndices[bendingIndices[i*3]];
			solverIndices[i*3+1] = constraints.Actor.particleIndices[bendingIndices[i*3+1]];
			solverIndices[i*3+2] = constraints.Actor.particleIndices[bendingIndices[i*3+2]];
		}

	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiBendingConstraints bc = (ObiBendingConstraints) constraints;

		for (int i = 0; i < bendingStiffnesses.Count; i++){
			bendingStiffnesses[i] = new Vector2(bc.maxBending,StiffnessToCompliance(bc.stiffness));
		}

		Oni.SetBendingConstraints(batch,solverIndices,restBends.ToArray(),bendingStiffnesses.ToArray(),ConstraintCount);
		Oni.SetBatchPhaseSizes(batch,phaseSizes.ToArray(),phaseSizes.Count);
	}	

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){
	}	

}
}
