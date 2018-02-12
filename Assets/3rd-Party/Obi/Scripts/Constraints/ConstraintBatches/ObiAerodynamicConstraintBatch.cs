using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds information about distance constraints for an actor.
 */
[Serializable]
public class ObiAerodynamicConstraintBatch : ObiConstraintBatch
{

	[HideInInspector] public List<int> aerodynamicIndices = new List<int>();	 			/**< particle indices, 1 per constraint.*/
	[HideInInspector] public List<float> aerodynamicCoeffs = new List<float>();				/**< Per-particle aerodynamic coeffs, 3 per constraint.*/

	int[] solverIndices = new int[0];

	public ObiAerodynamicConstraintBatch(bool cooked, bool sharesParticles) : base(cooked,sharesParticles){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Aerodynamics;
	}

	public override void Clear(){
		activeConstraints.Clear();
		aerodynamicIndices.Clear();
		aerodynamicCoeffs.Clear();
		constraintCount = 0;	
	}

	public void AddConstraint(int index, float area, float drag, float lift){
		activeConstraints.Add(constraintCount);
		aerodynamicIndices.Add(index);
		aerodynamicCoeffs.Add(area);
		aerodynamicCoeffs.Add(drag);
		aerodynamicCoeffs.Add(lift);
		constraintCount++;
	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);
		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		aerodynamicIndices.RemoveAt(index);
		aerodynamicCoeffs.RemoveRange(index*3,3);
		constraintCount--;
	}
	
	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(1);

		for (int i = 0; i < ConstraintCount; i++){
			if (aerodynamicIndices[i] == particleIndex) 
				constraints.Add(i);
		}
		
		return constraints;
	}

	protected override void OnAddToSolver(ObiBatchedConstraints constraints){

		// Set solver constraint data:
		solverIndices = new int[aerodynamicIndices.Count];
		for (int i = 0; i < aerodynamicIndices.Count; i++)
		{
			solverIndices[i] = constraints.Actor.particleIndices[aerodynamicIndices[i]];
		}

	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiAerodynamicConstraints dc = (ObiAerodynamicConstraints) constraints;
		
		for (int i = 0; i < aerodynamicCoeffs.Count; i+=3){
			aerodynamicCoeffs[i+1] = dc.dragCoefficient * dc.airDensity;
			aerodynamicCoeffs[i+2] = dc.liftCoefficient * dc.airDensity;
		}

		Oni.SetAerodynamicConstraints(batch,solverIndices,aerodynamicCoeffs.ToArray(),ConstraintCount);
	}

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){
	}	

}
}
