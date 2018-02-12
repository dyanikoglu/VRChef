﻿using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiCollider assets. 
	 */
	
	[CustomEditor(typeof(ObiColliderBase),true), CanEditMultipleObjects] 
	public class ObiColliderEditor : Editor
	{
		
		ObiColliderBase collider;	
		
		public void OnEnable(){
			collider = (ObiColliderBase)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();

			EditorGUI.BeginChangeCheck();
			ObiCollisionMaterial material = EditorGUILayout.ObjectField("Collision material",collider.CollisionMaterial, typeof(ObiCollisionMaterial), false) as ObiCollisionMaterial;
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(collider, "Set collision material");
				collider.CollisionMaterial = material;
			}

			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");

			// Apply changes to the serializedProperty
			if (GUI.changed){
				serializedObject.ApplyModifiedProperties();
			}
			
		}
		
	}
}


