using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

	/**
	 * Component that allows to generate baked caches from solver simulations, that you can play back later. This
	 * allows to save performance for non-interactive simulations.
	 */
	[ExecuteInEditMode]
	[RequireComponent(typeof(ObiSolver))]
	public class ObiParticleBaker : MonoBehaviour {
	
		public ObiParticleCache cache;
		public float playhead = 0;

		public int frameSkip = 8;
		public int fixedBakeFramerate = 60;
		public bool interpolate = true;
		public bool loopPlayback = true;
		public bool bakeOnAwake = false;
		public bool playOnAwake = false;

		private bool baking = false;
		private bool playing = false;
		private bool paused = false;
		private int framesToSkip = 0;
		private ObiSolver solver = null;
		private ObiParticleCache.Frame frame = new ObiParticleCache.Frame();

		public bool Baking{
			get{return baking;}
			set{baking = value;
				if (baking){
					Time.captureFramerate = Mathf.Max(0,fixedBakeFramerate);
					playing = false;
					solver.simulate = true;
					solver.RequireRenderablePositions();
				}else{
					framesToSkip = 0;
					Time.captureFramerate = 0;
					solver.RelinquishRenderablePositions();
				}	
			}
		}

		public bool Playing{	
			get{return playing;}
			set{playing = value;
				solver.simulate = !playing;
				if (playing)
					baking = false;}
		}

		public bool Paused{
			get{return paused;}
			set{paused = value;}
		}

		// Use this for initialization
		void Awake () {
			solver = GetComponent<ObiSolver>();

			// Only bake/play on awake outside editor.
			if (Application.isPlaying){
				if (bakeOnAwake){
					playhead = 0;
					Baking = true;
				}else if (playOnAwake){
					playhead = 0;
					Playing = true;
				}
			}
		}
		
		void OnEnable(){
			solver.OnFrameEnd += Solver_OnFrameEnd;
			solver.OnBeforeActorsFrameEnd += Solver_OnBeforeActorsFrameEnd;
		}

		void OnDisable(){
			Baking = false;
			solver.OnFrameEnd -= Solver_OnFrameEnd;
			solver.OnBeforeActorsFrameEnd -= Solver_OnBeforeActorsFrameEnd;
		}

		void Solver_OnFrameEnd (object sender, System.EventArgs e)
		{
			if (cache != null && !playing && baking){
		
				playhead += Time.deltaTime;
		
				// Add frame:
				if (framesToSkip <= 0){
					BakeFrame(playhead);	
					framesToSkip = frameSkip;
				}else{
					framesToSkip--;
				}
				
			}
		}

		void Solver_OnBeforeActorsFrameEnd (object sender, System.EventArgs e)
		{
			if (cache != null && playing){

				if (!paused){

					playhead += Time.deltaTime;

					if (loopPlayback)
						playhead = cache.Duration == 0 ? 0 : playhead % cache.Duration;
					else if (playhead > cache.Duration)
						playhead = cache.Duration;
				}

				PlaybackFrame(playhead);
			}
		}

		public void BakeFrame(float time){

			if (cache == null)
				return;

			ObiParticleCache.Frame frame = new ObiParticleCache.Frame();

			frame.time = time;

			for (int i = 0; i < solver.renderablePositions.Length; ++i){

				// If the particle has not been allocated or is inactive, skip it.
				ObiSolver.ParticleInActor pa = solver.particleToActor[i];
				if (pa == null || !pa.actor.active[pa.indexInActor]) 
					continue;
				
				frame.indices.Add(i);

				if (cache.localSpace)
					frame.positions.Add(solver.transform.InverseTransformPoint(solver.renderablePositions[i]));
				else
					frame.positions.Add(solver.renderablePositions[i]);
					
			}

			cache.AddFrame(frame);

		}
		
		void PlaybackFrame(float time){

			if (cache == null || cache.Duration == 0) 
				return;

			// Get current frame from cache:
			cache.GetFrame(time,interpolate, ref frame);

			if (solver.AllocParticleCount < frame.indices.Count){
				Debug.LogError("The ObiSolver doesn't have enough allocated particles to playback this cache.");
				Playing = false;
				return;
			}

			Matrix4x4 s2world = cache.localSpace ? solver.transform.localToWorldMatrix : Matrix4x4.identity;

			// Apply current frame:
			for (int i = 0; i < frame.indices.Count; ++i){	
				if (frame.indices[i] >= 0 && frame.indices[i] < solver.renderablePositions.Length){

					solver.renderablePositions[frame.indices[i]] = s2world.MultiplyPoint3x4(frame.positions[i]);	

				}
			}

			Oni.SetParticlePositions(solver.OniSolver,solver.renderablePositions,solver.renderablePositions.Length,0);
			solver.UpdateActiveParticles();

		}

	}
}
