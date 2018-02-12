using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Obi{

	public class ObiEdgeShapeTracker2D : ObiShapeTracker
	{
		private int pointCount;
		private GCHandle pointsHandle;
		private GCHandle indicesHandle;
		private bool edgeDataHasChanged = false;

		public ObiEdgeShapeTracker2D(EdgeCollider2D collider){

			this.collider = collider;
			adaptor.is2D = true;
			oniShape = Oni.CreateShape(Oni.ShapeType.EdgeMesh);

			UpdateEdgeData();
		}		

		public void UpdateEdgeData(){

			EdgeCollider2D edge = collider as EdgeCollider2D;

			if (edge != null){

				Vector3[] vertices = new Vector3[edge.pointCount];
				int[] indices = new int[edge.edgeCount*2];
	
				Vector2[] points = edge.points;
				for (int i = 0; i < edge.pointCount; ++i){
					vertices[i] = points[i];
				}
	
				for (int i = 0; i < edge.edgeCount; ++i){
					indices[i*2] = i;
					indices[i*2+1] = i+1;
				}
				
				Oni.UnpinMemory(pointsHandle);
				Oni.UnpinMemory(indicesHandle);
	
				pointsHandle = Oni.PinMemory(vertices);
				indicesHandle = Oni.PinMemory(indices);

				edgeDataHasChanged = true;
			}
		}
	
		public override void UpdateIfNeeded (){

			EdgeCollider2D edge = collider as EdgeCollider2D;
	
			if (edge != null && (edge.pointCount != pointCount || 
								 edgeDataHasChanged)){

				pointCount = edge.pointCount;
				edgeDataHasChanged = false;
				adaptor.Set(pointsHandle.AddrOfPinnedObject(),indicesHandle.AddrOfPinnedObject(),edge.pointCount,edge.edgeCount*2);
				Oni.UpdateShape(oniShape,ref adaptor);
			}			

		}

		public override void Destroy(){
			base.Destroy();

			Oni.UnpinMemory(pointsHandle);
			Oni.UnpinMemory(indicesHandle);
		}
	}
}

