using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiEmitter components.
	 * Allows particle emission and constraint edition. 
	 * 
	 * Selection:
	 * 
	 * - To select a particle, left-click on it. 
	 * - You can select multiple particles by holding shift while clicking.
	 * - To deselect all particles, click anywhere on the object except a particle.
	 * 
	 * Constraints:
	 * 
	 * - To edit particle constraints, select the particles you wish to edit.
	 * - Constraints affecting any of the selected particles will appear in the inspector.
	 * - To add a new pin constraint to the selected particle(s), click on "Add Pin Constraint".
	 * 
	 */
	[CustomEditor(typeof(ObiEmitter)), CanEditMultipleObjects] 
	public class ObiEmitterEditor : ObiParticleActorEditor
	{

		[MenuItem("GameObject/3D Object/Obi/Obi Emitter",false,4)]
		static void CreateObiCloth()
		{
			GameObject c = new GameObject("Obi Emitter");
			Undo.RegisterCreatedObjectUndo(c,"Create Obi Emitter");
			c.AddComponent<ObiEmitter>();
			c.AddComponent<ObiEmitterShapeDisk>();
			c.AddComponent<ObiParticleRenderer>();
		}

		[MenuItem("GameObject/3D Object/Obi/Obi Emitter (with solver)",false,5)]
		static void CreateObiClothWithSolver()
		{

			GameObject c = new GameObject("Obi Emitter");
			Undo.RegisterCreatedObjectUndo(c,"Create Obi Emitter");
			ObiEmitter em = c.AddComponent<ObiEmitter>();
			c.AddComponent<ObiEmitterShapeDisk>();
			c.AddComponent<ObiParticleRenderer>();

			ObiSolver solver = c.AddComponent<ObiSolver>();
			em.Solver = solver;

		}
		
		ObiEmitter emitter;
		
		public override void OnEnable(){
			base.OnEnable();
			emitter = (ObiEmitter)target;
		}
		
		public override void OnDisable(){
			base.OnDisable();
			EditorUtility.ClearProgressBar();
		}

		public override void UpdateParticleEditorInformation(){
			
			for(int i = 0; i < emitter.positions.Length; i++)
			{
				wsPositions[i] = emitter.GetParticlePosition(i);
				facingCamera[i] = true;		
			}

		}
		
		protected override void SetPropertyValue(ParticleProperty property,int index, float value){
			if (index >= 0 && index < emitter.invMasses.Length){
				switch(property){
				case ParticleProperty.Mass: 
						emitter.invMasses[index] = 1.0f / Mathf.Max(value,0.00001f);
					break; 
				}
			}
		}
		
		protected override float GetPropertyValue(ParticleProperty property, int index){
			if (index >= 0 && index < emitter.invMasses.Length){
				switch(property){
					case ParticleProperty.Mass:{
						return 1.0f/emitter.invMasses[index];
					}
				}
			}
			return 0;
		}

		public override void OnInspectorGUI() {
			
			serializedObject.Update();

			/*GUI.enabled = emitter.Initialized;
			EditorGUI.BeginChangeCheck();
			editMode = GUILayout.Toggle(editMode,new GUIContent("Edit particles",EditorGUIUtility.Load("Obi/EditParticles.psd") as Texture2D),"LargeButton");
			if (EditorGUI.EndChangeCheck()){
				SceneView.RepaintAll();
			}
			GUI.enabled = true;*/

			EditorGUILayout.HelpBox("Active particles:"+ emitter.ActiveParticles,MessageType.Info);

			EditorGUI.BeginChangeCheck();
			ObiSolver solver = EditorGUILayout.ObjectField("Solver",emitter.Solver, typeof(ObiSolver), true) as ObiSolver;
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(emitter, "Set solver");
				emitter.Solver = solver;
			}

			EditorGUI.BeginChangeCheck();
			ObiCollisionMaterial material = EditorGUILayout.ObjectField("Collision Material",emitter.CollisionMaterial, typeof(ObiCollisionMaterial), false) as ObiCollisionMaterial;
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(emitter, "Set collision material");
				emitter.CollisionMaterial = material;
			}

			EditorGUI.BeginChangeCheck();
			ObiEmitterMaterial emitterMaterial = EditorGUILayout.ObjectField(new GUIContent("Emitter Material","Emitter material used. This controls the behavior of the emitted particles."),
																  emitter.EmitterMaterial, typeof(ObiEmitterMaterial), false) as ObiEmitterMaterial;
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(emitter, "Set emitter material");
				emitter.EmitterMaterial = emitterMaterial;
			}

			EditorGUI.BeginChangeCheck();
			int numParticles = EditorGUILayout.IntField(new GUIContent("Num particles","Amount of pooled particles used by this emitter."), emitter.NumParticles);
			if (EditorGUI.EndChangeCheck()){
				Undo.RecordObject(emitter, "Set num particles");
				emitter.NumParticles = numParticles;
			}

			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
			
			// Apply changes to the serializedProperty
			if (GUI.changed){
				emitter.UpdateParticlePhases(); //TODO: only do this when changing material.
				serializedObject.ApplyModifiedProperties();
			}
			
		}
		
	}
}




