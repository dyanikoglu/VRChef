using System;
using UnityEngine;

namespace Obi{

	public class ObiBoxShapeTracker : ObiShapeTracker
	{
		private Vector3 size;
		private Vector3 center;

		public ObiBoxShapeTracker(BoxCollider collider){
			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.Box);
		}		
	
		public override void UpdateIfNeeded (){

			BoxCollider box = collider as BoxCollider;
	
			if (box != null && (box.size != size || box.center != center)){
				size = box.size;
				center = box.center;
				adaptor.Set(center, size);
				Oni.UpdateShape(oniShape,ref adaptor);
			}

		}

	}
}

