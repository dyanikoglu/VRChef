using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

	[CustomEditor(typeof(ObiParticleCache))] 
	public class ObiParticleCacheEditor : Editor
	{
		
		ObiParticleCache cache;				
		SerializedProperty localSpace;	

		[MenuItem("Assets/Create/Obi/Obi Particle Cache")]
		public static void CreateObiParticleCache ()
		{
			ObiEditorUtils.CreateAsset<ObiParticleCache> ();
		}
		
		public void OnEnable(){
			cache = (ObiParticleCache) target;
			localSpace = serializedObject.FindProperty("localSpace");
		}
		
		public void OnDisable(){
		}
		
		public override void OnInspectorGUI() {

			serializedObject.UpdateIfRequiredOrScript();

			EditorGUILayout.PropertyField(localSpace);

			if (GUILayout.Button("Clear")){
				if (EditorUtility.DisplayDialog("Clear particle cache","Are you sure you want to clear this cache?","Ok","Cancel")){
					Undo.RecordObject(cache, "Clear particle cache");
					cache.Clear();
				}
			}
			
			// Print topology info:
			EditorGUILayout.HelpBox("Duration:"+ cache.Duration + "\n"+
									"Frames:"+ cache.FrameCount +"\n"+
				                    "Size in memory (kb):"+ cache.SizeInBytes()*0.001f,MessageType.Info);
		
			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();
			
		}
		
	}
}

