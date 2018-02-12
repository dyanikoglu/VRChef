using UnityEngine;
using System;
using System.Collections.Generic;


namespace Obi{

	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public abstract class ObiEmitterShape : MonoBehaviour
	{

		public enum SamplingMethod{
			SURFACE,		/**< distributes particles in the surface of the object. Stream emission.*/
			LAYER,			/**< distributes particles in the surface of the object. Burst emission.*/
			FILL			/**< distributes particles in the surface of the object. Burst emission.*/
		}

		public struct DistributionPoint{
			public Vector3 position;
			public Vector3 velocity;
			public Color color;

			public DistributionPoint(Vector3 position, Vector3 velocity){
				this.position = position;
				this.velocity = velocity;
				this.color = Color.white;
			}

			public DistributionPoint(Vector3 position, Vector3 velocity, Color color){
				this.position = position;
				this.velocity = velocity;
				this.color = color;
			}
		}

		public SamplingMethod samplingMethod = SamplingMethod.SURFACE;
		[HideInInspector] public float particleSize = 0;

		protected List<DistributionPoint> distribution = new List<DistributionPoint>();
		protected int lastDistributionPoint = 0;

		public int DistributionPointsCount{
			get{return distribution.Count;}
		}

		public void OnEnable(){
			ObiEmitter emitter = GetComponent<ObiEmitter>();
			if (emitter != null)
				emitter.EmitterShape = this;
		}

		public void OnDisable(){
			ObiEmitter emitter = GetComponent<ObiEmitter>();
			if (emitter != null)
				emitter.EmitterShape = null;
		}

		public abstract void GenerateDistribution();

		public abstract bool SupportsAllSamplingMethods();

		public DistributionPoint GetDistributionPoint(){

			if (lastDistributionPoint >= distribution.Count)
				return new DistributionPoint();

			DistributionPoint point = distribution[lastDistributionPoint];
			lastDistributionPoint = (lastDistributionPoint + 1) % distribution.Count;

			return point;
			
		}
		
	}
}

