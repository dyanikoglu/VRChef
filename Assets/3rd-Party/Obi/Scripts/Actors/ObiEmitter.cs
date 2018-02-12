using UnityEngine;
using System;
using System.Collections;


namespace Obi{

	[ExecuteInEditMode]
	[AddComponentMenu("Physics/Obi/Obi Emitter")]
	public class ObiEmitter : ObiActor {


		public int fluidPhase = 1;
		[SerializeField][HideInInspector] private ObiEmitterMaterial emitterMaterial = null;
	
		[Tooltip("Amount of solver particles used by this emitter.")]
		[SerializeField][HideInInspector] private int numParticles = 1000;

		[Tooltip("Speed (in units/second) of emitted particles. Setting it to zero will stop emission. Large values will cause more particles to be emitted.")]
		public float speed = 0.25f;

		[Tooltip("Lifespan of each particle.")]
		public float lifespan = 4;

		[Range(0,1)]
		[Tooltip("Amount of randomization applied to particles.")]
		public float randomVelocity = 0;

		private ObiEmitterShape emitterShape = null;

		private int activeParticleCount = 0;			/**< number of currently active particles*/
		[HideInInspector] public float[] life;			/**< per particle remaining life in seconds.*/

		private float unemittedBursts = 0;

		public int NumParticles{
			set{
				if (numParticles != value){
					numParticles = value;
					GeneratePhysicRepresentation();
				}
			}
			get{return numParticles;}
		}

		public int ActiveParticles{
			get{return activeParticleCount;}
		}

		public override bool SelfCollisions{
			get{return selfCollisions;}
		}

		public ObiEmitterShape EmitterShape{
			get{return emitterShape;}
			set{
				if (emitterShape != value){
					emitterShape = value;
					UpdateEmitterDistribution();
				}
			}
		}

		public ObiEmitterMaterial EmitterMaterial{
			set{
				if (emitterMaterial != value){

					if (emitterMaterial != null)
					emitterMaterial.OnChangesMade -= EmitterMaterial_OnChangesMade;

					emitterMaterial = value;
				
					if (emitterMaterial != null){
						emitterMaterial.OnChangesMade += EmitterMaterial_OnChangesMade;
						EmitterMaterial_OnChangesMade(emitterMaterial,new ObiEmitterMaterial.MaterialChangeEventArgs(
																		  ObiEmitterMaterial.MaterialChanges.PER_MATERIAL_DATA |
																		  ObiEmitterMaterial.MaterialChanges.PER_PARTICLE_DATA)
													 );
					}
					
				}
			}
			get{
				return emitterMaterial;
			}
		}

		public override bool UsesCustomExternalForces{ 
			get{return true;}
		}
	
		public override void Awake()
		{
			base.Awake();
			selfCollisions = true;
			GeneratePhysicRepresentation();
		}

		public override void OnEnable(){

			if (emitterMaterial != null)
				emitterMaterial.OnChangesMade += EmitterMaterial_OnChangesMade;			

			base.OnEnable();

		}
		
		public override void OnDisable(){

			if (emitterMaterial != null)
				emitterMaterial.OnChangesMade -= EmitterMaterial_OnChangesMade;	
			
			base.OnDisable();
			
		}

		public override void DestroyRequiredComponents(){
		}

		public override bool AddToSolver(object info){
			
			if (Initialized && base.AddToSolver(info)){

				solver.RequireRenderablePositions();

				// recalculate particle masses, as the number of dimensions used to valculate particle volume depends on the solver.
				CalculateParticleMass();

				return true;
			}
			return false;
		}
		
		public override bool RemoveFromSolver(object info){

			if (solver != null)
				solver.RelinquishRenderablePositions();

			return base.RemoveFromSolver(info);

		}

		/**
		 * Sets all particle masses in accordance to the fluid's rest density.
		 */
		public void CalculateParticleMass()
		{
			float pmass = (emitterMaterial != null) ? emitterMaterial.GetParticleMass(solver.parameters.mode) : 0.1f;

			for (int i = 0; i < invMasses.Length; i++){
				invMasses[i] = 1.0f/pmass;
			}

			this.PushDataToSolver(ParticleData.INV_MASSES);
		}


