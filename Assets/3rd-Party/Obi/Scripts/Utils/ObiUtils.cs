using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Obi
{

public static class Constants{
	public const int maxVertsPerMesh = 65000;
}

public static class ObiUtils
{
	public static void DrawArrowGizmo(float bodyLenght, float bodyWidth, float headLenght, float headWidth){

		float halfBodyLenght = bodyLenght*0.5f;
		float halfBodyWidth = bodyWidth*0.5f;

		// arrow body:
		Gizmos.DrawLine(new Vector3(halfBodyWidth,0,-halfBodyLenght),new Vector3(halfBodyWidth,0,halfBodyLenght));
		Gizmos.DrawLine(new Vector3(-halfBodyWidth,0,-halfBodyLenght),new Vector3(-halfBodyWidth,0,halfBodyLenght));
		Gizmos.DrawLine(new Vector3(-halfBodyWidth,0,-halfBodyLenght),new Vector3(halfBodyWidth,0,-halfBodyLenght));

		// arrow head:
		Gizmos.DrawLine(new Vector3(halfBodyWidth,0,halfBodyLenght),new Vector3(headWidth,0,halfBodyLenght));
		Gizmos.DrawLine(new Vector3(-halfBodyWidth,0,halfBodyLenght),new Vector3(-headWidth,0,halfBodyLenght));
		Gizmos.DrawLine(new Vector3(0,0,halfBodyLenght+headLenght),new Vector3(headWidth,0,halfBodyLenght));
		Gizmos.DrawLine(new Vector3(0,0,halfBodyLenght+headLenght),new Vector3(-headWidth,0,halfBodyLenght));
	}

	public static void ArrayFill<T>(T[] arrayToFill, T[] fillValue)
	{
	    if (fillValue.Length <= arrayToFill.Length)
	    {
		    // set the initial array value
		    Array.Copy(fillValue, arrayToFill, fillValue.Length);
		
		    int arrayToFillHalfLength = arrayToFill.Length / 2;
		
		    for (int i = fillValue.Length; i < arrayToFill.Length; i *= 2)
		    {
		        int copyLength = i;
		        if (i > arrayToFillHalfLength)
		        {
		            copyLength = arrayToFill.Length - i;
		        }
		
		        Array.Copy(arrayToFill, 0, arrayToFill, i, copyLength);
		    }
		}
	}

	public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
	{
	    if (indexA != indexB && 
			indexB > -1 && indexB < list.Count &&
			indexA > -1 && indexA < list.Count)
	    {
	        T tmp = list[indexA];
	        list[indexA] = list[indexB];
	        list[indexB] = tmp;
	    }
	    return list;
	}

	/**
	 * Same as AddRange for Lists, but for arrays which are conveniently a blittable type.
     */
	public static void AddRange<T>(ref T[] array, T[] other){

		if (array == null || other == null) return;

		int blitStart = array.Length;
		System.Array.Resize(ref array,array.Length + other.Length);
		other.CopyTo(array,blitStart);

	}

	/**
	 * Same as RemoveRange for Lists, but for arrays which are conveniently a blittable type.
     */
	public static void RemoveRange<T>(ref T[] array, int index, int count){

		if (array == null) return;
	
		if (index < 0 || count < 0){
			throw new System.ArgumentOutOfRangeException("Index and/or count are < 0.");
		}

		if (index + count > array.Length){
			throw new System.ArgumentException("Index and count do not denote a valid range of elements.");
		}

		for (int i = index; i < array.Length - count; i++){
			array.SetValue(array.GetValue(i + count),i);
		}
		System.Array.Resize (ref array,array.Length - count);

	}

	public static Bounds Transform(this Bounds b, Matrix4x4 m)
	{
	    var xa = m.GetColumn(0) * b.min.x;
	    var xb = m.GetColumn(0) * b.max.x;
	 
	    var ya = m.GetColumn(1) * b.min.y;
	    var yb = m.GetColumn(1) * b.max.y;
	 
	    var za = m.GetColumn(2) * b.min.z;
	    var zb = m.GetColumn(2) * b.max.z;
	 
		Bounds result = new Bounds();
		Vector3 pos = m.GetColumn(3);
		result.SetMinMax(Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + pos,
						 Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + pos);
					

		return result;
	}

	public static float Remap (this float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	/**
	 * Modulo operator that also follows intuition for negative arguments. That is , -1 mod 3 = 2, not -1.
	 */
	public static float Mod(float a,float b)
	{
		return a - b * Mathf.Floor(a / b);
	}

	/**
	 * Calculates the area of a triangle.
	 */
	public static float TriangleArea(Vector3 p1, Vector3 p2, Vector3 p3){
		return Mathf.Sqrt(Vector3.Cross(p2-p1,p3-p1).sqrMagnitude) / 2f;
	}
}
}

