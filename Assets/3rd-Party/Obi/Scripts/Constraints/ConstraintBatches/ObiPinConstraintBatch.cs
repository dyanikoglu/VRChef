using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds information about distance constraints for an actor.
 */
[Serializable]
public class ObiPinConstraintBatch : ObiConstraintBatch
{

	[HideInInspector] public List<int> pinIndices = new List<int>();					/**< Pin constraint indices.*/
	[HideInInspector] public List<ObiCollider> pinBodies = new List<ObiCollider>();			/**< Pin bodies.*/
	[HideInInspector] public List<Vector4> pinOffsets = new List<Vector4>();			/**< Offset expressed in the attachment's local space.*/
	[HideInInspector] public List<float> stiffnesses = new List<float>();				/**< Stiffnesses of pin constraits.*/

	[HideInInspector] public List<float> pinBreakResistance = new List<float>(); 	/**< Per-constraint tear resistances.*/

	int[] solverIndices = new int[0];
	IntPtr[] solverColliders = new IntPtr[0];

	public ObiPinConstraintBatch(bool cooked, bool sharesParticles) : base(cooked,sharesParticles){
	}

	public ObiPinConstraintBatch(bool cooked, bool sharesParticles, float minYoungModulus, float maxYoungModulus) : 
	base(cooked,sharesParticles,minYoungModulus,maxYoungModulus){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Pin;
	}

	public override void Clear(){
		activeConstraints.Clear();
		pinIndices.Clear();
		pinBodies.Clear();
		pinOffsets.Clear();
		stiffnesses.Clear();
		pinBreakResistance.Clear();
		constraintCount = 0;	
	}

	public void AddConstraint(int index1, ObiCollider body, Vector3 offset, float stiffness){
		activeConstraints.Add(constraintCount);
		pinIndices.Add(index1);
		pinBodies.Add(body);
		pinOffsets.Add(offset);
        stiffnesses.Add(stiffness);
		pinBreakResistance.Add(float.MaxValue);
		constraintCount++;
	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);

		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		pinIndices.RemoveAt(index);
		pinBodies.RemoveAt(index);
        pinOffsets.RemoveAt(index);
		stiffnesses.RemoveAt(index);
		pinBreakResistance.RemoveAt(index);
		constraintCount--;
	}
	
	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(5);
		
		for (int i = 0; i < ConstraintCount; i++){
			if (pinIndices[i] == particleIndex)
				constraints.Add(i);
		}
		
		return constraints;
	}

	protected override void OnAddToSolver(ObiBatchedConstraints constraints){

		// Set solver constraint data:
		solverIndices = new int[pinIndices.Count];
 		solverColliders = new IntPtr[pinIndices.Count];
		for (int i = 0; i < pinOffsets.Count; i++)
		{
			solverIndices[i] = constraints.Actor.particleIndices[pinIndices[i]];
			solverColliders[i] = pinBodies[i] != null ? pinBodies[i].OniCollider : IntPtr.Zero;
		}

	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiPinConstraints pc = (ObiPinConstraints) constraints;

		for (int i = 0; i < stiffnesses.Count; i++){
			stiffnesses[i] = StiffnessToCompliance(pc.stiffness);
		}

		Oni.SetPinConstraints(batch,solverIndices,pinOffsets.ToArray(),solverColliders,stiffnesses.ToArray(),ConstraintCount);

	}

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){
	}	

	public void BreakConstraints(){

		float[] forces = new float[ConstraintCount];
		Oni.GetBatchConstraintForces(batch,forces,ConstraintCount,0);

		for (int i = 0; i < forces.Length; i++){
			if (-forces[i] * 1000 > pinBreakResistance[i]){ // units are kilonewtons.
				activeConstraints.Remove(i);
			}
		}

		SetActiveConstraints();
	}

}
}
