using UnityEngine;
using System;
using System.Collections;

namespace Obi{

/**
 * Holds information about the physics properties of a particle or collider, and how it should react to collisions.
 */
public class ObiCollisionMaterial : ScriptableObject
{
	private IntPtr oniCollisionMaterial = IntPtr.Zero;
	Oni.CollisionMaterial adaptor = new Oni.CollisionMaterial();

	public float friction;
	public float stickiness;
	public float stickDistance;
	
	public Oni.MaterialCombineMode frictionCombine;
	public Oni.MaterialCombineMode stickinessCombine;

	public IntPtr OniCollisionMaterial{
		get{return oniCollisionMaterial;}
	}

	public void OnEnable(){
		oniCollisionMaterial = Oni.CreateCollisionMaterial();
		OnValidate();
	}

	public void OnDisable(){
		Oni.DestroyCollisionMaterial(oniCollisionMaterial);
		oniCollisionMaterial = IntPtr.Zero;
	}

	public void OnValidate()
	{
		adaptor.friction = friction;
		adaptor.stickiness = stickiness;
		adaptor.stickDistance = stickDistance;
		adaptor.frictionCombine = frictionCombine;
		adaptor.stickinessCombine = stickinessCombine;
		
		Oni.UpdateCollisionMaterial(oniCollisionMaterial,ref adaptor);
	}
}
}