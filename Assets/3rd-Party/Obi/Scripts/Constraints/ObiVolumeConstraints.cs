using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
 	 * Holds information about volume constraints for an actor.
 	 */
	[DisallowMultipleComponent]
	public class ObiVolumeConstraints : ObiBatchedConstraints 
	{
		
		[Tooltip("Amount of pressure applied to the cloth.")]
		public float overpressure = 1;

		[Range(0,1)]
		[Tooltip("Stiffness of the volume constraints. Higher values will make the constraints to try harder to enforce the set volume.")]
		public float stiffness = 1;
		
		[SerializeField][HideInInspector] private List<ObiVolumeConstraintBatch> batches = new List<ObiVolumeConstraintBatch>();

		public override Oni.ConstraintType GetConstraintType(){
			return Oni.ConstraintType.Volume;
		}
	
		public override List<ObiConstraintBatch> GetBatches(){
			return batches.ConvertAll(x => (ObiConstraintBatch)x);
		}
	
		public override void Clear(){
			RemoveFromSolver(null); 
			batches.Clear();
		}
	
		public void AddBatch(ObiVolumeConstraintBatch batch){
			if (batch != null && batch.GetConstraintType() == GetConstraintType())
				batches.Add(batch);
		}
	
		public void RemoveBatch(ObiVolumeConstraintBatch batch){
			batches.Remove(batch);
		}

		public void OnDrawGizmosSelected(){
		
			if (!visualize) return;
	
			Gizmos.color = Color.red;
	
			foreach (ObiVolumeConstraintBatch batch in batches){
				foreach(int i in batch.ActiveConstraints){
					int first = batch.firstTriangle[i];
					for(int j = 0; j < batch.numTriangles[i]; ++j){

						int triangle = first + j;

						Vector3 p1 = actor.GetParticlePosition(batch.triangleIndices[triangle*3]);
						Vector3 p2 = actor.GetParticlePosition(batch.triangleIndices[triangle*3+1]);
						Vector3 p3 = actor.GetParticlePosition(batch.triangleIndices[triangle*3+2]);
	
						Gizmos.DrawLine(p1,p2);
						Gizmos.DrawLine(p1,p3);
						Gizmos.DrawLine(p2,p3);
					}
				}
			}
		
		}
		
	}
}





