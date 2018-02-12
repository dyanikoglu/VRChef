using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiDistanceConstraints component. 
	 */

	[CustomEditor(typeof(ObiDistanceConstraints)), CanEditMultipleObjects] 
	public class ObiDistanceConstraintsEditor : Editor
	{
	
		ObiDistanceConstraints constraints;
		
		public void OnEnable(){
			constraints = (ObiDistanceConstraints)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();
			
			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
			
			// Apply changes to the serializedProperty
			if (GUI.changed){
				
				serializedObject.ApplyModifiedProperties();
				
				constraints.PushDataToSolver();
				
			}
			
		}
		
	}

}

