using System;
using UnityEngine;

namespace Obi{

	public class ObiCapsuleShapeTracker2D : ObiShapeTracker
	{
		private CapsuleDirection2D direction;
		private Vector2 size;
		private Vector2 center;

		public ObiCapsuleShapeTracker2D(CapsuleCollider2D collider){
			this.collider = collider;
			adaptor.is2D = true;
			oniShape = Oni.CreateShape(Oni.ShapeType.Capsule);
		}	
	
		public override void UpdateIfNeeded (){

			CapsuleCollider2D capsule = collider as CapsuleCollider2D;
	
			if (capsule != null && (capsule.size != size ||
									capsule.direction != direction ||
									capsule.offset != center)){
				size = capsule.size;
				direction = capsule.direction;
				center = capsule.offset;
			
				adaptor.Set(center, 
							(capsule.direction == CapsuleDirection2D.Horizontal ? size.y : size.x)*0.5f, 
							Mathf.Max(size.x,size.y), 
							capsule.direction == CapsuleDirection2D.Horizontal ? 0 : 1);

				Oni.UpdateShape(oniShape,ref adaptor);
			}

		}

	}
}

