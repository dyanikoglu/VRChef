using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Obi{

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAdvector : MonoBehaviour {

	public ObiSolver solver;

	private ParticleSystem ps;
	private ParticleSystem.Particle[] particles;

	Vector4[] positions;
	Vector4[] velocities;

	int alive;
	int solverOffset;

	public ParticleSystem Particles{
		get{return ps;}
	}

	void OnEnable(){

		if (solver != null){

			solver.OnStepBegin += Solver_OnStepBegin;
			solver.OnStepEnd += Solver_OnStepEnd;

		}
	}

	void OnDisable(){
		if (solver != null){

			solver.OnStepBegin -= Solver_OnStepBegin;
			solver.OnStepEnd -= Solver_OnStepEnd;

		}
	}

	void ReallocateParticles(){

		if (ps == null){
			ps = GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = ps.main;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
		}

		// Array to get/set particles:
		if (particles == null || particles.Length != ps.main.maxParticles){
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
			positions = new Vector4[ps.main.maxParticles];
			velocities = new Vector4[ps.main.maxParticles];
		}

	}

	void Solver_OnStepBegin (object sender, System.EventArgs e)
	{

		ReallocateParticles();

		if (solver == null) return;

		alive = ps.GetParticles(particles);

		Vector3 p;
		for (int i = 0; i < alive; ++i){
			p = particles[i].position;
			positions[i].Set(p.x,p.y,p.z,0);
		}

		solverOffset = Oni.SetDiffuseParticles(solver.OniSolver,positions,alive);

	}

	void Solver_OnStepEnd (object sender, System.EventArgs e)
	{
		if (solver == null) return;

		Oni.GetDiffuseParticleVelocities(solver.OniSolver,velocities,alive,solverOffset);

		Vector3 velocity = Vector3.zero;

		for (int i = 0; i < alive; ++i){
			velocity.Set(velocities[i].x,velocities[i].y,velocities[i].z);
			particles[i].velocity = velocity;
		}

		ps.SetParticles(particles, alive);
	}
}
}