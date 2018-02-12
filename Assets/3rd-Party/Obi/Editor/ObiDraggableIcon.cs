using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Obi{
public class ObiDraggableIcon {
	
	public static bool Draw(bool selected, int id, ref Vector2 position, Color color){

		Texture texture = Resources.Load<Texture2D>("Particle");

		int controlID = GUIUtility.GetControlID(id,FocusType.Passive);		

		// select vertex on mouse click:
		switch (Event.current.GetTypeForControl(controlID)){
			
		case EventType.MouseDown: 
			
			Rect area = new Rect (position.x-texture.height*0.5f, position.y-texture.height*0.5f, texture.height*2, texture.height*2);

			if (area.Contains(Event.current.mousePosition)){
				selected = true;
				GUIUtility.hotControl = controlID;
				Event.current.Use();
			}else if ((Event.current.modifiers & EventModifiers.Shift) == 0 && (Event.current.modifiers & EventModifiers.Command) == 0){
				
				selected = false;

			}
			
			break;
			
		case EventType.MouseDrag:
			
			if (GUIUtility.hotControl == controlID){
				
				position = Event.current.mousePosition;
				GUI.changed = true;

				Event.current.Use();

			}
			
			break;
			
		case EventType.MouseUp:
			
			if (GUIUtility.hotControl == controlID){
				
				GUIUtility.hotControl = 0;
				Event.current.Use();

			}
			
			break;

		case EventType.Repaint:
	
				Color oldColor = GUI.color;
				GUI.color = color;
				Rect rect = new Rect (position.x-texture.height*0.5f, position.y-texture.height*0.5f, texture.height, texture.height);
				GUI.DrawTextureWithTexCoords (rect,texture,new Rect(selected?0.5f:0,0,0.5f,1));
				GUI.color = oldColor;

			break;
			
		}

		return selected;	
	}
}
}
