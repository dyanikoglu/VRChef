using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
 	* Holds information about skin constraints for an actor.
 	*/
	[DisallowMultipleComponent]
	public class ObiSkinConstraints : ObiBatchedConstraints 
	{
		
		[Range(0,1)]
		[Tooltip("Skin constraints stiffness.")]
		public float stiffness = 1;		   /**< Resistance of structural spring constraints to stretch..*/
		
		[SerializeField][HideInInspector] private List<ObiSkinConstraintBatch> batches = new List<ObiSkinConstraintBatch>();

		public override Oni.ConstraintType GetConstraintType(){
			return Oni.ConstraintType.Skin;
		}
	
		public override List<ObiConstraintBatch> GetBatches(){
			return batches.ConvertAll(x => (ObiConstraintBatch)x);
		}
	
		public override void Clear(){
			RemoveFromSolver(null); 
			batches.Clear();
		}
	
		public void AddBatch(ObiSkinConstraintBatch batch){
			if (batch != null && batch.GetConstraintType() == GetConstraintType())
				batches.Add(batch);
		}
	
		public void RemoveBatch(ObiSkinConstraintBatch batch){
			batches.Remove(batch);
		}

		public void OnDrawGizmosSelected(){
		
			if (!visualize) return;
	
			Gizmos.color = Color.magenta;

			Matrix4x4 s2wTransform = actor.Solver.transform.localToWorldMatrix;
	
			foreach (ObiSkinConstraintBatch batch in batches){
				batch.PullDataFromSolver(this);
				foreach(int i in batch.ActiveConstraints){

					Vector3 point = batch.GetSkinPosition(i);

					if (!InSolver){
						point = transform.TransformPoint(point);
					}else if (actor.Solver.simulateInLocalSpace){
						point = s2wTransform.MultiplyPoint3x4(point);
					}

					if (actor.invMasses[batch.skinIndices[i]] > 0)
		            	Gizmos.DrawLine(point,actor.GetParticlePosition(batch.skinIndices[i]));
				}
			}
		
		}

	}
}


