using System;
using UnityEngine;

namespace Obi{

	public class ObiSphereShapeTracker : ObiShapeTracker
	{
		private float radius;
		private Vector3 center;

		public ObiSphereShapeTracker(SphereCollider collider){
			this.collider = collider;
			adaptor.is2D = false;
			oniShape = Oni.CreateShape(Oni.ShapeType.Sphere);
		}	

		public override void UpdateIfNeeded (){

			SphereCollider sphere = collider as SphereCollider;
	
			if (sphere != null && (sphere.radius != radius || sphere.center != center)){
				radius = sphere.radius;
				center = sphere.center;
				adaptor.Set(center, radius);
				Oni.UpdateShape(oniShape,ref adaptor);
			}

		}

	}
}

