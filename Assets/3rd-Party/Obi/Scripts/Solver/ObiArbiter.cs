using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi
{
	/**
	 * ObiArbiter contains static methods to synchronize the update cycle of several solvers.
	 */
	public class ObiArbiter
	{
		private static List<ObiSolver> solvers = new List<ObiSolver>(); 
		private static int solverCounter = 0;

		private static int profileThrottle = 30;
		private static int stepCounter = 0; 
		private static bool frameStarted = false;

		public static event EventHandler OnFrameStart;
		public static event EventHandler OnFrameEnd;

		public static void RegisterSolver (ObiSolver solver)
		{
			if (solver != null)
				solvers.Add(solver);
		}

		public static void UnregisterSolver (ObiSolver solver)
		{
			if (solver != null)
				solvers.Remove(solver);
		}

		public static void FrameStart()
		{
			if (!frameStarted){
				frameStarted = true;
				if (OnFrameStart != null)
					OnFrameStart(null,null);
				Oni.SignalFrameStart();
			}	
		}

		public static double FrameEnd()
		{
			return Oni.SignalFrameEnd();
		}

		public static Oni.ProfileInfo[] GetProfileInfo()
		{
			int count = Oni.GetProfilingInfoCount();
			Oni.ProfileInfo[] info = new Oni.ProfileInfo[count];
			Oni.GetProfilingInfo(info,count);
			return info;
		}

		/**
		 * When all solvers have called this, it
		 * waits until all solver update tasks have been finished.
		 */
		public static void WaitForAllSolvers()
		{
			// Increase solver counter:
			solverCounter++;
		
			// If all solvers want to wait, we're done.
			if (solverCounter >= solvers.Count){

				solverCounter = 0;

				Oni.WaitForAllTasks(); 

				stepCounter--;
				if (stepCounter <= 0)
				{
					ObiProfiler.frameDuration = FrameEnd();
					ObiProfiler.info = GetProfileInfo();
					stepCounter = profileThrottle;
				}

				if (OnFrameEnd != null)
					OnFrameEnd(null,null);

				// Notify solvers that they've all completed this simulation step:
				foreach(ObiSolver s in solvers){
					s.AllSolversStepEnd();
				}

				frameStarted = false;
			}

		}
	}
}

