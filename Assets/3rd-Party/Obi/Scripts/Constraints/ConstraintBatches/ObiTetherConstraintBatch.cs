using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Obi{

/**
 * Holds information about distance constraints for an actor.
 */
[Serializable]
public class ObiTetherConstraintBatch : ObiConstraintBatch
{

	[HideInInspector] public List<int> tetherIndices = new List<int>();					/**< Tether constraint indices.*/
	[HideInInspector] public List<Vector2> maxLengthsScales = new List<Vector2>();				/**< Max distance and scale for each tether.*/
	[HideInInspector] public List<float> stiffnesses = new List<float>();				/**< Stiffnesses of distance constraits.*/
		

	int[] solverIndices = new int[0];

	public ObiTetherConstraintBatch(bool cooked, bool sharesParticles) : base(cooked,sharesParticles){
	}

	public ObiTetherConstraintBatch(bool cooked, bool sharesParticles, float minYoungModulus, float maxYoungModulus) : 
	base(cooked,sharesParticles,minYoungModulus,maxYoungModulus){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Tether;
	}

	public override void Clear(){
		activeConstraints.Clear();
		tetherIndices.Clear();
		maxLengthsScales.Clear();
		stiffnesses.Clear();	
		constraintCount = 0;	
	}

	public void AddConstraint(int index1, int index2, float maxLength, float scale, float stiffness){
		activeConstraints.Add(constraintCount);
		tetherIndices.Add(index1);
		tetherIndices.Add(index2);
		maxLengthsScales.Add(new Vector2(maxLength,scale));
		stiffnesses.Add(stiffness);
		constraintCount++;
	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);
		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		tetherIndices.RemoveRange(index*2,2);
		maxLengthsScales.RemoveAt(index);
        stiffnesses.RemoveAt(index);
		constraintCount--;
	}
	
	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(4);
		
		for (int i = 0; i < ConstraintCount; i++){
			if (tetherIndices[i*2] == particleIndex || tetherIndices[i*2+1] == particleIndex) 
				constraints.Add(i);
		}
		
		return constraints;
	}

	public override void Cook()
	{
		batch = Oni.CreateBatch((int)Oni.ConstraintType.Tether,true);

		// Send initial data to batch:
		Oni.SetTetherConstraints(batch,tetherIndices.ToArray(),maxLengthsScales.ToArray(),stiffnesses.ToArray(),ConstraintCount);

		// cook the batch and retrieve new sorted data:
		if (Oni.CookBatch(batch))
		{
			constraintCount = Oni.GetBatchConstraintCount(batch);
			activeConstraints = Enumerable.Range(0, constraintCount).ToList();

			int[] cookedIndices = new int[constraintCount*2]; 
			Vector2[] cookedRestLengths = new Vector2[constraintCount]; 
			float[] cookedStiffnesses = new float[constraintCount]; 

			Oni.GetTetherConstraints(batch,cookedIndices,cookedRestLengths,cookedStiffnesses);

			tetherIndices = new List<int>(cookedIndices);
			maxLengthsScales = new List<Vector2>(cookedRestLengths);
			stiffnesses = new List<float>(cookedStiffnesses);

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
		solverIndices = new int[tetherIndices.Count];
		for (int i = 0; i < ConstraintCount; i++)
		{
			solverIndices[i*2] = constraints.Actor.particleIndices[tetherIndices[i*2]];
			solverIndices[i*2+1] = constraints.Actor.particleIndices[tetherIndices[i*2+1]];
		}

	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiTetherConstraints tc = (ObiTetherConstraints) constraints;

		for (int i = 0; i < ConstraintCount; i++){
			maxLengthsScales[i] = new Vector2(maxLengthsScales[i].x, tc.tetherScale);
			stiffnesses[i] = StiffnessToCompliance(tc.stiffness);
		} 

		Oni.SetTetherConstraints(batch,solverIndices,maxLengthsScales.ToArray(),stiffnesses.ToArray(),ConstraintCount);
		Oni.SetBatchPhaseSizes(batch,phaseSizes.ToArray(),phaseSizes.Count);
	}

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){
	}	

}
}