		/**
		 * Sets particle solid radii to half of the fluids rest distance.
		 */
		public void SetParticleRestRadius(){
	
			if (!InSolver) return;

			// recalculate rest distance and particle mass:
			float restDistance = (emitterMaterial != null) ? emitterMaterial.GetParticleSize(solver.parameters.mode) : 0.1f ;

			for(int i = 0; i < particleIndices.Length; i++){
				solidRadii[i] = restDistance*0.5f;
			}

			PushDataToSolver(ParticleData.SOLID_RADII);
		}

		/**
	 	* Generates the particle based physical representation of the emitter. This is the initialization method for the rope object
		* and should not be called directly once the object has been created.
	 	*/
		public void GeneratePhysicRepresentation()
		{		
			initialized = false;			
			initializing = true;

			RemoveFromSolver(null);

			active = new bool[numParticles];
			life = new float[numParticles];
			positions = new Vector3[numParticles];
			velocities = new Vector3[numParticles];
			invMasses  = new float[numParticles];
			solidRadii = new float[numParticles];
			phases = new int[numParticles];
			colors = new Color[numParticles];

			float restDistance = (emitterMaterial != null) ? emitterMaterial.GetParticleSize(solver.parameters.mode) : 0.1f ;
			float pmass = (emitterMaterial != null) ? emitterMaterial.GetParticleMass(solver.parameters.mode) : 0.1f;
			
			for (int i = 0; i < numParticles; i++){

				active[i] = false;
				life[i] = 0;
				invMasses[i] = 1.0f/pmass;
				positions[i] = Vector3.zero;

				if (emitterMaterial != null && !(emitterMaterial is ObiEmitterMaterialFluid)){
					float randomRadius = UnityEngine.Random.Range(0,restDistance/100.0f * (emitterMaterial as ObiEmitterMaterialGranular).randomness);
					solidRadii[i] = Mathf.Max(0.001f + restDistance*0.5f - randomRadius);
				}else
					solidRadii[i] = restDistance*0.5f;

				colors[i] = Color.white;

				phases[i] = Oni.MakePhase(fluidPhase,(selfCollisions?Oni.ParticlePhase.SelfCollide:0) |
											    	   ((emitterMaterial != null && (emitterMaterial is ObiEmitterMaterialFluid))?Oni.ParticlePhase.Fluid:0));

			}

			initializing = false;
			initialized = true;
			
		}

		public override void UpdateParticlePhases(){
	
			if (!InSolver) return;

			Oni.ParticlePhase particlePhase = Oni.ParticlePhase.Fluid;
			if (emitterMaterial != null && !(emitterMaterial is ObiEmitterMaterialFluid))
				particlePhase = 0;
	
			for(int i = 0; i < particleIndices.Length; i++){
				phases[i] = Oni.MakePhase(fluidPhase,(selfCollisions?Oni.ParticlePhase.SelfCollide:0) | particlePhase);
			}
			PushDataToSolver(ParticleData.PHASES);
		}

		private void UpdateEmitterDistribution(){
			if (emitterShape != null){
				emitterShape.particleSize = (emitterMaterial != null) ? emitterMaterial.GetParticleSize(solver.parameters.mode) : 0.1f;
				emitterShape.GenerateDistribution();
			}
		}

		void EmitterMaterial_OnChangesMade (object sender, ObiEmitterMaterial.MaterialChangeEventArgs e)
		{
			if ((e.changes & ObiEmitterMaterial.MaterialChanges.PER_PARTICLE_DATA) != 0){
				CalculateParticleMass();
				SetParticleRestRadius();
				UpdateParticlePhases();
			}
			UpdateEmitterDistribution();
		}

