/*
 * Author: David Arayan
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public enum SideOfPlane {
		UP,
		DOWN,
		ON
	}

public struct ExtendedPlane
{
    private Vector3 m_normal;
    private float m_dist;

    public ExtendedPlane(Vector3 pos, Vector3 norm)
    {
        this.m_normal = norm;
        this.m_dist = Vector3.Dot(norm, pos);
    }

    public ExtendedPlane(Vector3 norm, float dot)
    {
        this.m_normal = norm;
        this.m_dist = dot;
    }

    public void Compute(Vector3 pos, Vector3 norm)
    {
        this.m_normal = norm;
        this.m_dist = Vector3.Dot(norm, pos);
    }

    public void Compute(Transform trans)
    {
        Compute(trans.position, trans.up);
    }

    public void Compute(GameObject obj)
    {
        Compute(obj.transform);
    }

    public Vector3 Normal
    {
        get { return this.m_normal; }
    }

    public float Dist
    {
        get { return this.m_dist; }
    }

    public SideOfPlane SideOf(Vector3 pt)
    {
        float result = Vector3.Dot(m_normal, pt) - m_dist;

        if (result > 0.001f)
        {
            return SideOfPlane.UP;
        }

        if (result < 0.001f)
        {
            return SideOfPlane.DOWN;
        }

        return SideOfPlane.ON;
    }

}