using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

public struct ParticleStampInfo{
	
	public int index;
	public float sqrDistanceToMouse;
	
	public ParticleStampInfo(int particleIndex, float particleDistanceToBrush){
		this.index = particleIndex;
		this.sqrDistanceToMouse = particleDistanceToBrush;
	}
}

public class ObiClothParticleHandles
{

	static int particleSelectorHash = "ObiParticleSelectorHash".GetHashCode();
	static int particleBrushHash = "ObiParticleBrushHash".GetHashCode();

	static Vector2 startPos;
	static Vector2 currentPos;
	static bool dragging = false;
	static Rect marquee;

	public static bool ParticleSelector(Vector3[] positions,
	                                    bool[] selectionStatus,
										ObiParticleActorEditor.FaceCulling selectBackfaces,
	                                    bool[] facingCamera){

		Matrix4x4 cachedMatrix = Handles.matrix;

		int controlID = GUIUtility.GetControlID(particleSelectorHash,FocusType.Passive);
		int selectedParticleIndex = -1;
		bool selectionStatusChanged = false;
		
		// select vertex on mouse click:
		switch (Event.current.GetTypeForControl(controlID)){
			
		case EventType.MouseDown: 
			
			if (Event.current.button != 0) break;
			
			startPos = Event.current.mousePosition;
			marquee.Set(0,0,0,0);
			
			// If the user is pressing shift, accumulate selection.
			if ((Event.current.modifiers & EventModifiers.Shift) == 0 && (Event.current.modifiers & EventModifiers.Alt) == 0){
				for(int i = 0; i < selectionStatus.Length; i++)
					selectionStatus[i] = false;
			}
			
			// If the user is holding down control, dont allow selection of other objects and use marquee tool.
			if ((Event.current.modifiers & EventModifiers.Control) != 0)
				GUIUtility.hotControl = controlID;
			
			float minSqrDistance = System.Single.MaxValue;
			
			for(int i = 0; i < positions.Length; i++){
				
				// skip not selectable particles:
				switch(selectBackfaces){
					case ObiParticleActorEditor.FaceCulling.Back: if (!facingCamera[i]) continue; break;
					case ObiParticleActorEditor.FaceCulling.Front: if (facingCamera[i]) continue; break;
				}
				
				// get particle position in gui space:
				Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
				
				// get distance from mouse position to particle position:
				float sqrDistance = Vector2.SqrMagnitude(startPos-pos);
				
				// check if this particle is closer to the cursor that any previously considered particle.
				if (sqrDistance < 100 && sqrDistance < minSqrDistance){ //magic number 100 = 10*10, where 10 is min distance in pixels to select a particle.
					minSqrDistance = sqrDistance;
					selectedParticleIndex = i;
				}
				
			}
			
			if (selectedParticleIndex >= 0){ // toggle particle selection status.
				
				selectionStatus[selectedParticleIndex] = !selectionStatus[selectedParticleIndex];
				selectionStatusChanged = true;

				// Prevent cloth deselection if we have selected a particle:
				GUIUtility.hotControl = controlID;
				Event.current.Use();

			}
			else if (Event.current.modifiers == EventModifiers.None){ // deselect all particles:
				for(int i = 0; i < selectionStatus.Length; i++)
					selectionStatus[i] = false;

				selectionStatusChanged = true;
			}
			
			break;

		case EventType.MouseMove:
			SceneView.RepaintAll();
			break;
			
		case EventType.MouseDrag:
			
			if (GUIUtility.hotControl == controlID){
				
				currentPos = Event.current.mousePosition;
				if (!dragging && Vector2.Distance(startPos, currentPos) > 5) {
					dragging = true;
				}else{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				
				//update marquee rect:
				float left = Mathf.Min(startPos.x,currentPos.x);
				float right = Mathf.Max(startPos.x,currentPos.x);
				float bottom = Mathf.Min(startPos.y,currentPos.y);
				float top = Mathf.Max(startPos.y,currentPos.y);
				
				marquee = new Rect(left, bottom, right - left, top - bottom);
				
			}
			
			break;
			
		case EventType.MouseUp:
			
			if (GUIUtility.hotControl == controlID){
				
				dragging = false;
				
				for(int i = 0; i < positions.Length; i++){

					// skip not selectable particles:
					switch(selectBackfaces){
						case ObiParticleActorEditor.FaceCulling.Back: if (!facingCamera[i]) continue; break;
						case ObiParticleActorEditor.FaceCulling.Front: if (facingCamera[i]) continue; break;
					}
					
					// get particle position in gui space:
					Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
					
					if (pos.x > marquee.xMin && pos.x < marquee.xMax && pos.y > marquee.yMin && pos.y < marquee.yMax){
						selectionStatus[i] = true;
						selectionStatusChanged = true;
					}
					
				}
				
				GUIUtility.hotControl = 0;
				Event.current.Use();
			}
			
			break;

		case EventType.Repaint:
			
			Handles.matrix = Matrix4x4.identity;

			if(dragging){
				GUISkin oldSkin = GUI.skin;
				GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
				Handles.BeginGUI();
					GUI.Box (new Rect (marquee.xMin, marquee.yMin, marquee.width, marquee.height),"");
				Handles.EndGUI();
				GUI.skin = oldSkin;
			}

			Handles.matrix = cachedMatrix;

			break;
			
		}

		return selectionStatusChanged;
	}


	public static bool ParticleBrush(Vector3[] positions,
									 ObiParticleActorEditor.FaceCulling selectBackfaces,
                                     bool[] facingCamera,
									 float radius,
									 Action strokeStart,
									 Action<List<ParticleStampInfo>,bool> particlesAffected,
									 Action strokeEnd,
									 Texture2D brushImage){
		
		Matrix4x4 cachedMatrix = Handles.matrix;
		
		int controlID = GUIUtility.GetControlID(particleBrushHash,FocusType.Passive);
		bool selectionStatusChanged = false;

		List<ParticleStampInfo> affectedParticles = new List<ParticleStampInfo>();

		switch (Event.current.GetTypeForControl(controlID)){
			
		case EventType.MouseDown: 

			if (Event.current.button != 0 || (Event.current.modifiers & ~EventModifiers.Shift) != EventModifiers.None) break;

			GUIUtility.hotControl = controlID;
			startPos = Event.current.mousePosition;
			
			for(int i = 0; i < positions.Length; i++){
				
				// skip not selectable particles:
				switch(selectBackfaces){
					case ObiParticleActorEditor.FaceCulling.Back: if (!facingCamera[i]) continue; break;
					case ObiParticleActorEditor.FaceCulling.Front: if (facingCamera[i]) continue; break;
				}
				
				// get particle position in gui space:
				Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
				
				// get distance from mouse position to particle position:
				float sqrDistance = Vector2.SqrMagnitude(startPos-pos);
				
				if (sqrDistance < radius*radius){
					affectedParticles.Add(new ParticleStampInfo(i,sqrDistance));
				}
				
			}

			if (affectedParticles.Count > 0){
				selectionStatusChanged = true;
				if (strokeStart != null) strokeStart();
				if (particlesAffected != null) particlesAffected(affectedParticles,(Event.current.modifiers & EventModifiers.Shift) != 0);
			}

			Event.current.Use();
			
			break;

		case EventType.MouseMove:
			SceneView.RepaintAll();
			break;
			
		case EventType.MouseDrag:
			
			if (GUIUtility.hotControl == controlID){
				
				currentPos = Event.current.mousePosition;
				dragging = true;

				for(int i = 0; i < positions.Length; i++){
					
					// skip not selectable particles:
					switch(selectBackfaces){
						case ObiParticleActorEditor.FaceCulling.Back: if (!facingCamera[i]) continue; break;
						case ObiParticleActorEditor.FaceCulling.Front: if (facingCamera[i]) continue; break;
					}
					
					// get particle position in gui space:
					Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
					
					// get distance from mouse position to particle position:
					float sqrDistance = Vector2.SqrMagnitude(currentPos-pos);
					
					if (sqrDistance < radius*radius){
						affectedParticles.Add(new ParticleStampInfo(i,sqrDistance));
					}
					
				}

				if (affectedParticles.Count > 0){
					selectionStatusChanged = true;
					if (particlesAffected != null) particlesAffected(affectedParticles,(Event.current.modifiers & EventModifiers.Shift) != 0);
				}

				Event.current.Use();

			}
			
			break;
			
		case EventType.MouseUp:
			
			if (GUIUtility.hotControl == controlID){
				
				dragging = false;
				
				GUIUtility.hotControl = 0;
				Event.current.Use();
				if (strokeEnd != null) strokeEnd();
			}
			
			break;
			
		case EventType.Repaint:
			
			Handles.matrix = Matrix4x4.identity;

				Handles.BeginGUI();
				GUI.DrawTexture (new Rect (Event.current.mousePosition.x - radius,
				                           Event.current.mousePosition.y - radius, 
				                           radius*2, 
				                           radius*2),brushImage);
				Handles.EndGUI();
				
			Handles.matrix = cachedMatrix;
			
			break;
			
		}
		
		return selectionStatusChanged;
	}

}
}

