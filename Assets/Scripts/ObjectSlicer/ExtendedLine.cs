/*
 * Author: David Arayan
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExtendedLine {
    private readonly Vector3 m_pos_a;
    private readonly Vector3 m_pos_b;

    public ExtendedLine(Vector3 pta, Vector3 ptb)
    {
        this.m_pos_a = pta;
        this.m_pos_b = ptb;
    }

    public float Dist
    {
        get { return Vector3.Distance(this.m_pos_a, this.m_pos_b); }
    }

    public float DistSq
    {
        get { return (this.m_pos_a - this.m_pos_b).sqrMagnitude; }
    }

    public Vector3 PositionA
    {
        get { return this.m_pos_a; }
    }

    public Vector3 PositionB
    {
        get { return this.m_pos_b; }
    }
}