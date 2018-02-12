using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
 	* Holds information about aerodynamic constraints for an actor.
 	*/
	[DisallowMultipleComponent]
	public class ObiAerodynamicConstraints : ObiBatchedConstraints
	{
		
		[Tooltip("Air density in kg/m3. Higher densities will make both drag and lift forces stronger.")]
		public float airDensity = 1.225f;
		
		[Tooltip("How much is the cloth affected by drag forces. Extreme values can cause the cloth to behave unrealistically, so use with care.")]
		public float dragCoefficient = 0.05f;
		
		[Tooltip("How much is the cloth affected by lift forces. Extreme values can cause the cloth to behave unrealistically, so use with care.")]
		public float liftCoefficient = 0.05f;
		
		[SerializeField][HideInInspector] private List<ObiAerodynamicConstraintBatch> batches = new List<ObiAerodynamicConstraintBatch>();

		public override Oni.ConstraintType GetConstraintType(){
			return Oni.ConstraintType.Aerodynamics;
		}
	
		public override List<ObiConstraintBatch> GetBatches(){
			return batches.ConvertAll(x => (ObiConstraintBatch)x);
		}
	
		public void OnValidate(){
			airDensity = Mathf.Max(0,airDensity);
			dragCoefficient = Mathf.Max(0,dragCoefficient);
			liftCoefficient = Mathf.Max(0,liftCoefficient);
		}

		public override void Clear(){
			RemoveFromSolver(null); 
			batches.Clear();
		}
	
		public void AddBatch(ObiAerodynamicConstraintBatch batch){
			if (batch != null && batch.GetConstraintType() == GetConstraintType())
				batches.Add(batch);
		}
	
		public void RemoveBatch(ObiAerodynamicConstraintBatch batch){
			batches.Remove(batch);
		}

		public void OnDrawGizmosSelected(){
		
			if (!visualize) return;
	
			Gizmos.color = Color.blue;
	
			foreach (ObiAerodynamicConstraintBatch batch in batches){
				foreach(int i in batch.ActiveConstraints){
		            Gizmos.DrawWireSphere(actor.GetParticlePosition(batch.aerodynamicIndices[i]),0.01f);
				}
			}
		
		}
	
	}
}




