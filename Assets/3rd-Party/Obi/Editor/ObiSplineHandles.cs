using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

public class ObiSplineHandles
{

	static int splineSelectorHash = "ObiSplineSelectorHash".GetHashCode();

	static Vector2 startPos;
	static Vector2 currentPos;
	static bool dragging = false;
	static Rect marquee;

	public static bool SplineCPSelector(Vector3[] positions,
	                                    bool[] selectionStatus){

		Matrix4x4 cachedMatrix = Handles.matrix;

		int controlID = GUIUtility.GetControlID(splineSelectorHash,FocusType.Passive);
		int selectedCPIndex = -1;
		bool selectionStatusChanged = false;

		// select vertex on mouse click:
		switch (Event.current.GetTypeForControl(controlID)){

		case EventType.MouseDown: {

			if ((Event.current.modifiers & EventModifiers.Control) == 0 && 
				(HandleUtility.nearestControl != controlID || Event.current.button != 0)) break;
			
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
				
				// get particle position in gui space:
				Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
				
				// get distance from mouse position to particle position:
				float sqrDistance = Vector2.SqrMagnitude(startPos-pos);
				
				// check if this control point is closer to the cursor that any previously considered point.
				if (sqrDistance < 100 && sqrDistance < minSqrDistance){ //magic number 100 = 10*10, where 10 is min distance in pixels to select a particle.
					minSqrDistance = sqrDistance;
					selectedCPIndex = i;
				}
				
			}
			
			if (selectedCPIndex >= 0){ // toggle particle selection status.
				
				selectionStatus[selectedCPIndex] = !selectionStatus[selectedCPIndex];
				selectionStatusChanged = true;

				// Prevent spline deselection if we have selected a particle:
				GUIUtility.hotControl = controlID;
				Event.current.Use();

			}
			else if (Event.current.modifiers == EventModifiers.None){ // deselect all particles:
				for(int i = 0; i < selectionStatus.Length; i++)
					selectionStatus[i] = false;

				selectionStatusChanged = true;
			}
			
			} break;
			
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


		case EventType.Layout:{
             Handles.matrix = Matrix4x4.identity;

			float minSqrDistance = System.Single.MaxValue;
			
			for(int i = 0; i < positions.Length; i++){
				
				// get particle position in gui space:
				Vector2 pos = HandleUtility.WorldToGUIPoint(positions[i]);
				
				// get distance from mouse position to particle position:
				float sqrDistance = Vector2.SqrMagnitude(Event.current.mousePosition-pos);
				
				// check if this control point is closer to the cursor that any previously considered point.
				if (sqrDistance < 100 && sqrDistance < minSqrDistance){ //magic number 100 = 10*10, where 10 is min distance in pixels to select a particle.
					minSqrDistance = sqrDistance;
				}
				
			}

             HandleUtility.AddControl(controlID,  Mathf.Sqrt(minSqrDistance) );
             Handles.matrix = cachedMatrix;
         }break;
			
		}

		return selectionStatusChanged;
	}

}
}

