using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

/**
 * Small helper class that allows particles to be (individually or in group) parented to a GameObject.
 */ 
[ExecuteInEditMode]
public class ObiParticleHandle : MonoBehaviour {

	[SerializeField][HideInInspector] private ObiActor actor;
	[SerializeField][HideInInspector] private List<int> handledParticleIndices = new List<int>();
	[SerializeField][HideInInspector] private List<Vector3> handledParticlePositions = new List<Vector3>();
	[SerializeField][HideInInspector] private List<float> handledParticleInvMasses = new List<float>();

	private const float HANDLED_PARTICLE_MASS = 0.0001f;

	public int ParticleCount{
		get{return handledParticleIndices.Count;}
	}

	public ObiActor Actor{
		set{
			if (actor != value)
			{
				if (actor != null && actor.Solver != null)
				{
					actor.Solver.OnFrameBegin -= Actor_solver_OnFrameBegin;
				}
				actor = value;
				if (actor != null && actor.Solver != null)
				{
					actor.Solver.OnFrameBegin += Actor_solver_OnFrameBegin;
				}
			}
		}
		get{ return actor;}
	}

	void OnEnable(){
		if (actor != null && actor.Solver != null)
		{
			actor.Solver.OnFrameBegin += Actor_solver_OnFrameBegin;
		}
	}

	void OnDisable(){
		if (actor != null && actor.Solver != null)
		{
			actor.Solver.OnFrameBegin -= Actor_solver_OnFrameBegin;
			ResetInvMasses();
		}
	}

	private void ResetInvMasses(){

		// Reset original mass of all handled particles:
		if (actor.InSolver)
		{
			float[] invMass = new float[1];
			for (int i = 0; i < handledParticleIndices.Count; ++i)
			{
				int solverParticleIndex = actor.particleIndices[handledParticleIndices[i]];
	
				invMass[0] = actor.invMasses[handledParticleIndices[i]] = handledParticleInvMasses[i];
				Oni.SetParticleInverseMasses(actor.Solver.OniSolver,invMass,1,solverParticleIndex);
			}
		}
	}

	public void Clear(){
		ResetInvMasses();
		handledParticleIndices.Clear();
		handledParticlePositions.Clear();
		handledParticleInvMasses.Clear();
	}

	public void AddParticle(int index, Vector3 position, float invMass){
		handledParticleIndices.Add(index);
		handledParticlePositions.Add(transform.InverseTransformPoint(position));
		handledParticleInvMasses.Add(invMass);
	}

	public void RemoveParticle(int index){

		int i = handledParticleIndices.IndexOf(index);

		if (i > -1){

			if (actor.InSolver){
				int solverParticleIndex = actor.particleIndices[index];
				float[] invMass = {actor.invMasses[index] = handledParticleInvMasses[i]};
				Oni.SetParticleInverseMasses(actor.Solver.OniSolver,invMass,1,solverParticleIndex);
			}
	
			handledParticleIndices.RemoveAt(i);
			handledParticlePositions.RemoveAt(i);
			handledParticleInvMasses.RemoveAt(i);

		}
	}

	void Actor_solver_OnFrameBegin (object sender, System.EventArgs e)
	{
		if (actor.InSolver){

			Vector4[] pos = new Vector4[1];
			Vector4[] vel = new Vector4[]{-actor.Solver.parameters.gravity * Time.fixedDeltaTime};
			float[] invMass = new float[]{HANDLED_PARTICLE_MASS};

			Matrix4x4 l2sTransform;
			if (actor.Solver.simulateInLocalSpace)
				l2sTransform = actor.Solver.transform.worldToLocalMatrix * transform.localToWorldMatrix;
			else 
				l2sTransform = transform.localToWorldMatrix;

			for (int i = 0; i < handledParticleIndices.Count; ++i){

				int solverParticleIndex = actor.particleIndices[handledParticleIndices[i]];

				// handled particles should always stay fixed:
				Oni.SetParticleVelocities(actor.Solver.OniSolver,vel,1,solverParticleIndex);
				Oni.SetParticleInverseMasses(actor.Solver.OniSolver,invMass,1,solverParticleIndex);

				// set particle position:
				pos[0] = l2sTransform.MultiplyPoint3x4(handledParticlePositions[i]);
				Oni.SetParticlePositions(actor.Solver.OniSolver,pos,1,solverParticleIndex);
				
			}

		}
	}

}
}
