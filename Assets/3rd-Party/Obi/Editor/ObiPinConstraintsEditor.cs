using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Obi{
	
	/**
	 * Custom inspector for ObiPinConstraints component. 
	 */
	
	[CustomEditor(typeof(ObiPinConstraints)), CanEditMultipleObjects] 
	public class ObiPinConstraintsEditor : Editor
	{
		
		ObiPinConstraints constraints;
		
		public void OnEnable(){
			constraints = (ObiPinConstraints)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();
			
			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");

			// Get the particle actor editor to retrieve selected particles:
			ObiParticleActorEditor[] editors = (ObiParticleActorEditor[])Resources.FindObjectsOfTypeAll(typeof(ObiParticleActorEditor));
 			
			// If there's any particle actor editor active, we can show pin constraints:
			if (editors.Length >0)
 			{
			
				List<int> selectedPins = new List<int>();
				List<int> removedPins = new List<int>();

				if (constraints.GetBatches().Count > 0){

					ObiPinConstraintBatch batch = (ObiPinConstraintBatch)constraints.GetBatches()[0];
						
					// Get the list of pin constraints from the selected particles:
					for (int i = 0; i < batch.ConstraintCount; i++){
	
						int particleIndex = batch.pinIndices[i];
						
						if (particleIndex >= 0 && particleIndex < ObiParticleActorEditor.selectionStatus.Length && 
							ObiParticleActorEditor.selectionStatus[particleIndex]){
	
							selectedPins.Add(i);
	
						}
					}
	
					if (selectedPins.Count > 0){
			
						//Iterate over all constraints:
						foreach (int i in selectedPins){
			
							GUILayout.BeginVertical("box");
									
							GUILayout.BeginHorizontal();
							
							EditorGUI.BeginChangeCheck();
							bool allowSceneObjects = !EditorUtility.IsPersistent(target);
							batch.pinBodies[i] = EditorGUILayout.ObjectField("Pinned to:",batch.pinBodies[i],typeof(ObiCollider),allowSceneObjects) as ObiCollider;
							
							// Calculate initial pin offset value after changing the rigidbody.
							if (EditorGUI.EndChangeCheck() && batch.pinBodies[i] != null){
								batch.pinOffsets[i] = batch.pinBodies[i].transform.InverseTransformPoint(constraints.Actor.GetParticlePosition(batch.pinIndices[i]));
							}
							
							Color oldColor = GUI.color;
							GUI.color = Color.red;
							if (GUILayout.Button("X",GUILayout.Width(30))){
								// Mark this constraint to be removed outside of the loop.
								removedPins.Add(i);
								continue;
							}
							GUI.color = oldColor;
							
							GUILayout.EndHorizontal();
							
							batch.pinOffsets[i] = EditorGUILayout.Vector3Field("Offset:",batch.pinOffsets[i]);
							batch.pinBreakResistance[i] = EditorGUILayout.DelayedFloatField("Break Resistance:",batch.pinBreakResistance[i]);
							
							GUILayout.EndVertical();
			
						}
	
					}else{
						EditorGUILayout.HelpBox("No pin constraints for the selected particles.",MessageType.Info);
					}
	
					if (GUILayout.Button("Remove selected")){
	
						for (int i = 0; i < batch.ConstraintCount; i++){
							int particleIndex = batch.pinIndices[i];
						
							if (particleIndex >= 0 && particleIndex < ObiParticleActorEditor.selectionStatus.Length && 
								ObiParticleActorEditor.selectionStatus[particleIndex]){
		
								removedPins.Add(i);
		
							}
						}		
					}
	
					if (GUILayout.Button("Add Pin Constraint")){
	
						Undo.RecordObject(constraints, "Add pin constraints");
		
						bool wasInSolver = constraints.InSolver;
						constraints.RemoveFromSolver(null);
			
						for(int i = 0; i < ObiParticleActorEditor.selectionStatus.Length; i++){
							if (ObiParticleActorEditor.selectionStatus[i]){
								batch.AddConstraint(i,null,Vector3.zero,0);
							}
						}
	
						if (wasInSolver) 
							constraints.AddToSolver(null);
					}
					
					// Remove selected constraint outside of constraint listing loop:
					if (removedPins.Count > 0){
	
						Undo.RecordObject(constraints, "Remove pin constraints");
	
						bool wasInSolver = constraints.InSolver;
						constraints.RemoveFromSolver(null);
	
						// Remove from last to first, to avoid throwing off subsequent indices:
						foreach(int i in removedPins.OrderByDescending(i => i)){
							batch.RemoveConstraint(i);
						}
	
						if (wasInSolver) 
							constraints.AddToSolver(null);
	
					}
				}

			}
		
			// Apply changes to the serializedProperty
			if (GUI.changed){
				
				serializedObject.ApplyModifiedProperties();
				
				constraints.PushDataToSolver();
				
			}
			
		}

		/**
		 * Draws selected pin constraints in the scene view.
		 */
		public void OnSceneGUI(){

			if (Event.current.type != EventType.Repaint || !ObiParticleActorEditor.editMode) return;
					
			// Get the particle actor editor to retrieve selected particles:
			ObiParticleActorEditor[] editors = (ObiParticleActorEditor[])Resources.FindObjectsOfTypeAll(typeof(ObiParticleActorEditor));
 			
			// If there's any particle actor editor active, we can show pin constraints:
			if (editors.Length >0 && constraints.GetBatches().Count > 0)
 			{

				Handles.color = Color.cyan;
		
				ObiPinConstraintBatch batch = (ObiPinConstraintBatch)constraints.GetBatches()[0];

				// Get the list of pin constraints from the selected particles:
				foreach (int i in batch.ActiveConstraints){

					if (batch.pinBodies[i] != null){
						
						Vector3 pinPosition = batch.pinBodies[i].transform.TransformPoint(batch.pinOffsets[i]);
						Handles.DrawDottedLine(constraints.Actor.GetParticlePosition(batch.pinIndices[i]),pinPosition,5);
						Handles.SphereHandleCap(0,pinPosition,Quaternion.identity,HandleUtility.GetHandleSize(pinPosition)*0.1f,EventType.Repaint);

					}
				}
			}	
		}
		
	}
}

