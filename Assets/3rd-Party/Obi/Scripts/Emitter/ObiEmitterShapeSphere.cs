using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace Obi
{

	[ExecuteInEditMode]
	public class ObiEmitterShapeSphere : ObiEmitterShape
	{
		public float radius = 0.5f;

		public void OnValidate(){
			radius = Mathf.Max(0,radius);
		}

		public override void GenerateDistribution(){

			distribution.Clear(); 

			if (particleSize <= 0 || radius <= 0) return;

			switch (samplingMethod)
			{
				case SamplingMethod.FILL:
				{
	
					int num = Mathf.CeilToInt(radius/particleSize);
					float norm = radius/(float)num;

					for (int x = -num; x <= num; ++x){
						for (int y = -num; y <= num; ++y){
							for (int z = -num; z <= num; ++z){
								Vector3 pos = new Vector3(x,y,z) * norm;
			
								if (pos.magnitude < radius){
									distribution.Add(new ObiEmitterShape.DistributionPoint(pos,Vector3.forward));
								}
							}
						}
					}
	
				}break;

				case SamplingMethod.SURFACE:
				case SamplingMethod.LAYER:
				{
					
					// divide the sphere's surface into several spherical caps:
					float zAxisAngleIncrement = Mathf.Asin(particleSize / (2 * radius)) * 2;
					int numCaps = (int)(Mathf.PI / zAxisAngleIncrement);

					// distribute samples in a circle around each cap's border:
					for (int i = 0; i <= numCaps; ++i){
					
						// cap radius and height:
						float r = Mathf.Sin(zAxisAngleIncrement * i) * radius;
						float h = Mathf.Cos(zAxisAngleIncrement * i) * radius;

						// len = 2 sin(angle/2) * radius
						float angleIncrement = Mathf.Asin(particleSize / (2 * r)) * 2;
						float steps = 2 * Mathf.PI / angleIncrement;

						for (int j = 0; j < steps; ++j){
							Vector3 pos = new Vector3(r * Mathf.Cos(angleIncrement*j), r * Mathf.Sin(angleIncrement*j),h);
							distribution.Add(new ObiEmitterShape.DistributionPoint(pos,pos.normalized));
						}
					}
					
				}break;
			}
		}

		public override bool SupportsAllSamplingMethods(){return true;}


	#if UNITY_EDITOR
		public void OnDrawGizmosSelected(){

			Handles.matrix = transform.localToWorldMatrix;
			Handles.color  = Color.cyan;

			Handles.DrawWireDisc(Vector3.zero,Vector3.forward,radius);
			Handles.DrawWireDisc(Vector3.zero,Vector3.up,radius);
			Handles.DrawWireDisc(Vector3.zero,Vector3.right,radius);

			foreach (DistributionPoint point in distribution)
				Handles.ArrowHandleCap(0,point.position,Quaternion.LookRotation(point.velocity),0.05f,EventType.Repaint);

		}
	#endif

	}
}

