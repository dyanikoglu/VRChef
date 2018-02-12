using System;
using UnityEngine;

namespace Obi
{
	public class ObiProfiler : MonoBehaviour
	{
		public GUISkin skin;
		public bool showPercentages = false;
		public int maxVisibleThreads = 4;

		public static Oni.ProfileInfo[] info;
		public static double frameDuration;

		private float zoom = 1;
		private Vector2 scrollPosition = Vector2.zero;
		private int numThreads = 1;

		public void OnEnable(){
			Oni.EnableProfiler(true);
			numThreads = Oni.GetMaxSystemConcurrency();
		}

		public void OnDisable(){
			Oni.EnableProfiler(false);
		}

		public void OnGUI()
		{
			GUI.skin = skin;
			int toolbarHeight = 20;
			int threadHeight = 20;
	
			GUI.BeginGroup(new Rect(0,0,Screen.width,toolbarHeight),"","Box");

			GUI.Label(new Rect(5,0,50,toolbarHeight),"Zoom:");
			zoom = GUI.HorizontalSlider(new Rect(50,5,100,toolbarHeight),zoom,0.005f,1);
			GUI.Label(new Rect(Screen.width - 100,0,100,toolbarHeight),(frameDuration/1000.0f).ToString("0.###") + " ms/step");

			GUI.EndGroup();

			scrollPosition = GUI.BeginScrollView(new Rect(0, toolbarHeight, Screen.width, Mathf.Min(maxVisibleThreads,numThreads) * threadHeight+10), scrollPosition, 
												 new Rect(0, 0, Screen.width / zoom, numThreads * threadHeight)); // height depends on amount of threads.

			foreach (Oni.ProfileInfo i in info)
			{	
				GUI.color = Color.green;

				int taskStart = (int) (i.start / frameDuration * (Screen.width-10) / zoom);
				int taskEnd = (int) (i.end / frameDuration * (Screen.width-10) / zoom);
			
				string name;
				if (showPercentages)
				{
					double pctg = (i.end-i.start)/frameDuration*100;
					name = i.name + " ("+pctg.ToString("0.#")+"%)"; 
				}
				else{
					double ms = (i.end-i.start)/1000.0f;
					name = i.name + " ("+ms.ToString("0.##")+"ms)"; 
				}

				GUI.Box(new Rect(taskStart,  i.threadID*threadHeight,taskEnd-taskStart, threadHeight),name,"thread");
			}

			GUI.EndScrollView();
		}
	}
}

