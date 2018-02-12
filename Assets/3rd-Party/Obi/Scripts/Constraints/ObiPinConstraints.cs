using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Holds information about pin constraints for an actor.
 */
[DisallowMultipleComponent]
public class ObiPinConstraints : ObiBatchedConstraints
{
	
	[Range(0,1)]
	[Tooltip("Pin resistance to stretching. Lower values will yield more elastic pin constraints.")]
	public float stiffness = 1;		   /**< Resistance of structural spring constraints to stretch..*/
	
	[SerializeField][HideInInspector] private List<ObiPinConstraintBatch> batches = new List<ObiPinConstraintBatch>();

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Pin;
	}

	public override List<ObiConstraintBatch> GetBatches(){
		return batches.ConvertAll(x => (ObiConstraintBatch)x);
	}

	public override void Clear(){
		RemoveFromSolver(null); 
		batches.Clear();
	}

	public void AddBatch(ObiPinConstraintBatch batch){
		if (batch != null && batch.GetConstraintType() == GetConstraintType())
			batches.Add(batch);
	}

	public void RemoveBatch(ObiPinConstraintBatch batch){
		batches.Remove(batch);
	}

	public void OnDrawGizmosSelected(){

		if (!visualize) return;

		Gizmos.color = Color.cyan;

		foreach (ObiPinConstraintBatch batch in batches){
			foreach(int i in batch.ActiveConstraints){

				Vector3 pinPosition = batch.pinBodies[i].transform.TransformPoint(batch.pinOffsets[i]);

				Gizmos.DrawLine(actor.GetParticlePosition(batch.pinIndices[i]),
								pinPosition);
			}
		}

	}
}
}
