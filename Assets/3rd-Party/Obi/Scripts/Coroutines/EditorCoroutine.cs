using UnityEngine;
#if (UNITY_EDITOR)
	using UnityEditor;
#endif
using System.Collections;

namespace Obi
{

/**
 * Implementation of coroutines for the editor. These are just like regular coroutines, except they can be started from
 * any editor class, can be explicitly stopped, and you can look at their partial results.
 */
public class EditorCoroutine
{
	public static EditorCoroutine StartCoroutine( IEnumerator _routine )
	{
		EditorCoroutine coroutine = new EditorCoroutine(_routine);
		coroutine.Start();
		return coroutine;
	}
	
	readonly IEnumerator routine;

	object result;
	public object Result{
		get{return result;}
	}

	bool isDone;
	public bool IsDone{
		get{return isDone;}
	}

	EditorCoroutine( IEnumerator _routine )
	{
		routine = _routine;
	}
	
	void Start()
	{
		isDone = false;
		result = null;
		#if (UNITY_EDITOR)
			EditorApplication.update += Update;
		#endif
		Update ();
	}

	public void Stop()
	{
		isDone = true;
		#if (UNITY_EDITOR)
			EditorApplication.update -= Update;
		#endif
	}
	
	void Update()
	{
		bool next = routine.MoveNext();
		result = routine.Current;

		if (!next)
		{
			Stop();
		}
	}

	public static void ShowCoroutineProgressBar(string title, EditorCoroutine coroutine){
		
		#if (UNITY_EDITOR)
		if (coroutine != null && !coroutine.IsDone){
			CoroutineJob.ProgressInfo progressInfo = coroutine.Result as CoroutineJob.ProgressInfo;
			if (progressInfo != null){
				if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo.userReadableInfo, progressInfo.progress)){
					coroutine.Stop();
				}
			}
		}else{
			EditorUtility.ClearProgressBar();
		}
		#endif

	}
		
}
}