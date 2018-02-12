using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Obi{
	
	/**
	 * Custom inspector for ObiActor components.
	 * Allows particle selection and constraint edition. 
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
	[CustomEditor(typeof(ObiActor)), CanEditMultipleObjects] 
	public abstract class ObiParticleActorEditor : Editor
	{

		protected List<string> particlePropertyNames;

		public class ParticleProperty
		{
		  public int Value { get; set; }
		 
		  public const int Mass = 0;
		  public const int Radius = 1;
		  public const int Layer = 2;

		  public ParticleProperty (int value)
		  {
		    Value = value;
		  }
		 
		  public static implicit operator int(ParticleProperty cType)
		  {
		    return cType.Value;
		  }
		 
		  public static implicit operator ParticleProperty (int value)
		  {
		    return new ParticleProperty (value);
		  }
		}
		
		public enum PaintBrushType{
			Gaussian,
			Pencil,
			Smooth
		}

		public enum FaceCulling{
			Off,
			Back,
			Front
		}

		public enum TextureChannel{
			Red = 0,
			Green = 1,
			Blue = 2,
			Alpha = 3,
		}
		
		ObiActor actor;
		Mesh particlesMesh;
		protected EditorCoroutine routine;
		
		public static bool editMode = false;
		public static bool selectionBrush = false;
		public static bool paintBrush = false;
		public static bool textureProperties = false;
		public static PaintBrushType paintMode = PaintBrushType.Gaussian;

		public ParticleProperty currentProperty = ParticleProperty.Mass;
		
		static Gradient valueGradient = new Gradient();
		
		static protected FaceCulling faceCulling = FaceCulling.Back;
		Rect uirect;
		
		//Property edition related:
		static int lastSelectedParticle = 0;
		static float newProperty = 0;
		
		static bool autoRangeDraw = true;
		static float maxRangeValue = Single.MinValue;
		static float minRangeValue = Single.MaxValue;

		static protected float minPropertyValue = 0;
		static protected float maxPropertyValue = 10;
		
		//Brush related:
		static protected float brushRadius = 50;
		static protected float brushOpacity = 1;
		static protected float brushModulation = 0.1f;
		static protected bool selectionMask = false;

		// Texture property stuff:
		Texture2D propertyTexture;
		TextureChannel textureChannel;
		
		//Selection related:
		static protected int selectedCount = 0;
		
		//Editor playback related:
		static protected bool isPlaying = false;
		static protected float lastFrameTime = 0.0f;
		static protected float accumulatedTime = 0.0f;
		
		protected Vector3 camup;
		protected Vector3 camright;
		protected Vector3 camforward;
		
		//Additional GUI styles:
		static protected GUIStyle separatorLine;
		
		//Additional status info for all particles:
		static public bool[] selectionStatus = new bool[0];
		static protected bool[] facingCamera = new bool[0];
		static protected float[] sqrDistanceToCamera = new float[0];
		static protected int[] sortedIndices = new int[0];
		static protected Vector3[] wsPositions = new Vector3[0];

		public static int SelectedParticleCount{
			get{return selectedCount;}
		}
		
		public virtual void OnEnable(){

			actor = (ObiActor)target;

			particlePropertyNames = new List<string>(){"Mass","Radius","Phase"};

			if (actor.Solver)
				actor.Solver.RequireRenderablePositions();

			particlesMesh = new Mesh();
			particlesMesh.hideFlags = HideFlags.HideAndDontSave;
			
			SetupValuesGradient();
			
			separatorLine = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).box);
			separatorLine.normal.background = Resources.Load<Texture2D>("SeparatorLine");
			separatorLine.border = new RectOffset(3,3,0,0);
			separatorLine.fixedHeight = 3;
			separatorLine.stretchWidth = true;

			EditorApplication.update += Update;
			EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;

		}
		
		public virtual void OnDisable(){

			if (actor.Solver)
				actor.Solver.RelinquishRenderablePositions();

			GameObject.DestroyImmediate(particlesMesh);
			EditorApplication.update -= Update;
			EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
			EditorUtility.ClearProgressBar();
		}

		public void OnDestroy()
	    {
	         if ( Application.isEditor )
	         {
	             if(target == null)
					actor.DestroyRequiredComponents();
	         }
	    }
		
		private void SetupValuesGradient(){
			
			GradientColorKey[] gck = new GradientColorKey[2];
			gck[0].color = Color.grey*0.7f;
			gck[0].time = 0.0f;
			gck[1].color = Color.white;
			gck[1].time = 1.0f;
			
			GradientAlphaKey[] gak = new GradientAlphaKey[2];
			gak[0].alpha = 1.0f;
			gak[0].time = 0.0f;
			gak[1].alpha = 1.0f;
			gak[1].time = 1.0f;
			
			valueGradient.SetKeys(gck,gak);
		}
		
		private void ResizeParticleArrays(){
			
			if (actor.positions != null){
				
				// Reinitialize particle property min/max values if needed:
				if (selectionStatus.Length != actor.positions.Length){
					ParticlePropertyChanged();
				}
				
				Array.Resize(ref selectionStatus,actor.positions.Length);
				Array.Resize(ref facingCamera,actor.positions.Length);
				Array.Resize(ref sqrDistanceToCamera,actor.positions.Length);
				Array.Resize(ref sortedIndices,actor.positions.Length);
				Array.Resize(ref wsPositions,actor.positions.Length);
			
			}
			
		}
		
		public static Material particleMaterial;
		public static Material radiiMaterial;
		static void CreateParticleMaterials() {
			if (!particleMaterial) { 
				particleMaterial = Resources.Load<Material>("EditorParticle");
			}
			if (!radiiMaterial) { 
				radiiMaterial = Resources.Load<Material>("EditorParticleRadius");
			}
		}
		
		public void OnSceneGUI(){

			if (!editMode) 
				return;
			
			CreateParticleMaterials();
			
			ResizeParticleArrays();
			
			if (!actor.Initialized) return;
			
			if (Camera.current != null){
				
				camup = Camera.current.transform.up;
				camright = Camera.current.transform.right;
				camforward = Camera.current.transform.forward;
			}
			
			if (Event.current.type == EventType.Repaint){
				
				// Update camera facing status and world space positions array:
				UpdateParticleEditorInformation();

				// Generate sorted indices for back-to-front rendering:
				for(int i = 0; i < sortedIndices.Length; i++)
					sortedIndices[i] = i;

				Array.Sort<int>(sortedIndices, (a,b) => sqrDistanceToCamera[b].CompareTo(sqrDistanceToCamera[a]));
	
				// Draw custom actor stuff.
				DrawActorInfo();
				
			}
			
			// Draw tool handles:
			if (Camera.current != null){
				
 				if (paintBrush){
					if (ObiClothParticleHandles.ParticleBrush(wsPositions,faceCulling,facingCamera,brushRadius,
															 	()=>{
																	// As RecordObject diffs with the end of the current frame,
																	// and this is a multi-frame operation, we need to use RegisterCompleteObjectUndo instead.
																	Undo.RegisterCompleteObjectUndo(actor, "Paint particles");
															  	},
					                                          	PaintbrushStampCallback,
															  	()=>{
																	EditorUtility.SetDirty(actor);
															  	},
					                                          	Resources.Load<Texture2D>("BrushHandle"))){
						ParticlePropertyChanged();
					}
				}else if (selectionBrush){
					if (ObiClothParticleHandles.ParticleBrush(wsPositions,faceCulling,facingCamera,brushRadius,null,
					                                          	(List<ParticleStampInfo> stampInfo,bool modified)=>{
																	foreach(ParticleStampInfo info in stampInfo){
																		if (actor.active[info.index])
																			selectionStatus[info.index] = !modified;
																	}
																},null,
																Resources.Load<Texture2D>("BrushHandle"))){
						SelectionChanged();
					}
				}else{	
					if (ObiClothParticleHandles.ParticleSelector(wsPositions,selectionStatus,faceCulling,facingCamera)){
						SelectionChanged();
					}
				}	
			}
			
			// Sceneview GUI:
			Handles.BeginGUI();			

			GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			
			if (Event.current.type == EventType.Repaint){
				uirect = GUILayout.Window(0,uirect,DrawUIWindow,"Particle editor");
				uirect.x = Screen.width/EditorGUIUtility.pixelsPerPoint - uirect.width - 10; //10 and 28 are magic values, since Screen size is not exactly right.
				uirect.y = Screen.height/EditorGUIUtility.pixelsPerPoint - uirect.height - 28;
			}

			GUILayout.Window(0,uirect,DrawUIWindow,"Particle editor");

			Handles.EndGUI();
			
		}
		
		private void ForceWindowRelayout(){
			uirect.Set(0,0,0,0);
		}

		public static bool IsParticleVisible(int index){
			switch(faceCulling){
				case FaceCulling.Back: return facingCamera[index];
				case FaceCulling.Front: return !facingCamera[index];
				default: return true;
			}
		}
		
		protected void DrawParticles(){

			if (!particleMaterial.SetPass(0))
				return;

			//because each vertex needs to be drawn as a quad.
			int particlesPerDrawcall = Constants.maxVertsPerMesh/4;
			int drawcallCount = actor.positions.Length / particlesPerDrawcall + 1;
			particlesPerDrawcall = Mathf.Min(particlesPerDrawcall,actor.positions.Length);

			int i = 0;

			for (int m = 0; m < drawcallCount; ++m){

				//Draw all cloth vertices:		
				particlesMesh.Clear();
				Vector3[] vertices = new Vector3[particlesPerDrawcall * 4];
				Vector2[] uv = new Vector2[particlesPerDrawcall * 4];
				Color[] colors = new Color[particlesPerDrawcall * 4];
				int[] triangles = new int[particlesPerDrawcall * 6];

				for(int particlesDrawn = 0; i < actor.positions.Length && particlesDrawn < particlesPerDrawcall; ++i, ++particlesDrawn)
				{
					int sortedIndex = sortedIndices[i];
	
					// skip particles not facing the camera, or inactive ones:
					if (!actor.active[sortedIndex] || !IsParticleVisible(sortedIndex)) continue;
	
					int i4 = i*4;
					int i41 = i4+1;
					int i42 = i4+2;
	                int i43 = i4+3;
	                int i6 = i*6;
					
					// get particle size in screen space:
					float size = HandleUtility.GetHandleSize(wsPositions[sortedIndex])*0.05f;
					
					// get particle color:
					Color color;

					if (selectionMask && !selectionStatus[sortedIndex])
						color = Color.gray;
					else{
						if (actor.invMasses[sortedIndex] == 0){
							color = Color.red;
						}else{
							color = Color.blue;
						}
					}
	
					color.a = facingCamera[sortedIndex] ? 1:0.5f;
					
					uv[i4] = new Vector2(0.5f,1);
					uv[i41] = new Vector2(0,1);
					uv[i42] = Vector3.zero;
					uv[i43] = new Vector2(0.5f,0);
	
					// highlight the particle if its selected:
					if (selectionStatus[sortedIndex]){
						uv[i4] = new Vector2(1,1);
						uv[i41] = new Vector2(0.5f,1);
						uv[i42] = new Vector3(0.5f,0);
	                    uv[i43] = new Vector2(1,0);
					}
					
					vertices[i4] = wsPositions[sortedIndex] + camup*size + camright*size;
					vertices[i41] = wsPositions[sortedIndex] + camup*size - camright*size;
					vertices[i42] = wsPositions[sortedIndex] - camup*size - camright*size;
					vertices[i43] = wsPositions[sortedIndex] - camup*size + camright*size;
	
					colors[i4] = color;
					colors[i41] = color;
					colors[i42] = color;
					colors[i43] = color;
	                
	                triangles[i6] = i42;
	                triangles[i6+1] = i41;
	                triangles[i6+2] = i4;
	                triangles[i6+3] = i43;
	                triangles[i6+4] = i42;
	                triangles[i6+5] = i4;
	                
	            }

	            particlesMesh.vertices = vertices;
	            particlesMesh.triangles = triangles;
	            particlesMesh.uv = uv;
				particlesMesh.colors = colors;
	            
	            Graphics.DrawMeshNow(particlesMesh,Matrix4x4.identity);
			}
        }	

		protected void DrawParticleRadii(){

			if (currentProperty == ParticleProperty.Radius){

				if (!radiiMaterial.SetPass(0))
					return;
	
				//because each vertex needs to be drawn as a quad.	
				int particlesPerDrawcall = Constants.maxVertsPerMesh/4;
				int drawcallCount = actor.positions.Length / particlesPerDrawcall + 1;
				particlesPerDrawcall = Mathf.Min(particlesPerDrawcall,actor.positions.Length);

				int i = 0;

				for (int m = 0; m < drawcallCount; ++m){

					//Draw all cloth vertices:		
					particlesMesh.Clear();
					Vector3[] vertices = new Vector3[particlesPerDrawcall * 4];
					Vector2[] uv = new Vector2[particlesPerDrawcall * 4];
					Color[] colors = new Color[particlesPerDrawcall * 4];
					int[] triangles = new int[particlesPerDrawcall * 6];
	
					for(int particlesDrawn = 0; i < actor.positions.Length && particlesDrawn < particlesPerDrawcall; ++i, ++particlesDrawn)
					{
						int sortedIndex = sortedIndices[i];
	
						// skip particles not facing the camera, inactive and unselected ones:
						if (!actor.active[sortedIndex] || 
							!selectionStatus[sortedIndex] ||
							!IsParticleVisible(sortedIndex)) continue;
		
						int i4 = i*4;
						int i41 = i4+1;
						int i42 = i4+2;
		                int i43 = i4+3;
		                int i6 = i*6;
						
						// get particle size in screen space:
						float size = actor.solidRadii[sortedIndex];
						
						// get particle color:
						Color color = Color.white;
		
						color.a = facingCamera[sortedIndex] ? 1:0.5f;
						
						uv[i4] = new Vector2(1,1);
						uv[i41] = new Vector2(0,1);
						uv[i42] = Vector3.zero;
						uv[i43] = new Vector2(1,0);
						
						vertices[i4] = wsPositions[sortedIndex] + camup*size + camright*size;
						vertices[i41] = wsPositions[sortedIndex] + camup*size - camright*size;
						vertices[i42] = wsPositions[sortedIndex] - camup*size - camright*size;
						vertices[i43] = wsPositions[sortedIndex] - camup*size + camright*size;
		
						colors[i4] = color;
						colors[i41] = color;
						colors[i42] = color;
						colors[i43] = color;
		                
		                triangles[i6] = i42;
		                triangles[i6+1] = i41;
		                triangles[i6+2] = i4;
		                triangles[i6+3] = i43;
		                triangles[i6+4] = i42;
		                triangles[i6+5] = i4;
		                
		                
		            }
		            particlesMesh.vertices = vertices;
		            particlesMesh.triangles = triangles;
		            particlesMesh.uv = uv;
					particlesMesh.colors = colors;
		            
		            Graphics.DrawMeshNow(particlesMesh,Matrix4x4.identity);
				}

			}

		}

		public virtual void UpdateParticleEditorInformation(){

			for(int i = 0; i < actor.positions.Length; i++)
			{
				if (actor.active[i]){
					wsPositions[i] = actor.transform.TransformPoint(actor.positions[i]);		
					facingCamera[i] = true;
				}
			}

		}
		
		private void SelectionChanged(){
			
			// Find out how many selected particles we have:
			selectedCount = 0;
			for(int i = 0; i < selectionStatus.Length; i++){
				if (actor.active[i] && selectionStatus[i]){
					selectedCount++;
					lastSelectedParticle = i;
				}
			}
			
			// Set initial property value:
			newProperty = GetPropertyValue(currentProperty,lastSelectedParticle);	
			
			Repaint();	
			
		}
		
		/**
		 * Called when the currenty edited property of any particle as changed.
	 	 */
		protected void ParticlePropertyChanged(){

			if (autoRangeDraw){			

				maxRangeValue = Single.MinValue;
				minRangeValue = Single.MaxValue;
				
				for(int i = 0; i < actor.invMasses.Length; i++){
					
					//Skip inactive and fixed particles:
					if (!actor.active[i]) continue;
	
					// Skip fixed particles, if the current property is mass:
					if (currentProperty == ParticleProperty.Mass && actor.invMasses[i] == 0) continue;
					
					float value = GetPropertyValue(currentProperty,i); 
					maxRangeValue = Mathf.Max(maxRangeValue,value);
					minRangeValue = Mathf.Min(minRangeValue,value);
					
				}

			}else{

				maxRangeValue = maxPropertyValue;
				minRangeValue = minPropertyValue;
				
			}

			UpdatePropertyInSolver();	

		}
		
		protected abstract void SetPropertyValue(ParticleProperty property, int index, float value);
			
		protected abstract float GetPropertyValue(ParticleProperty property, int index);

		protected virtual void UpdatePropertyInSolver(){

			switch(currentProperty){
				case ParticleProperty.Mass:
					actor.PushDataToSolver(ParticleData.INV_MASSES);
				break;
				case ParticleProperty.Radius:
					actor.PushDataToSolver(ParticleData.SOLID_RADII);
				break;
				case ParticleProperty.Layer:
					actor.PushDataToSolver(ParticleData.PHASES);
				break;
			}

		}

		protected Color GetPropertyValueGradient(float value){
			if (!Mathf.Approximately(minRangeValue, maxRangeValue))
				return valueGradient.Evaluate(Mathf.InverseLerp(minRangeValue,maxRangeValue,value));
			else return valueGradient.Evaluate(0);
		}

		protected virtual void DrawActorInfo(){
			DrawParticleRadii();
			DrawParticles();
		}
		
		/**
	 	* Callback called for each paintbrush stamp (each time the user drags the mouse, and when he first clicks down).
	 	*/ 
		private void PaintbrushStampCallback(List<ParticleStampInfo> stampInfo, bool modified){
			
			// Average and particle count for SMOOTH mode.
			float averageValue = 0;	
			int numParticles = 0;
			
			foreach(ParticleStampInfo info in stampInfo){
				
				// Skip unselected particles, if selection mask is on.
				if (selectionMask && !selectionStatus[info.index]) continue;
				
				switch(paintMode){
				case PaintBrushType.Gaussian: 
				case PaintBrushType.Pencil: 

					float currentValue = GetPropertyValue(currentProperty,info.index);
					float profile = (paintMode == PaintBrushType.Pencil) ? brushModulation : 
									ObiEditorUtils.GaussianBrushProfile(Mathf.Sqrt(info.sqrDistanceToMouse)/brushRadius,4) * brushModulation;

					if (modified){
						SetPropertyValue(currentProperty,info.index,currentValue + profile * (currentValue - newProperty) * brushOpacity);
					}else{
						SetPropertyValue(currentProperty,info.index,currentValue - profile * (currentValue - newProperty) * brushOpacity);
					}
					break;
				case PaintBrushType.Smooth:
					averageValue += GetPropertyValue(currentProperty,info.index);
					numParticles++;
					break;
				}
				
			}
			
			if (paintMode == PaintBrushType.Smooth){
				averageValue /= numParticles;
				foreach(ParticleStampInfo info in stampInfo){
					
					// Skip unselected particles, if selection mask is on.
					if (selectionMask && !selectionStatus[info.index]) continue;
					
					float currentValue = GetPropertyValue(currentProperty,info.index);
					float profile = ObiEditorUtils.GaussianBrushProfile(Mathf.Sqrt(info.sqrDistanceToMouse)/brushRadius,4) * brushModulation;

					if (modified){ //Sharpen
						SetPropertyValue(currentProperty,info.index,currentValue + profile * (currentValue - averageValue) * brushOpacity);
					}else{	//Smooth
						SetPropertyValue(currentProperty,info.index,currentValue - profile * (currentValue - averageValue) * brushOpacity);
					}
				}
			}
			
		}

		void DrawPlaybackControls(){

			//-------------------------------
			//Playback functions
			//-------------------------------

			GUI.enabled = !EditorApplication.isPlaying;
						
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("RewindButton"),"Rewind"),GUILayout.MaxHeight(24),GUILayout.Width(42))){

				isPlaying = false;
				

				if (actor.InSolver){
					actor.RemoveFromSolver(null);
					actor.Solver.ResetSimulationTime();
				}

				actor.ResetActor();
				accumulatedTime = 0;
			}
			
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("StopButton"),"Stop"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				isPlaying = false;
			}
			
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("PlayButton"),"Play"),GUILayout.MaxHeight(24),GUILayout.Width(42))){

				if (!actor.InSolver) 
					actor.AddToSolver(null);

				lastFrameTime = Time.realtimeSinceStartup;
				isPlaying = true;

			}
			
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("StepButton"),"Step forward"),GUILayout.MaxHeight(24),GUILayout.Width(42))){

				isPlaying = false;

				if (!actor.InSolver) 
					actor.AddToSolver(null);

				if (actor.InSolver){
					actor.Solver.AccumulateSimulationTime(Time.fixedDeltaTime);
					actor.Solver.SimulateStep(Time.fixedDeltaTime);
					actor.Solver.EndFrame(Time.fixedDeltaTime);
				}

			}
			
			GUILayout.EndHorizontal();

			GUI.enabled = true;
			
		}

		void DrawSelectionControls(){
			
			//GUILayout.Label(selectedCount+" particle(s) selected");
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("InvertButton") ,"Invert selection"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				for(int i = 0; i < selectionStatus.Length; i++){
					if (actor.active[i])
						selectionStatus[i] = !selectionStatus[i];
				}
				SelectionChanged();
			}

			GUI.enabled = selectedCount > 0;
			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("ClearButton") ,"Clear selection"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				for(int i = 0; i < selectionStatus.Length; i++)
					selectionStatus[i] = false;
				SelectionChanged();
			}
			GUI.enabled = true;

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("InvertPinButton") ,"Select fixed"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				for(int i = 0; i < actor.invMasses.Length; i++){
					if (actor.active[i] && actor.invMasses[i] == 0)
						selectionStatus[i] = true;
				}
				SelectionChanged();
			}

			EditorGUI.BeginChangeCheck();
			selectionBrush = GUILayout.Toggle(selectionBrush,new GUIContent(Resources.Load<Texture2D>("Paint_brush_icon") ,"Selection brush"),GUI.skin.FindStyle("Button"),GUILayout.MaxHeight(24),GUILayout.Width(42));
			if (EditorGUI.EndChangeCheck()){
				if (selectionBrush){
					paintBrush = false;
				}
				ForceWindowRelayout();
			}

			GUILayout.EndHorizontal();

			if (selectionBrush){
				GUILayout.BeginHorizontal();
				GUILayout.Label("Radius");
				brushRadius = EditorGUILayout.Slider(brushRadius,5,200);
				GUILayout.EndHorizontal();
			}
		}

		void DrawPropertyControls(){

			GUILayout.BeginHorizontal();	

			// Property dropdown:
			EditorGUI.BeginChangeCheck();
			currentProperty = (ParticleProperty) EditorGUILayout.Popup(currentProperty,particlePropertyNames.ToArray());
			if (EditorGUI.EndChangeCheck()){
				newProperty = GetPropertyValue(currentProperty,lastSelectedParticle);
				ParticlePropertyChanged();
			}

			// Property value:
			EditorGUI.showMixedValue = false;
			float selectionProperty = GetPropertyValue(currentProperty,lastSelectedParticle);
			for(int i = 0; i < selectionStatus.Length; i++){
				if (selectionStatus[i] && !Mathf.Approximately(GetPropertyValue(currentProperty,i), selectionProperty)){
					EditorGUI.showMixedValue = true;
				}	
			}
			
			EditorGUI.BeginChangeCheck();
			newProperty = EditorGUILayout.FloatField(newProperty,GUILayout.Width(88));
			if (EditorGUI.EndChangeCheck()){
			
				// If we are not in paint mode, allow instant change of particle properties:
				if (!paintBrush){
					Undo.RecordObject(actor, "Set particle property");
					for(int i = 0; i < selectionStatus.Length; i++){
						if (!selectionStatus[i]) continue;
						SetPropertyValue(currentProperty,i,newProperty);
					}
					ParticlePropertyChanged();
				}
			}

			EditorGUI.showMixedValue = false;
			
			GUILayout.EndHorizontal();	

			GUILayout.BeginHorizontal();	

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("FillButton") ,"Fill property value"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				Undo.RecordObject(actor, "Property fill");
				for(int i = 0; i < selectionStatus.Length; i++){
					// Skip unselected particles, if selection mask is on.
					if (selectionMask && !selectionStatus[i]) continue;

					SetPropertyValue(currentProperty,i,newProperty);
				}
				ParticlePropertyChanged();
			}

			selectionMask = GUILayout.Toggle(selectionMask,new GUIContent(Resources.Load<Texture2D>("MaskButton") ,"Selection mask"),GUI.skin.FindStyle("Button"),GUILayout.MaxHeight(24),GUILayout.Width(42));
			
			EditorGUI.BeginChangeCheck();
			textureProperties = GUILayout.Toggle(textureProperties,new GUIContent(Resources.Load<Texture2D>("TextureButton")),GUI.skin.FindStyle("Button"),GUILayout.MaxHeight(24),GUILayout.Width(42));
			if (EditorGUI.EndChangeCheck()){
				if (textureProperties){
					paintBrush = false;
				}
				ForceWindowRelayout();
			}

			EditorGUI.BeginChangeCheck();
			paintBrush = GUILayout.Toggle(paintBrush,new GUIContent(Resources.Load<Texture2D>("PaintButton") ,"Paint brush"),GUI.skin.FindStyle("Button"),GUILayout.MaxHeight(24),GUILayout.Width(42));
			if (EditorGUI.EndChangeCheck()){
				if (paintBrush){
					selectionBrush = false;
					textureProperties = false;
				}
				ForceWindowRelayout();
			}

			GUILayout.EndHorizontal();

			if (paintBrush){

				GUILayout.BeginHorizontal();
				if (GUILayout.Toggle(paintMode == PaintBrushType.Gaussian,new GUIContent(Resources.Load<Texture2D>("GaussianButton") ,"Soft brush"),GUI.skin.FindStyle("ButtonLeft"),GUILayout.MaxHeight(28)))
					paintMode = PaintBrushType.Gaussian;
				if (GUILayout.Toggle(paintMode == PaintBrushType.Pencil,new GUIContent(Resources.Load<Texture2D>("PencilButton") ,"Pencil"),GUI.skin.FindStyle("ButtonMid"),GUILayout.MaxHeight(28)))
					paintMode = PaintBrushType.Pencil;
				if (GUILayout.Toggle(paintMode == PaintBrushType.Smooth,new GUIContent(Resources.Load<Texture2D>("SmoothButton") ,"Smooth"),GUI.skin.FindStyle("ButtonRight"),GUILayout.MaxHeight(28)))
					paintMode = PaintBrushType.Smooth;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Radius");
				brushRadius = EditorGUILayout.Slider(brushRadius,5,200);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Opacity");
				brushOpacity = EditorGUILayout.Slider(brushOpacity,0,1);
				GUILayout.EndHorizontal();

				EditorGUI.BeginChangeCheck();

				// auto range visualization:
				autoRangeDraw = GUILayout.Toggle(autoRangeDraw,"Auto range visualization");
			
				if (!autoRangeDraw){
					GUILayout.BeginHorizontal();
					GUILayout.Label("Min");
					GUILayout.FlexibleSpace();
					minPropertyValue = EditorGUILayout.FloatField(minPropertyValue,GUILayout.Width(EditorGUIUtility.fieldWidth));
					GUILayout.FlexibleSpace();
					GUILayout.Label("Max");
					GUILayout.FlexibleSpace();
					maxPropertyValue = EditorGUILayout.FloatField(maxPropertyValue,GUILayout.Width(EditorGUIUtility.fieldWidth));
					GUILayout.EndHorizontal();
				}
				
				if (EditorGUI.EndChangeCheck()){
					ParticlePropertyChanged();
					ForceWindowRelayout();
				}
			}

			if (textureProperties){
				GUILayout.BeginHorizontal();
				float oldLabelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 40;
				GUILayout.Label("Source");
				propertyTexture = (Texture2D)EditorGUILayout.ObjectField("",propertyTexture, typeof(Texture2D),false);
				EditorGUIUtility.labelWidth = oldLabelWidth;	
				GUILayout.EndHorizontal();
	
				GUILayout.BeginHorizontal();
				GUILayout.Label("Source channel");
				GUILayout.FlexibleSpace();
				textureChannel = (TextureChannel) EditorGUILayout.EnumPopup(textureChannel);
				GUILayout.EndHorizontal();	

				EditorGUI.BeginChangeCheck();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Min value");
				GUILayout.FlexibleSpace();
				minPropertyValue = EditorGUILayout.FloatField(minPropertyValue,GUILayout.Width(EditorGUIUtility.fieldWidth));
				GUILayout.EndHorizontal();
			
				GUILayout.BeginHorizontal();
				GUILayout.Label("Max value");
				GUILayout.FlexibleSpace();
				maxPropertyValue = EditorGUILayout.FloatField(maxPropertyValue,GUILayout.Width(EditorGUIUtility.fieldWidth));
				GUILayout.EndHorizontal();		
	
				if (GUILayout.Button("Load property")){
					Undo.RecordObject(actor, "Load particle property");
					if (!actor.ReadParticlePropertyFromTexture(propertyTexture,(int i,Color color) =>{
						if (!selectionMask || selectionStatus[i])
							SetPropertyValue(currentProperty,i,minPropertyValue + color[(int)textureChannel] * (maxPropertyValue - minPropertyValue));
						})){
						EditorUtility.DisplayDialog("Invalid texture","The texture is either null or not readable.","Ok");
					}
					ParticlePropertyChanged();
				}

				// auto range visualization:
				autoRangeDraw = GUILayout.Toggle(autoRangeDraw,"Auto range visualization");
				
				if (EditorGUI.EndChangeCheck()){
					ParticlePropertyChanged();
				}
			}


		}

		void DrawFixControls(){

			GUILayout.BeginHorizontal();

			GUI.enabled = selectedCount > 0;

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("PinButton") ,"Fix selected"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				Undo.RecordObject(actor, "Fix particles");
				for(int i = 0; i < selectionStatus.Length; i++){
					if (selectionStatus[i]){
						if (actor.invMasses[i] != 0){	
							SetPropertyValue(ParticleProperty.Mass,i,Mathf.Infinity);
							newProperty = GetPropertyValue(currentProperty,i);
							actor.velocities[i] = Vector3.zero;
						}
					}
				}
				actor.PushDataToSolver(ParticleData.INV_MASSES | ParticleData.VELOCITIES);
			}

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("UnpinButton") ,"Unfix selected"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				Undo.RecordObject(actor, "Unfix particles");
				for(int i = 0; i < selectionStatus.Length; i++){
					if (selectionStatus[i]){
						if (actor.invMasses[i] == 0){	
							SetPropertyValue(ParticleProperty.Mass,i,1);
							newProperty = GetPropertyValue(currentProperty,i);
						}
					}
				}
				actor.PushDataToSolver(ParticleData.INV_MASSES);
			}

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("HandleButton") ,"Create handle"),GUILayout.MaxHeight(24),GUILayout.Width(42))){

				// Create the handle:
				GameObject c = new GameObject("Obi Handle");
				Undo.RegisterCreatedObjectUndo(c,"Create Obi Particle Handle");
				ObiParticleHandle handle = c.AddComponent<ObiParticleHandle>();
				handle.Actor = actor;

				// Calculate position of handle from average of particle positions:
				Vector3 average = Vector3.zero;
				for(int i = 0; i < selectionStatus.Length; i++){
					if (selectionStatus[i]){
						average += wsPositions[i];
					}
				}

				c.transform.position = average / selectedCount;

				// Add the selected particles to the handle:
				for(int i = 0; i < selectionStatus.Length; i++){
					if (selectionStatus[i]){
						handle.AddParticle(i,wsPositions[i],actor.invMasses[i]);
					}
				}

			}
			GUI.enabled = true;

			if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("BackfacesButton") ,"Show backfaces"),GUI.skin.FindStyle("Button"),GUILayout.MaxHeight(24),GUILayout.Width(42))){
				faceCulling = (FaceCulling) (((int)faceCulling + 1)%3);
			}

			GUILayout.EndHorizontal();
		}
		
		/**
	 	* Draws a window with cloth tools:
	 	*/
		void DrawUIWindow(int windowID) {
			
			DrawPlaybackControls();

			GUILayout.Box("",separatorLine);

			DrawSelectionControls();

			GUILayout.Box("",separatorLine);

			DrawFixControls();

			GUILayout.Box("",separatorLine);

			DrawPropertyControls();
			
		}
		
		void OnPlayModeStateChanged()
		{
			//Prevent the user from going into play mode while we are doing stuff:
			if (routine != null && !routine.IsDone && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.isPlaying = false;
			}
		}
		
		void Update () {

			if (isPlaying && actor.InSolver){
				
				float deltaTime = Mathf.Min(Time.realtimeSinceStartup - lastFrameTime, Time.maximumDeltaTime);

				accumulatedTime += deltaTime;
				actor.Solver.AccumulateSimulationTime(deltaTime);

				while (accumulatedTime >= Time.fixedDeltaTime){
					actor.Solver.SimulateStep(Time.fixedDeltaTime);
					accumulatedTime -= Time.fixedDeltaTime;
				}

				actor.Solver.EndFrame(Time.fixedDeltaTime);

				lastFrameTime = Time.realtimeSinceStartup;
			}

		}
		
	}
}

