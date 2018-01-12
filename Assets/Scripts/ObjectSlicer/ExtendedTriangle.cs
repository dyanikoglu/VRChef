/*
 * Author: David Arayan
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExtendedTriangle
{
    private readonly Vector3 m_pos_a;
    private readonly Vector3 m_pos_b;
    private readonly Vector3 m_pos_c;

    private readonly Vector2 m_uv_a;
    private readonly Vector2 m_uv_b;
    private readonly Vector2 m_uv_c;

    public ExtendedTriangle(Vector3 posa,
                    Vector3 posb,
                    Vector3 posc) : this(posa, posb, posc, Vector2.zero, Vector2.zero, Vector2.zero) { }

    public ExtendedTriangle(Vector3 posa,
                    Vector3 posb,
                    Vector3 posc,
                    Vector2 uva,
                    Vector2 uvb,
                    Vector2 uvc)
    {
        this.m_pos_a = posa;
        this.m_pos_b = posb;
        this.m_pos_c = posc;

        this.m_uv_a = uva;
        this.m_uv_b = uvb;
        this.m_uv_c = uvc;
    }

    public Vector3 PositionA
    {
        get { return this.m_pos_a; }
    }

    public Vector3 PositionB
    {
        get { return this.m_pos_b; }
    }

    public Vector3 PositionC
    {
        get { return this.m_pos_c; }
    }

    public Vector2 UvA
    {
        get { return this.m_uv_a; }
    }

    public Vector2 UvB
    {
        get { return this.m_uv_b; }
    }

    public Vector2 UvC
    {
        get { return this.m_uv_c; }
    }

    public Vector3 Barycentric(Vector3 p)
    {
        Vector3 a = m_pos_a;
        Vector3 b = m_pos_b;
        Vector3 c = m_pos_c;

        Vector3 m = Vector3.Cross(b - a, c - a);

        float nu;
        float nv;
        float ood;

        float x = Mathf.Abs(m.x);
        float y = Mathf.Abs(m.y);
        float z = Mathf.Abs(m.z);

        if (x >= y && x >= z)
        {
            nu = IntersectionProcessor.TriArea2D(p.y, p.z, b.y, b.z, c.y, c.z);
            nv = IntersectionProcessor.TriArea2D(p.y, p.z, c.y, c.z, a.y, a.z);
            ood = 1.0f / m.x;
        }
        else if (y >= x && y >= z)
        {
            nu = IntersectionProcessor.TriArea2D(p.x, p.z, b.x, b.z, c.x, c.z);
            nv = IntersectionProcessor.TriArea2D(p.x, p.z, c.x, c.z, a.x, a.z);
            ood = 1.0f / -m.y;
        }
        else
        {
            nu = IntersectionProcessor.TriArea2D(p.x, p.y, b.x, b.y, c.x, c.y);
            nv = IntersectionProcessor.TriArea2D(p.x, p.y, c.x, c.y, a.x, a.y);
            ood = 1.0f / m.z;
        }

        float u = nu * ood;
        float v = nv * ood;
        float w = 1.0f - u - v;

        return new Vector3(u, v, w);
    }

    public Vector2 GenerateUVCoords(Vector3 pt)
    {
        Vector3 weights = Barycentric(pt);

        return (weights.x * m_uv_a) + (weights.y * m_uv_b) + (weights.z * m_uv_c);
    }


    public bool Split(ExtendedPlane pl, IntersectionResult result)
    {
        IntersectionProcessor.Intersect(pl, this, ref result);

        return result.IsValid;
    }

    public bool IsCW()
    {
        return SignedSquare(m_pos_a, m_pos_b, m_pos_c) >= -0.001f;
    }


    public static float SignedSquare(Vector3 a, Vector3 b, Vector3 c)
    {
        return (a.x * (b.y * c.z - b.z * c.y) -
                a.y * (b.x * c.z - b.z * c.x) +
                a.z * (b.x * c.y - b.y * c.x));
    }
}