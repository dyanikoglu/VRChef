using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds information about distance constraints for an actor.
 */
[DisallowMultipleComponent]
public class ObiDistanceConstraints : ObiBatchedConstraints
{
	[Tooltip("Scale of stretching constraints. Values > 1 will expand initial cloth size, values < 1 will make it shrink.")]
	public float stretchingScale = 1;				/**< Stiffness of structural spring constraints.*/
	
	[Range(0,1)]
	[Tooltip("Cloth resistance to stretching. Lower values will yield more elastic cloth.")]
	public float stiffness = 1;		   /**< Resistance of structural spring constraints to stretch..*/
	
	[Range(0,1)]
	[Tooltip("Amount of compression slack. 0 means total resistance to compression, 1 no resistance at all. 0.5 means constraints will allow a compression of up to 50% of their rest length.")]
	public float slack = 0;		   /**< Resistance of structural spring constraints to compression.*/

	[SerializeField][HideInInspector] private List<ObiDistanceConstraintBatch> batches = new List<ObiDistanceConstraintBatch>();

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Distance;
	}

	public override List<ObiConstraintBatch> GetBatches(){
		return batches.ConvertAll(x => (ObiConstraintBatch)x);
	}

	public override void Clear(){
		RemoveFromSolver(null); 
		batches.Clear();
	}

	public void AddBatch(ObiDistanceConstraintBatch batch){
		if (batch != null && batch.GetConstraintType() == GetConstraintType())
			batches.Add(batch);
	}

	public void RemoveBatch(ObiDistanceConstraintBatch batch){
		batches.Remove(batch);
	}

	public void OnDrawGizmosSelected(){

		if (!visualize) return;

		Gizmos.color = Color.green;

		foreach (ObiDistanceConstraintBatch batch in batches){
			foreach(int i in batch.ActiveConstraints){
				Gizmos.DrawLine(actor.GetParticlePosition(batch.springIndices[i*2]),
								actor.GetParticlePosition(batch.springIndices[i*2+1]));
			}
		}

	}
}
}
