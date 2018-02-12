using System;
using UnityEngine;

namespace Obi{

	public class ObiBoxShapeTracker2D : ObiShapeTracker
	{
		private Vector2 size;
		private Vector2 center;

		public ObiBoxShapeTracker2D(BoxCollider2D collider){
			this.collider = collider;
			adaptor.is2D = true;
			oniShape = Oni.CreateShape(Oni.ShapeType.Box);
		}		
	
		public override void UpdateIfNeeded (){

			BoxCollider2D box = collider as BoxCollider2D;
	
			if (box != null && (box.size != size || box.offset != center)){
				size = box.size;
				center = box.offset;
				adaptor.Set(center, size);
				Oni.UpdateShape(oniShape,ref adaptor);
			}

		}

	}
}

