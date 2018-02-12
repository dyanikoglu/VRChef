using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Obi{


	[CustomEditor(typeof(ObiParticleBaker))] 
	public class ObiParticleBakerEditor : Editor
	{

		[MenuItem("Component/Physics/Obi/Obi Particle Baker",false,0)]
		static void AddObiParticleBaker() 
		{
			foreach(Transform t in Selection.transforms)
				Undo.AddComponent<ObiParticleBaker>(t.gameObject);
		}
		
		ObiParticleBaker baker;
		SerializedProperty cache;
		SerializedProperty frameSkip;
		SerializedProperty fixedBakeFramerate;
		SerializedProperty interpolate;
		SerializedProperty loopPlayback;
		SerializedProperty bakeOnAwake;
		SerializedProperty playOnAwake;
		
		public void OnEnable(){
			baker = (ObiParticleBaker) target;
			cache = serializedObject.FindProperty("cache");
			frameSkip = serializedObject.FindProperty("frameSkip");
			fixedBakeFramerate = serializedObject.FindProperty("fixedBakeFramerate");
			interpolate = serializedObject.FindProperty("interpolate");
			loopPlayback = serializedObject.FindProperty("loopPlayback");
			bakeOnAwake = serializedObject.FindProperty("bakeOnAwake");
			playOnAwake = serializedObject.FindProperty("playOnAwake");
		}
		
		public void OnDisable(){
		}
		
		public override void OnInspectorGUI() {

			serializedObject.UpdateIfRequiredOrScript();

			EditorGUILayout.PropertyField(cache);

			GUI.enabled = (baker.cache != null);
			if (GUILayout.Button("Clear cache")){
				if (EditorUtility.DisplayDialog("Clear particle cache","Are you sure you want to clear this cache?","Ok","Cancel")){
					Undo.RecordObject(baker.cache, "Clear particle cache");
					baker.cache.Clear();
				}
			}
			GUI.enabled = true;

			EditorGUILayout.PropertyField(frameSkip);
			EditorGUILayout.PropertyField(fixedBakeFramerate);
			EditorGUILayout.PropertyField(interpolate);
			EditorGUILayout.PropertyField(loopPlayback);
			EditorGUILayout.PropertyField(bakeOnAwake);
			if (bakeOnAwake.boolValue)
				playOnAwake.boolValue = false;
			EditorGUILayout.PropertyField(playOnAwake);
			if (playOnAwake.boolValue)
				bakeOnAwake.boolValue = false;

			EditorGUILayout.Space();

			if (!baker.Baking){
				GUI.enabled = (baker.cache != null && baker.Playing);
				EditorGUI.BeginChangeCheck();
				float newPlayhead = EditorGUILayout.Slider(baker.playhead,0,baker.cache != null?baker.cache.Duration:0);
				if (EditorGUI.EndChangeCheck()){
					baker.playhead = newPlayhead;
				}
				GUI.enabled = (baker.cache != null);
			}else{
				EditorGUILayout.LabelField("Cache time: " + baker.playhead);
			}

			GUI.enabled = (baker.cache != null);

			EditorGUILayout.BeginHorizontal();

			if (!baker.Baking){
				if (GUILayout.Button(new GUIContent("Bake",Resources.Load<Texture2D>("RecButton") ))){
					baker.Baking = true;
				}
			}else{
				if (GUILayout.Button(new GUIContent("Stop Bake",Resources.Load<Texture2D>("StopRecButton")))){
					baker.Baking = false;
				}
			}
			
			if (!baker.Playing){
				if (GUILayout.Button(new GUIContent("Play",Resources.Load<Texture2D>("PlayButton") ))){
					baker.Playing = true;
				}
			}else{
				if (GUILayout.Button(new GUIContent("Stop",Resources.Load<Texture2D>("StopButton")))){
					baker.Playing = false;
				}
			}

			if (!baker.Paused){
				if (GUILayout.Button(new GUIContent("Pause",Resources.Load<Texture2D>("PauseButton")))){
					baker.Paused = true;
				}
			}else{
				if (GUILayout.Button(new GUIContent("Resume",Resources.Load<Texture2D>("StepButton") ))){
					baker.Paused = false;
				}
			}
		
			EditorGUILayout.EndHorizontal();

			GUI.enabled = true;

			Color oldColor = GUI.color;

			if (baker.Baking){
				GUI.color = Color.red;
				EditorGUILayout.HelpBox("Baking...",MessageType.None);
			}

			if (baker.Playing){
				GUI.color = Color.green;
				EditorGUILayout.HelpBox("Playing...",MessageType.None);
			}

		 	GUI.color = oldColor;
		
			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();
			
		}
		
	}
}

