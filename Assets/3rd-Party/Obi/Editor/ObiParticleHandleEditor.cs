using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiParticleHandle component. 
	 */

	[CustomEditor(typeof(ObiParticleHandle)), CanEditMultipleObjects] 
	public class ObiParticleRendererEditor : Editor
	{
	
		ObiParticleHandle renderer;
		
		public void OnEnable(){
			renderer = (ObiParticleHandle)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();

			renderer.Actor = EditorGUILayout.ObjectField("Actor",renderer.Actor, typeof(ObiActor), true) as ObiActor;
			
			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
			
			// Apply changes to the serializedProperty
			if (GUI.changed){
				
				serializedObject.ApplyModifiedProperties();
				
			}
			
		}
		
	}

}

