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
public class ObiSkinConstraintBatch : ObiConstraintBatch
{

	[HideInInspector] public List<int> skinIndices = new List<int>();						/**< Distance constraint indices.*/
	[HideInInspector] public List<Vector4> skinPoints = new List<Vector4>();				/**< Skin constraint anchor points, in world space.*/
	[HideInInspector] public List<Vector4> skinNormals = new List<Vector4>();				/**< Rest distances.*/
	[HideInInspector] public List<float> skinRadiiBackstop = new List<float>();				/**< Rest distances.*/
	[HideInInspector] public List<float> skinStiffnesses = new List<float>();				/**< Stiffnesses of distance constraits.*/

	int[] solverIndices = new int[0];

	public ObiSkinConstraintBatch(bool cooked, bool sharesParticles) : base(cooked,sharesParticles){
	}

	public ObiSkinConstraintBatch(bool cooked, bool sharesParticles, float minYoungModulus, float maxYoungModulus) : 
	base(cooked,sharesParticles,minYoungModulus,maxYoungModulus){
	}

	public override Oni.ConstraintType GetConstraintType(){
		return Oni.ConstraintType.Skin;
	}

	public override void Clear(){
		activeConstraints.Clear();
		skinIndices.Clear();
		skinPoints.Clear();
		skinNormals.Clear();	
		skinRadiiBackstop.Clear();
		skinStiffnesses.Clear();
		constraintCount = 0;	
	}

	public void AddConstraint(int index, Vector4 point, Vector4 normal, float radius, float collisionRadius, float backstop, float stiffness){
		activeConstraints.Add(constraintCount);
		skinIndices.Add(index);
		skinPoints.Add(point);
		skinNormals.Add(normal);
		skinRadiiBackstop.Add(radius);
		skinRadiiBackstop.Add(collisionRadius);
		skinRadiiBackstop.Add(backstop);
		skinStiffnesses.Add(stiffness);
		constraintCount++;
	}

	public void RemoveConstraint(int index){

		if (index < 0 || index >= ConstraintCount)
			return;

		activeConstraints.Remove(index);
		for(int i = 0; i < activeConstraints.Count; ++i)
		    if (activeConstraints[i] > index) activeConstraints[i]--;

		skinIndices.RemoveAt(index);
		skinPoints.RemoveAt(index);
		skinNormals.RemoveAt(index);
		skinStiffnesses.RemoveAt(index);
		skinRadiiBackstop.RemoveRange(index*3,3);
		constraintCount--;
	}
	
	public override List<int> GetConstraintsInvolvingParticle(int particleIndex){
	
		List<int> constraints = new List<int>(1);
		
		for (int i = 0; i < ConstraintCount; i++){
			if (skinIndices[i] == particleIndex) 
				constraints.Add(i);
		}
		
		return constraints;
	}

	public override void Cook()
	{
		batch = Oni.CreateBatch((int)Oni.ConstraintType.Skin,true);

		// Send initial data to batch:
		Oni.SetSkinConstraints(batch,skinIndices.ToArray(),skinPoints.ToArray(),skinNormals.ToArray(),skinRadiiBackstop.ToArray(),skinStiffnesses.ToArray(),ConstraintCount);

		// cook the batch and retrieve new sorted data:
		if (Oni.CookBatch(batch))
		{
			constraintCount = Oni.GetBatchConstraintCount(batch);
			activeConstraints = Enumerable.Range(0, constraintCount).ToList();

			int[] cookedIndices = new int[constraintCount]; 
			Vector4[] cookedPoints = new Vector4[constraintCount]; 
			Vector4[] cookedNormals = new Vector4[constraintCount]; 
			float[] cookedRadiiBackstop = new float[constraintCount*3]; 
			float[] cookedStiffnesses = new float[constraintCount]; 

			Oni.GetSkinConstraints(batch,cookedIndices,cookedPoints,cookedNormals,cookedRadiiBackstop,cookedStiffnesses);

			skinIndices = new List<int>(cookedIndices);
			skinPoints = new List<Vector4>(cookedPoints);
			skinNormals = new List<Vector4>(cookedNormals);
			skinRadiiBackstop = new List<float>(cookedRadiiBackstop);
			skinStiffnesses = new List<float>(cookedStiffnesses);

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
		solverIndices = new int[skinIndices.Count];
		for (int i = 0; i < skinIndices.Count; i++)
		{
			solverIndices[i] = constraints.Actor.particleIndices[skinIndices[i]];
			solverIndices[i] = constraints.Actor.particleIndices[skinIndices[i]];
		}

	}

	protected override void OnRemoveFromSolver(ObiBatchedConstraints constraints){
	}

	public override void PushDataToSolver(ObiBatchedConstraints constraints){ 

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		ObiSkinConstraints sc = (ObiSkinConstraints) constraints;

		float[] stiffnesses = new float[skinStiffnesses.Count];

		for (int i = 0; i < stiffnesses.Length; i++){
			stiffnesses[i] = StiffnessToCompliance(skinStiffnesses[i] * sc.stiffness);
		}

		Oni.SetSkinConstraints(batch,solverIndices,
									 skinPoints.ToArray(),
								     skinNormals.ToArray(),
									 skinRadiiBackstop.ToArray(),
								     stiffnesses,
									 ConstraintCount);
		Oni.SetBatchPhaseSizes(batch,phaseSizes.ToArray(),phaseSizes.Count);
	}

	public override void PullDataFromSolver(ObiBatchedConstraints constraints){

		if (constraints == null || constraints.Actor == null || !constraints.Actor.InSolver)
			return;

		int[] cookedIndices = new int[constraintCount]; 
		Vector4[] cookedPoints = new Vector4[constraintCount]; 
		Vector4[] cookedNormals = new Vector4[constraintCount]; 
		float[] cookedRadiiBackstop = new float[constraintCount*3]; 
		float[] cookedStiffnesses = new float[constraintCount]; 

		Oni.GetSkinConstraints(batch,cookedIndices,cookedPoints,cookedNormals,cookedRadiiBackstop,cookedStiffnesses);

		skinPoints = new List<Vector4>(cookedPoints);
		skinNormals = new List<Vector4>(cookedNormals);
		// dont retrieve radii and stiffness, since the solver never modifies them.
	}	

	/**
	 * Returns the position of a skin constraint in world space. 
	 */
	public Vector3 GetSkinPosition(int index){
		return skinPoints[index];
	}

	/**
	 * Returns the normal of a skin constraint in world space. 
	 */
	public Vector3 GetSkinNormal(int index){
		return skinNormals[index];
	}
		

}
}
