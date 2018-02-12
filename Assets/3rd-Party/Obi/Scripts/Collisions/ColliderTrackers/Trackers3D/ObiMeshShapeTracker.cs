using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Obi{

	public class ObiMeshShapeTracker : ObiShapeTracker
	{
		private class MeshDataHandles{

			private int refCount = 1;
			private GCHandle verticesHandle;
			private GCHandle indicesHandle;

			public int RefCount{
				get{return refCount;}
			}

			public IntPtr VerticesAddress{
				get{return verticesHandle.AddrOfPinnedObject();}
			}

			public IntPtr IndicesAddress{
				get{return indicesHandle.AddrOfPinnedObject();}
			}

			public void FromMesh(Mesh mesh){
				Oni.UnpinMemory(verticesHandle);
				Oni.UnpinMemory(indicesHandle);
				verticesHandle = Oni.PinMemory(mesh.vertices);
				indicesHandle = Oni.PinMemory(mesh.triangles);
			}			

			public void Ref(){			
				refCount++;
			}

			public void Unref(){
				refCount--;
				if (refCount <= 0){
					refCount = 0;
					Oni.UnpinMemory(verticesHandle);
					Oni.UnpinMemory(indicesHandle);
				}
			}			
		}

		private static Dictionary<Mesh,MeshDataHandles> meshDataCache = new Dictionary<Mesh,MeshDataHandles>();
		private bool meshDataHasChanged = false;
		private MeshDataHandles handles;

		public ObiMeshShapeTracker(MeshCollider collider){

			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.TriangleMesh);

			UpdateMeshData();	

		}		

		/**
		 * Updates mesh data, in case the collider mesh had its vertices modified, or is an entirely different mesh.
		 */
		public void UpdateMeshData(){

			MeshCollider meshCollider = collider as MeshCollider;

			if (meshCollider != null){

				Mesh mesh = meshCollider.sharedMesh;
				
				// Decrease reference count of current handles:
				if (handles != null)
					handles.Unref();

				MeshDataHandles newHandles;

				// if handles do not exist for this mesh, create them:
				if (!meshDataCache.TryGetValue(mesh,out newHandles)){
					handles = new MeshDataHandles();
					meshDataCache[mesh] = handles;
				}
				// if the handles already exist, increase their reference count and set them as the current handles.
				else{ 
					newHandles.Ref();
					handles = newHandles;
				}

				// Update handles from mesh:
				handles.FromMesh(meshCollider.sharedMesh);

				meshDataHasChanged = true;
			}
		}
	
		public override void UpdateIfNeeded (){

			MeshCollider meshCollider = collider as MeshCollider;
	
			if (meshCollider != null){

				Mesh mesh = meshCollider.sharedMesh;

				if (mesh != null && meshDataHasChanged){
					meshDataHasChanged = false;
					adaptor.Set(handles.VerticesAddress,handles.IndicesAddress,mesh.vertexCount,mesh.triangles.Length);
					Oni.UpdateShape(oniShape,ref adaptor);
				}			
			}

		}

		public override void Destroy(){
			base.Destroy();

			MeshCollider meshCollider = collider as MeshCollider;

			if (meshCollider != null && handles != null){

				handles.Unref(); // Decrease handles refcount.

				if (handles.RefCount <= 0)
					meshDataCache.Remove(meshCollider.sharedMesh);
				
			}
		}
	}
}

