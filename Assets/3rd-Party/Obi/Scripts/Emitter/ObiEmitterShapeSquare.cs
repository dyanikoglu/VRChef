using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace Obi
{

	[ExecuteInEditMode]
	public class ObiEmitterShapeSquare : ObiEmitterShape
	{
		public Vector2 size = Vector2.one;

		public override void GenerateDistribution(){

			distribution.Clear(); 

			int numX = Mathf.FloorToInt(size.x/particleSize*0.5f);
			int numY = Mathf.FloorToInt(size.y/particleSize*0.5f);

			for (int x = -numX; x <= numX; ++x){
				for (int y = -numY; y <= numY; ++y){
					Vector3 pos = new Vector3(x,y,0)*particleSize;
					Vector3 vel = Vector3.forward;

					distribution.Add(new ObiEmitterShape.DistributionPoint(pos,vel));	
				}
			}
	
		}

		public override bool SupportsAllSamplingMethods(){return false;}


	#if UNITY_EDITOR
		public void OnDrawGizmosSelected(){

			Handles.matrix = transform.localToWorldMatrix;
			Handles.color  = Color.cyan;

			float sx = size.x*0.5f;
			float sy = size.y*0.5f;

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

