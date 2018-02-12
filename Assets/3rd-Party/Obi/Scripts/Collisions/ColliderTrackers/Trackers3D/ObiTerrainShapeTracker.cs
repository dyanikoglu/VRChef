using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Obi{

	public class ObiTerrainShapeTracker : ObiShapeTracker
	{
		private Vector3 size;
		private int resolutionU;
		private int resolutionV;
		private GCHandle dataHandle;
		private bool heightmapDataHasChanged = false;

		public ObiTerrainShapeTracker(TerrainCollider collider){

			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.Heightmap);

			UpdateHeightData();
		}		

		public void UpdateHeightData(){

			TerrainCollider terrain = collider as TerrainCollider;

			if (terrain != null){

				TerrainData data = terrain.terrainData;
	
				float[,] heights = data.GetHeights(0,0,data.heightmapWidth,data.heightmapHeight);
				
				float[] buffer = new float[data.heightmapWidth * data.heightmapHeight];
				for (int y = 0; y < data.heightmapHeight; ++y)
					for (int x = 0; x < data.heightmapWidth; ++x)
						buffer[y*data.heightmapWidth+x] = heights[y,x];
				
				Oni.UnpinMemory(dataHandle);
	
				dataHandle = Oni.PinMemory(buffer);

				heightmapDataHasChanged = true;
			}
		}
	
		public override void UpdateIfNeeded (){

			TerrainCollider terrain = collider as TerrainCollider;
	
			if (terrain != null){

				TerrainData data = terrain.terrainData;

				if (data != null && (data.size != size || 
									 data.heightmapWidth != resolutionU ||
									 data.heightmapHeight != resolutionV || 
									 heightmapDataHasChanged)){

					size = data.size;
					resolutionU = data.heightmapWidth;
					resolutionV = data.heightmapHeight;
					heightmapDataHasChanged = false;
					adaptor.Set(size,resolutionU,resolutionV,dataHandle.AddrOfPinnedObject());
					Oni.UpdateShape(oniShape,ref adaptor);
				}			
			}

		}

		public override void Destroy(){
			base.Destroy();

			Oni.UnpinMemory(dataHandle);
		}
	}
}

