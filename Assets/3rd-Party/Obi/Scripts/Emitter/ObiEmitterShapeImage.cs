using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace Obi
{

	[ExecuteInEditMode]
	public class ObiEmitterShapeImage : ObiEmitterShape
	{
		public Texture2D image = null;
		public float pixelScale = 0.05f;
		public float maxSize = 2;

		[Range(0,1)]	
		public float maskThreshold = 0.5f;

		public override void GenerateDistribution(){

			distribution.Clear(); 

			if (image == null) return;

			switch (samplingMethod)
			{

				/*case SamplingMethod.FILL:
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
	
				}break;*/

				case SamplingMethod.LAYER:
				case SamplingMethod.SURFACE:
				{

					float width,height;
					GetWorldSpaceEmitterSize(out width,out height);
		
					int numX = Mathf.FloorToInt(width/particleSize);
					int numY = Mathf.FloorToInt(height/particleSize);
		
					for (int x = 0; x < numX; ++x){
						for (int y = 0; y < numY; ++y){
		
							Color sample = image.GetPixelBilinear(x/(float)numX,y/(float)numY);
							if (sample.a > maskThreshold){
		
								Vector3 pos = new Vector3(x*particleSize - width*0.5f ,y*particleSize - height*0.5f,0);
								Vector3 vel = Vector3.forward;
			
								distribution.Add(new ObiEmitterShape.DistributionPoint(pos,vel,sample));
							}	
						}
					}
				}break;
			}
	
		}

		private void GetWorldSpaceEmitterSize(out float width, out float height){

			width = image.width*pixelScale;
			height = image.height*pixelScale;
			float ratio = width/height;
	
			if (width > maxSize || height > maxSize){
				if (width > height){
					width = maxSize;
					height = width / ratio;
				}else{
 					height = maxSize;
					width = ratio * height;
				}
			}

		}

		public override bool SupportsAllSamplingMethods(){return true;}

	#if UNITY_EDITOR
		public void OnDrawGizmosSelected(){

			if (image == null) return;	

			Handles.matrix = transform.localToWorldMatrix;
			Handles.color  = Color.cyan;

			float width,height;
			GetWorldSpaceEmitterSize(out width,out height);

			float sx = width*0.5f;
			float sy = height*0.5f;

			Vector3[] corners = {new Vector3(-sx,-sy,0),
								 new Vector3(sx,-sy,0),
							     new Vector3(sx,sy,0),
								 new Vector3(-sx,sy,0),
								 new Vector3(-sx,-sy,0)};

			Handles.DrawPolyLine(corners);

			foreach (DistributionPoint point in distribution)
				Handles.ArrowHandleCap(0,point.position,Quaternion.LookRotation(point.velocity),0.05f,EventType.Repaint);

		}
	#endif

	}
}

