using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi
{
	
	/**
	 * Drives a bone hierarchy using Obi particles.
	 */
	[ExecuteInEditMode]
	[AddComponentMenu("Physics/Obi/Obi Bone")]
	[RequireComponent(typeof (ObiDistanceConstraints))]
	[RequireComponent(typeof (ObiBendingConstraints))]
	[RequireComponent(typeof (ObiSkinConstraints))]
	public class ObiBone : ObiActor
	{
		public const float DEFAULT_PARTICLE_MASS = 0.1f;
		public const float MAX_YOUNG_MODULUS = 20.0f; //that of wood (N/m2);
		public const float MIN_YOUNG_MODULUS = 0.0001f; //that of polymer foam (N/m2);

		[Tooltip("Initial particle radius.")]
		public float particleRadius = 0.05f;				/**< Thickness of the rope.*/

		[HideInInspector][SerializeField] List<Transform> bones;	/**< flattened bone hierarchy (nth bone corresponds to nth particle).*/
		[HideInInspector][SerializeField] int[] parentIndices;	/**< parent particle index for each particle.*/
		[HideInInspector]public bool[] frozen;		/**< whether the associated bone is frozen or not.*/

		protected ObiAnimatorController animatorController;

		public ObiSkinConstraints SkinConstraints{
			get{return constraints[Oni.ConstraintType.Skin] as ObiSkinConstraints;}
		}
		public ObiDistanceConstraints DistanceConstraints{
			get{return constraints[Oni.ConstraintType.Distance] as ObiDistanceConstraints;}
		}
		public ObiBendingConstraints BendingConstraints{
			get{return constraints[Oni.ConstraintType.Bending] as ObiBendingConstraints;}
		}

		public override void Awake()
		{
			base.Awake();
			SetupAnimatorController();
		}
	     
		public void OnValidate(){
			particleRadius = Mathf.Max(0,particleRadius);
	    }

		public override void OnSolverFrameEnd(){
			
			base.OnSolverFrameEnd();

			UpdateBones();
			
		}
		
		public override bool AddToSolver(object info){
			
			if (Initialized && base.AddToSolver(info)){

				solver.RequireRenderablePositions();

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
	 	 * Find the first animator up the hierarchy of the cloth, and get its ObiAnimatorController component or add one if it is not present.
	     */
		private void SetupAnimatorController(){
	
			// find the first animator up our hierarchy:
			Animator animator = GetComponentInParent<Animator>();
				
			// if we have an animator above us, see if it has an animator controller component and add one if it doesn't:
			if (animator != null){

				animatorController = animator.GetComponent<ObiAnimatorController>();

				if (animatorController == null)
					animatorController = animator.gameObject.AddComponent<ObiAnimatorController>();
			}
		}

		/**
		 * Use this to iterate the bone hierarchy in a breadth-first fashion.
		 */
		private System.Collections.IEnumerable EnumerateBonesBreadthFirst(){

			int count = 0;

			Queue<Transform> queue = new Queue<Transform>();
			queue.Enqueue(transform);
			Transform current;

			while (queue.Count > 0){

				current = queue.Dequeue();

				if (current != null){
	
					count++;

					yield return current;
			
					// enqueue children for processing:
					foreach(Transform child in current.transform){
						queue.Enqueue(child);
					}

					// found a loop, stop enumerating:
					if (current == transform && count > 1){
						yield return null;
					}	
				}
			}

		}
		
		/**
	 	* Generates the particle based physical representation of the bone hierarcht. This is the initialization method for the actor
		* and should not be called directly once the object has been created.
	 	*/
		public IEnumerator GeneratePhysicRepresentationForBones()
		{		
			initialized = false;			
			initializing = true;	

			RemoveFromSolver(null);

			// get a list of bones in preorder:
			bones = new List<Transform>();
			foreach(Transform bone in EnumerateBonesBreadthFirst())
				bones.Add(bone);

			parentIndices = new int[bones.Count];

			active = new bool[bones.Count];
			positions = new Vector3[bones.Count];
			velocities = new Vector3[bones.Count];
			invMasses  = new float[bones.Count];
			solidRadii = new float[bones.Count];
			phases = new int[bones.Count];
			restPositions = new Vector4[bones.Count];
			frozen = new bool[bones.Count];
			
			DistanceConstraints.Clear();
			ObiDistanceConstraintBatch distanceBatch = new ObiDistanceConstraintBatch(false,false,MIN_YOUNG_MODULUS,MAX_YOUNG_MODULUS);
			DistanceConstraints.AddBatch(distanceBatch);

			BendingConstraints.Clear();
			ObiBendConstraintBatch bendingBatch = new ObiBendConstraintBatch(false,false,MIN_YOUNG_MODULUS,MAX_YOUNG_MODULUS);
			BendingConstraints.AddBatch(bendingBatch);

			SkinConstraints.Clear();
			ObiSkinConstraintBatch skinBatch = new ObiSkinConstraintBatch(true,false,MIN_YOUNG_MODULUS,MAX_YOUNG_MODULUS);
			SkinConstraints.AddBatch(skinBatch);

			for(int i = 0; i < bones.Count; ++i){

				active[i] = true;
				invMasses[i] = 1.0f/DEFAULT_PARTICLE_MASS;
				positions[i] = transform.InverseTransformPoint(bones[i].position);
				restPositions[i] = positions[i];
				restPositions[i][3] = 0;
				solidRadii[i] = particleRadius;
				frozen[i] = false;
				phases[i] = Oni.MakePhase(1,selfCollisions?Oni.ParticlePhase.SelfCollide:0);

				parentIndices[i] = -1;
				if (bones[i].parent != null)
					parentIndices[i] = bones.IndexOf(bones[i].parent);

				skinBatch.AddConstraint(i,positions[i],Vector3.up,0.05f,0,0,1);

				foreach (Transform child in bones[i]){

					int childIndex = bones.IndexOf(child);
					if (childIndex >= 0){

						// add distance constraint between the bone and its child.
						distanceBatch.AddConstraint(i,childIndex,Vector3.Distance(bones[i].position,child.position),1,1);
	
						if (parentIndices[i] >= 0){

							Transform parent = bones[parentIndices[i]];
			
							float[] restPositions = new float[]{parent.position[0],parent.position[1],parent.position[2],
																child.position[0],child.position[1],child.position[2],
																bones[i].position[0],bones[i].position[1],bones[i].position[2]};
							float restBend = Oni.BendingConstraintRest(restPositions);
		
							// add bend constraint between the bone, its parent and its child.
							bendingBatch.AddConstraint(parentIndices[i],childIndex,i,restBend,0,0);
						}
						
					}
				}	

				if (i % 10 == 0)
					yield return new CoroutineJob.ProgressInfo("ObiBone: generating particles...",i/(float)bones.Count);

			}

			skinBatch.Cook();

			initializing = false;
			initialized = true;
		}

		public override void OnSolverStepBegin(){
	
			// apparently checking whether the actor is enabled or not doesn't take a despreciable amount of time.
			bool actorEnabled = this.enabled;

			// manually update animator (before particle physics):

			// TODO: update root bone here if animator is deactiavted.
			if (animatorController != null)
				animatorController.UpdateAnimation();
	
			Vector4[] simulationPosition = {Vector4.zero};
	
			// build local to simulation space transform:
			Matrix4x4 l2sTransform;
			if (Solver.simulateInLocalSpace)
				l2sTransform = Solver.transform.worldToLocalMatrix * ActorLocalToWorldMatrix;
			else 
				l2sTransform = ActorLocalToWorldMatrix;
	
			Matrix4x4 delta = Solver.transform.worldToLocalMatrix * Solver.LastTransform;

			ObiSkinConstraintBatch skinConstraints = (ObiSkinConstraintBatch)SkinConstraints.GetBatches()[0];

			// transform fixed particles:
			for(int i = 0; i < particleIndices.Length; i++){

				Vector3 solverSpaceBone = l2sTransform.MultiplyPoint3x4(transform.InverseTransformPoint(bones[i].position));
	
				if (!actorEnabled || invMasses[i] == 0){
	
					simulationPosition[0] = solverSpaceBone;
					Oni.SetParticlePositions(solver.OniSolver,simulationPosition,1,particleIndices[i]);
	
				}else if (Solver.simulateInLocalSpace){
	
					Oni.GetParticlePositions(solver.OniSolver,simulationPosition,1,particleIndices[i]);
					simulationPosition[0] = Vector3.Lerp(simulationPosition[0],delta.MultiplyPoint3x4(simulationPosition[0]),worldVelocityScale);
					Oni.SetParticlePositions(solver.OniSolver,simulationPosition,1,particleIndices[i]);
	
				}

				skinConstraints.skinPoints[i] = solverSpaceBone;

			}

			skinConstraints.PushDataToSolver(SkinConstraints);
		}

		public override void OnSolverStepEnd(){	

			base.OnSolverStepEnd();

			if (animatorController != null)
				animatorController.ResetUpdateFlag();

		}

		/**
		 * Updates bone positions.
		 */
		public void UpdateBones(){

			for(int i = 0; i < bones.Count; ++i){

				if (frozen[i])
					continue;

				Vector3 position =  GetParticlePosition(i);

				//update parent rotation:
				if (parentIndices[i] >= 0 && !frozen[parentIndices[i]]){

					Transform parent = bones[parentIndices[i]];

					if (parent.childCount <= 1){

						Vector3 source = parent.TransformDirection(bones[i].localPosition);
						Vector3 target = position - GetParticlePosition(parentIndices[i]);
						
		                parent.rotation = Quaternion.FromToRotation(source, target) * parent.rotation;
					}			

				}

				// update current bone position:
				bones[i].position = position;

			}
			
		}
		
		/**
 		* Resets bones to their original state.
 		*/
		public override void ResetActor(){
	
			PushDataToSolver(ParticleData.POSITIONS | ParticleData.VELOCITIES);
			
			if (particleIndices != null){
				for(int i = 0; i < particleIndices.Length; ++i){
					solver.renderablePositions[particleIndices[i]] = positions[i];
					bones[i].position = transform.TransformPoint(positions[i]);
				}
			}

		}
		
	}
}



