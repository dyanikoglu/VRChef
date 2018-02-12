using System;
using UnityEngine;

namespace Obi{

	public abstract class ObiShapeTracker
	{
		protected Component collider;
		protected Oni.Shape adaptor = new Oni.Shape();
		protected IntPtr oniShape = IntPtr.Zero;

		public IntPtr OniShape {
			get{return oniShape;}
		}

		public virtual void Destroy(){
			Oni.DestroyShape(oniShape);
			oniShape = IntPtr.Zero;
		}

		public abstract void UpdateIfNeeded ();

	}

}


