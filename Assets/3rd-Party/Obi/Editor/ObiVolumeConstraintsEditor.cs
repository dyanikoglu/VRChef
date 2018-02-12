using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiVolumeConstraints component. 
	 */
	
	[CustomEditor(typeof(ObiVolumeConstraints)), CanEditMultipleObjects] 
	public class ObiVolumeConstraintsEditor : Editor
	{
		
		ObiVolumeConstraints constraints;
		
		public void OnEnable(){
			constraints = (ObiVolumeConstraints)target;
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
