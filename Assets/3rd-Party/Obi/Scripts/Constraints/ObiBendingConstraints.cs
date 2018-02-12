using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds information about bend constraints for an actor.
 */
[DisallowMultipleComponent]
public class ObiBendingConstraints : ObiBatchedConstraints
{
	[Tooltip("Bending offset. Leave at zero to keep the original bending amount.")]
	public float maxBending = 0;				/**< Stiffness of structural spring constraints.*/
	
	[Range(0,1)]
	[Tooltip("Cloth resistance to bending. Higher values will yield more stiff cloth.")]
	public float stiffness = 1;		   /**< Resistance of structural spring constraints to stretch..*/
	
	[SerializeField][HideInInspector] private List<ObiBendConstraintBatch> batches = new List<ObiBendConstraintBatch>();

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Bending;
	}

	public override List<ObiConstraintBatch> GetBatches(){
		return batches.ConvertAll(x => (ObiConstraintBatch)x);
	}

	public override void Clear(){
		RemoveFromSolver(null); 
		batches.Clear();
	}

	public void AddBatch(ObiBendConstraintBatch batch){
		if (batch != null && batch.GetConstraintType() == GetConstraintType())
			batches.Add(batch);
	}

	public void RemoveBatch(ObiBendConstraintBatch batch){
		batches.Remove(batch);
	}

	public void OnDrawGizmosSelected(){

		if (!visualize) return;

		Gizmos.color = new Color(0.5f,0,1,1);

		foreach (ObiBendConstraintBatch batch in batches){
			foreach(int i in batch.ActiveConstraints){
	            Gizmos.DrawLine(actor.GetParticlePosition(batch.bendingIndices[i*3]),
								actor.GetParticlePosition(batch.bendingIndices[i*3+1]));
			}
		}

	}

}
}

