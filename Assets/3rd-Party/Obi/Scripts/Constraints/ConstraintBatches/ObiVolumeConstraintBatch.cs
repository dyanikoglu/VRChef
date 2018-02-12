using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds data about volume constraints for an actor.
 */
[Serializable]
public class ObiVolumeConstraintBatch : ObiConstraintBatch
{

	[HideInInspector] public List<int> triangleIndices = new List<int>();			/**< Triangle indices.*/
	[HideInInspector] public List<int> firstTriangle = new List<int>();				/**< index of first triangle for each constraint.*/
	[HideInInspector] public List<int> numTriangles = new List<int>();				/**< num of triangles for each constraint.*/

	[HideInInspector] public List<float> restVolumes = new List<float>();				/**< rest volume for each constraint.*/
	[HideInInspector] public List<Vector2> pressureStiffness = new List<Vector2>();		/**< pressure and stiffness for each constraint.*/

	int[] solverIndices;

	public ObiVolumeConstraintBatch(bool cooked, bool sharesParticles) : base(cooked,sharesParticles){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Volume;
	}

	public override void Clear(){
		activeConstraints.Clear();
		triangleIndices.Clear();
		firstTriangle.Clear();
		numTriangles.Clear();
		restVolumes.Clear();
		pressureStiffness.Clear();
		constraintCount = 0;	
	}

	public void AddConstraint(int[] triangles, float restVolume, float pressure, float stiffness){

		activeConstraints.Add(constraintCount);

		firstTriangle.Add((int)triangleIndices.Count/3);
		numTriangles.Add((int)triangles.Length/3);
		triangleIndices.AddRange(triangles);

		restVolumes.Add(restVolume);
		pressureStiffness.Add(new Vector2(pressure,stiffness));

		constraintCount++;

	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);
		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		triangleIndices.RemoveRange(firstTriangle[index],numTriangles[index]);
		firstTriangle.RemoveAt(index);
	    numTriangles.RemoveAt(index);
		restVolumes.RemoveAt(index);
		pressureStiffness.RemoveAt(index);
		constraintCount--;
	}

	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(4);
		
		for (int i = 0; i < ConstraintCount; i++){
			if (triangleIndices[i*3] == particleIndex || triangleIndices[i*3+1] == particleIndex || triangleIndices[i*3+2] == particleIndex) 
				constraints.Add(i);
		}
		
		return constraints;
	}

	protected override void OnAddToSolver(ObiBatchedConstraints constraints){

		// Set solver constraint data:
		solverIndices = new int[triangleIndices.Count];
		for (int i = 0; i < triangleIndices.Count; i++)
		{
			solverIndices[i] = constraints.Actor.particleIndices[triangleIndices[i]];
		}
	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiVolumeConstraints vc = (ObiVolumeConstraints) constraints;

		for (int i = 0; i < pressureStiffness.Count; i++){
			pressureStiffness[i] = new Vector2(vc.overpressure,StiffnessToCompliance(vc.stiffness));
		}

		Oni.SetVolumeConstraints(batch,solverIndices,
									   firstTriangle.ToArray(),
								       numTriangles.ToArray(),
									   restVolumes.ToArray(),
									   pressureStiffness.ToArray(),
									   ConstraintCount);
	}	

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){
	}	

}
}
