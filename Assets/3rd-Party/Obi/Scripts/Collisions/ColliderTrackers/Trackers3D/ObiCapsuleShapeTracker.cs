using System;
using UnityEngine;

namespace Obi{

	public class ObiCapsuleShapeTracker : ObiShapeTracker
	{
		private int direction;
		private float radius;
		private float height;
		private Vector3 center;

		public ObiCapsuleShapeTracker(CapsuleCollider collider){
			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.Capsule);
		}	

		public ObiCapsuleShapeTracker(CharacterController collider){
			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.Capsule);
		}	
	
		public override void UpdateIfNeeded (){

			CapsuleCollider capsule = collider as CapsuleCollider;
	
			if (capsule != null && (capsule.radius != radius ||
									capsule.height != height ||
									capsule.direction != direction ||
									capsule.center != center)){
				radius = capsule.radius;
				height = capsule.height;
				direction = capsule.direction;
				center = capsule.center;
				adaptor.Set(center, radius, height, direction);
				Oni.UpdateShape(oniShape,ref adaptor);
			}

			CharacterController character = collider as CharacterController;
	
			if (character != null && (character.radius != radius ||
									character.height != height ||
									character.center != center)){
				radius = character.radius;
				height = character.height;
				center = character.center;
				adaptor.Set(center, radius, height, 1);
				Oni.UpdateShape(oniShape,ref adaptor);
			}

		}

	}
}

