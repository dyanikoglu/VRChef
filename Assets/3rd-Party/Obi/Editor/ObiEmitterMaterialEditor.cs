using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{
	
	/**
	 * Custom inspector for ObiEmitterMaterial assets. 
	 */
	
	[CustomEditor(typeof(ObiEmitterMaterial),true), CanEditMultipleObjects] 
	public class ObiEmitterMaterialEditor : Editor
	{

		[MenuItem("Assets/Create/Obi/Obi Emitter Material Fluid")]
		public static void CreateObiEmitterMaterialFluid ()
		{
			ObiEditorUtils.CreateAsset<ObiEmitterMaterialFluid> ();
		}

		[MenuItem("Assets/Create/Obi/Obi Emitter Material Granular")]
		public static void CreateObiEmitterMaterialGranular ()
		{
			ObiEditorUtils.CreateAsset<ObiEmitterMaterialGranular> ();
		}
		
		ObiEmitterMaterial material;	
		
		public void OnEnable(){
			material = (ObiEmitterMaterial)target;
		}
		
		public override void OnInspectorGUI() {
			
			serializedObject.UpdateIfRequiredOrScript();
			
			ObiEmitterMaterial.MaterialChanges changes = ObiEmitterMaterial.MaterialChanges.PER_MATERIAL_DATA | ObiEmitterMaterial.MaterialChanges.PER_PARTICLE_DATA;			

			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");

			EditorGUILayout.HelpBox("Particle mass (kg):\n"+
									"2D:"+material.GetParticleMass(Oni.SolverParameters.Mode.Mode2D)+"\n"+
									"3D;"+material.GetParticleMass(Oni.SolverParameters.Mode.Mode3D)+"\n\n"+
									"Particle size:\n"+
									"2D:"+material.GetParticleSize(Oni.SolverParameters.Mode.Mode2D)+"\n"+
									"3D;"+material.GetParticleSize(Oni.SolverParameters.Mode.Mode3D),MessageType.Info);	

			// Apply changes to the serializedProperty
			if (GUI.changed){
				
				serializedObject.ApplyModifiedProperties();

				material.CommitChanges(changes);
				
			}
			
		}
		
	}
}


