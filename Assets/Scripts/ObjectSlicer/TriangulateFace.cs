using System;
using System.Collections.Generic;
using UnityEngine;

public class TriangulateFace {

    // Structure required for using MonotoneChain algorithm for building a hull from given vertex list.
    internal struct Mapped2D
    {
        private readonly Vector3 original;
        private readonly Vector2 mapped;

        public Mapped2D(Vector3 newOriginal, Vector3 u, Vector3 v)
        {
            this.original = newOriginal;
            this.mapped = new Vector2(Vector3.Dot(newOriginal, u), Vector3.Dot(newOriginal, v));
        }

        public Vector2 MappedValue
        {
            get { return this.mapped; }
        }

        public Vector3 OriginalValue
        {
            get { return this.original; }
        }
    }

    // Monotonechain algorithm. ( O(nlgn) worst case time )
    // Source: Wikipedia
    public static bool MonotoneChain(List<Vector3> vertices, Vector3 normal, out Vector3[] verts, out int[] indices, out Vector2[] uv)
    {
        int count = vertices.Count;

        if (count < 3)
        {
            verts = null;
            indices = null;
            uv = null;

            return false;
        }

        Vector3 r = Mathf.Abs(normal.x) > Mathf.Abs(normal.y) ? new Vector3(0, 1, 0) : new Vector3(1, 0, 0);

        Vector3 v = Vector3.Normalize(Vector3.Cross(r, normal));
        Vector3 u = Vector3.Cross(normal, v);

        Mapped2D[] mapped = new Mapped2D[count];

        float maxDivX = 0.0f;
        float maxDivY = 0.0f;

        for (int i = 0; i < count; i++)
        {
            Vector3 vertToAdd = vertices[i];

            Mapped2D newMappedValue = new Mapped2D(vertToAdd, u, v);
            Vector2 mapVal = newMappedValue.MappedValue;

            maxDivX = Mathf.Max(maxDivX, mapVal.x);
            maxDivY = Mathf.Max(maxDivY, mapVal.y);

            mapped[i] = newMappedValue;
        }

        Array.Sort<Mapped2D>(mapped, (a, b) =>
        {
            Vector2 x = a.MappedValue;
            Vector2 p = b.MappedValue;

            return (x.x < p.x || (x.x == p.x && x.y < p.y)) ? -1 : 1;
        });

        Mapped2D[] hulls = new Mapped2D[count + 1];

        int k = 0;

        for (int i = 0; i < count; i++)
        {
            while (k >= 2)
            {
                Vector2 mA = hulls[k - 2].MappedValue;
                Vector2 mB = hulls[k - 1].MappedValue;
                Vector2 mC = mapped[i].MappedValue;

                if (IntersectionProcessor.TriArea2D(mA.x, mA.y, mB.x, mB.y, mC.x, mC.y) > 0.0f)
                {
                    break;
                }

                k--;
            }

            hulls[k++] = mapped[i];
        }

        for (int i = count - 2, t = k + 1; i >= 0; i--)
        {
            while (k >= t)
            {
                Vector2 mA = hulls[k - 2].MappedValue;
                Vector2 mB = hulls[k - 1].MappedValue;
                Vector2 mC = mapped[i].MappedValue;

                if (IntersectionProcessor.TriArea2D(mA.x, mA.y, mB.x, mB.y, mC.x, mC.y) > 0.0f)
                {
                    break;
                }

                k--;
            }

            hulls[k++] = mapped[i];
        }

        int vertCount = k - 1;

        if (vertCount < 3)
        {
            verts = null;
            indices = null;
            uv = null;

            return false;
        }

        int triCount = (vertCount - 2) * 3;

        verts = new Vector3[vertCount];
        indices = new int[triCount];
        uv = new Vector2[vertCount];

        for (int i = 0; i < vertCount; i++)
        {
            Mapped2D val = hulls[i];

            // place the vertex
            verts[i] = val.OriginalValue;

            // generate and place the UV
            Vector2 mappedValue = val.MappedValue;
            mappedValue.x = (mappedValue.x / maxDivX) * 0.5f;
            mappedValue.y = (mappedValue.y / maxDivY) * 0.5f;

            uv[i] = mappedValue;
        }

        int indexCount = 1;

        for (int i = 0; i < triCount; i += 3)
        {
            indices[i + 0] = 0;
            indices[i + 1] = indexCount;
            indices[i + 2] = indexCount + 1;

            indexCount++;
        }

        return true;
    }
}