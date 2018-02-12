using UnityEngine;
using System;
using System.Collections;

namespace Obi{

	/**
	 * Small helper class that lets you specify Obi-only properties for rigidbodies.
	 */

	[ExecuteInEditMode]
	[RequireComponent(typeof(Rigidbody))]
	public class ObiRigidbody : MonoBehaviour
	{
		public bool kinematicForParticles = false;

		private IntPtr oniRigidbody = IntPtr.Zero;
		private Rigidbody unityRigidbody;
		private bool dirty = true;

		private Oni.Rigidbody adaptor = new Oni.Rigidbody();
		private Oni.RigidbodyVelocities oniVelocities = new Oni.RigidbodyVelocities();

		private Vector3 velocity, angularVelocity;

		public IntPtr OniRigidbody {
			get{return oniRigidbody;}
		}

		public void Awake(){
			unityRigidbody = GetComponent<Rigidbody>();
			oniRigidbody = Oni.CreateRigidbody();
			UpdateIfNeeded();
		}

		public void OnDestroy(){
			Oni.DestroyRigidbody(oniRigidbody);
			oniRigidbody = IntPtr.Zero;
		}

		public void UpdateIfNeeded(){

			if (dirty){

				velocity = unityRigidbody.velocity;
				angularVelocity = unityRigidbody.angularVelocity;

				adaptor.Set(unityRigidbody,kinematicForParticles);
				Oni.UpdateRigidbody(oniRigidbody,ref adaptor);

				dirty = false;

			}

		}

		/**
		 * Reads velocities back from the solver.
		 */
		public void UpdateVelocities(){

			if (!dirty){

				// kinematic rigidbodies are passed to Obi with zero velocity, so we must ignore the new velocities calculated by the solver:
				if (unityRigidbody.isKinematic || !kinematicForParticles){

					Oni.GetRigidbodyVelocity(oniRigidbody,ref oniVelocities);	
					unityRigidbody.velocity += oniVelocities.linearVelocity - velocity;
					unityRigidbody.angularVelocity += oniVelocities.angularVelocity - angularVelocity;

				}

				dirty = true;

			}

		}
	}
}

