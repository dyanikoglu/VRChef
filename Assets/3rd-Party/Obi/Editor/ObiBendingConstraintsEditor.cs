using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiBendingConstraints component. 
	 */
	
	[CustomEditor(typeof(ObiBendingConstraints)), CanEditMultipleObjects] 
	public class ObiBendingConstraintsEditor : Editor
	{
		
		ObiBendingConstraints constraints;
		
		public void OnEnable(){
			constraints = (ObiBendingConstraints)target;
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