		public void ResetParticlePosition(int index, float offset){	

			if (emitterShape == null){

				Vector3 spawnVelocity = Vector3.Lerp(transform.forward,UnityEngine.Random.onUnitSphere,randomVelocity);

				Vector3 positionOffset = spawnVelocity * (speed * Time.fixedDeltaTime) * offset;

				Vector4[] posArray = {transform.position + positionOffset};
				Vector4[] velArray = {spawnVelocity * speed};

				Oni.SetParticlePositions(solver.OniSolver,posArray,1,particleIndices[index]);
				Oni.SetParticleVelocities(solver.OniSolver,velArray,1,particleIndices[index]);

				colors[index] = Color.white;

			}else{

				ObiEmitterShape.DistributionPoint distributionPoint = emitterShape.GetDistributionPoint();

				Vector3 spawnVelocity = Vector3.Lerp(transform.TransformVector(distributionPoint.velocity),UnityEngine.Random.onUnitSphere,randomVelocity);

				Vector3 positionOffset = spawnVelocity * (speed * Time.fixedDeltaTime) * offset;

				Vector4[] posArray = {transform.TransformPoint(distributionPoint.position) + positionOffset};
				Vector4[] velArray = {spawnVelocity * speed};

				Oni.SetParticlePositions(solver.OniSolver,posArray,1,particleIndices[index]);
				Oni.SetParticleVelocities(solver.OniSolver,velArray,1,particleIndices[index]);

				colors[index] = distributionPoint.color;

			}

		}

		/**
		 * Asks the emiter to emits a new particle. Returns whether the emission was succesful.
		 */
		public bool EmitParticle(float offset){

			if (activeParticleCount == numParticles) return false;

			life[activeParticleCount] = lifespan;
			
			// move particle to its spawn position:
			ResetParticlePosition(activeParticleCount, offset);

			// now there's one active particle more:
			active[activeParticleCount] = true;
			activeParticleCount++;

			return true;

		}

		/**
		 * Asks the emiter to kill a particle. Returns whether it was succesful.
		 */
		public bool KillParticle(int index){

			if (activeParticleCount == 0 || index >= activeParticleCount) return false;

			// reduce amount of active particles:
			activeParticleCount--;
			active[activeParticleCount] = false; 

			// swap solver particle indices:
			int temp = particleIndices[activeParticleCount];
			particleIndices[activeParticleCount] = particleIndices[index];
			particleIndices[index] = temp;

			// also swap lifespans, so the swapped particle enjoys the rest of its life! :)
			float tempLife = life[activeParticleCount];
			life[activeParticleCount] = life[index];
			life[index] = tempLife;

			// and swap colors:
			Color tempColor = colors[activeParticleCount];
			colors[activeParticleCount] = colors[index];
			colors[index] = tempColor;

			return true;
			
		}

		public void KillAll(){

			for (int i = activeParticleCount-1; i >= 0; --i){
				KillParticle(i);
			}

			PushDataToSolver(ParticleData.ACTIVE_STATUS);
		}

		public override void OnSolverStepBegin(){

			base.OnSolverStepBegin();

			bool emitted = false;
			bool killed = false;

			// Update lifetime and kill dead particles:
			for (int i = activeParticleCount-1; i >= 0; --i){
				life[i] -= Time.deltaTime;

				if (life[i] <= 0){
					killed |= KillParticle(i);	
				}
			}

			int emissionPoints = emitterShape != null ? emitterShape.DistributionPointsCount : 1;

			// stream emission:
			if (emitterShape == null || emitterShape.samplingMethod == ObiEmitterShape.SamplingMethod.SURFACE)
			{	

				// number of bursts per simulation step:
				float burstCount = (speed * Time.fixedDeltaTime) / ((emitterMaterial != null) ? emitterMaterial.GetParticleSize(solver.parameters.mode) : 0.1f);
	
				// Emit new particles:
				unemittedBursts += burstCount;
				int burst = 0;
				while (unemittedBursts > 0){
					for (int i = 0; i < emissionPoints; ++i){
						emitted |= EmitParticle(burst / burstCount);
					}
					unemittedBursts -= 1;
					burst++;
				}

			}else{ // burst emission:

				if (activeParticleCount == 0){
					for (int i = 0; i < emissionPoints; ++i){
						emitted |= EmitParticle(0);
					}
				}
			}

			// Push active array to solver if any particle has been killed or emitted this frame.
			if (emitted || killed){
				PushDataToSolver(ParticleData.ACTIVE_STATUS);		
			}	

		}
	}
}
