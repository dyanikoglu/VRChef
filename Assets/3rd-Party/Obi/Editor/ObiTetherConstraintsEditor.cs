using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiTetherConstraints component. 
	 */
	
	[CustomEditor(typeof(ObiTetherConstraints)), CanEditMultipleObjects] 
	public class ObiTetherConstraintsEditor : Editor
	{
		
		ObiTetherConstraints constraints;
		ObiActor.TetherType tetherType;
		
		public void OnEnable(){
			constraints = (ObiTetherConstraints)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();
			
			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
			
			GUI.enabled = (constraints.Actor != null && constraints.Actor.Initialized);

			
			GUILayout.BeginHorizontal();
			
				if (GUILayout.Button("Generate Tethers")){
	
					if (constraints.Actor != null){
	
						Undo.RegisterCompleteObjectUndo(constraints, "Generate tethers");
	
						constraints.RemoveFromSolver(null);
						if (!constraints.Actor.GenerateTethers(tetherType)){
							Debug.LogWarning("Could not generate tethers. Make sure the actor has been properly initialized.");
						}
						constraints.AddToSolver(null);
					}
				}
	
				//if (constraints.Actor is ObiRope)
					//tetherType = (ObiActor.TetherType)EditorGUILayout.EnumPopup(tetherType);

			GUILayout.EndHorizontal();

			if (GUILayout.Button("Clear Tethers")){

				if (constraints.Actor != null){

					if (EditorUtility.DisplayDialog("Clear tethers","Are you sure you want to remove all tethers?","Ok","Cancel")){

						Undo.RegisterCompleteObjectUndo(constraints, "Clear tethers");
	
						constraints.RemoveFromSolver(null);
						constraints.Actor.ClearTethers();
						constraints.AddToSolver(null);

					}
				}
			}
			
			GUI.enabled = true;
			
			// Apply changes to the serializedProperty
			if (GUI.changed){
				
				serializedObject.ApplyModifiedProperties();
				
				constraints.PushDataToSolver();
				
			}
			
		}
		
	}
}

