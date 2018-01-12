/*
 * Author: David Arayan
 */

using UnityEngine;

/*
 * Helper class for IntersectionProcessor.
 * Stores results from intersections detected in IntersectionProcessor while applying MonotoneChain algorithm.
 */
public class IntersectionResult {
    private bool is_success;

    private ExtendedTriangle[] upper_hull;
    private ExtendedTriangle[] lower_hull;
    private Vector3[] intersection_pt;

    private int upper_hull_count;
    private int lower_hull_count;
    private int intersection_pt_count;

    public IntersectionResult()
    {
        this.is_success = false;

        this.upper_hull = new ExtendedTriangle[2];
        this.lower_hull = new ExtendedTriangle[2];
        this.intersection_pt = new Vector3[2];

        this.upper_hull_count = 0;
        this.lower_hull_count = 0;
        this.intersection_pt_count = 0;
    }

    public ExtendedTriangle[] UpperHull
    {
        get { return upper_hull; }
    }

    public ExtendedTriangle[] LowerHull
    {
        get { return lower_hull; }
    }

    public Vector3[] IntersectionPoints
    {
        get { return intersection_pt; }
    }

    public int UpperHullCount
    {
        get { return upper_hull_count; }
    }

    public int LowerHullCount
    {
        get { return lower_hull_count; }
    }

    public int IntersectionPointCount
    {
        get { return intersection_pt_count; }
    }

    public bool IsValid
    {
        get { return is_success; }
    }

    public IntersectionResult AddUpperHull(ExtendedTriangle tri)
    {
        upper_hull[upper_hull_count++] = tri;

        is_success = true;

        return this;
    }

    public IntersectionResult AddLowerHull(ExtendedTriangle tri)
    {
        lower_hull[lower_hull_count++] = tri;

        is_success = true;

        return this;
    }

    public void AddIntersectionPoint(Vector3 pt)
    {
        intersection_pt[intersection_pt_count++] = pt;
    }


    public void Clear()
    {
        is_success = false;
        upper_hull_count = 0;
        lower_hull_count = 0;
        intersection_pt_count = 0;
    }
}