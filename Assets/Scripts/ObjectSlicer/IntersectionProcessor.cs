﻿/*
 * Author: David Arayan
 */

using UnityEngine;

/*
 * Intersection processor class for Monotonechain algorithm. Required for detecting & dropping inner vertices while building convex hull.
 * Makes use of Extended classes as ExtendedTriangle, ExtendedPlane, and ExtendedLine.
 */
public class IntersectionProcessor
{
    /**
     * Perform an intersection between Plane and Line, storing intersection point
     * in reference q. Function returns true if intersection has been found or
     * false otherwise.
     */
public static bool Intersect(ExtendedPlane pl, ExtendedLine ln, out Vector3 q)
    {
        return IntersectionProcessor.Intersect(pl, ln.PositionA, ln.PositionB, out q);
    }

    /**
     * Perform an intersection between Plane and Line made up of points a and b. Intersection
     * point will be stored in reference q. Function returns true if intersection has been
     * found or false otherwise.
     */
    public static bool Intersect(ExtendedPlane pl, Vector3 a, Vector3 b, out Vector3 q)
    {
        Vector3 normal = pl.Normal;
        Vector3 ab = b - a;

        float t = (pl.Dist - Vector3.Dot(normal, a)) / Vector3.Dot(normal, ab);

        // need to be careful and compensate for floating errors
        if (t >= -0.001f && t <= 1.001f)
        {
            q = a + t * ab;

            return true;
        }

        q = Vector3.zero;

        return false;
    }

    /**
     * Support functionality 
     */
    public static float TriArea2D(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
    }

    /**
     * Perform an intersection between Plane and Triangle. This is a comprehensive function
     * which alwo builds a HULL Hirearchy useful for decimation projects. This obviously
     * comes at the cost of more complex code and runtime checks, but the returned results
     * are much more flexible.
     * Results will be filled into the IntersectionResult reference. Check result.isValid()
     * for the final results.
     */
    public static void Intersect(ExtendedPlane pl, ExtendedTriangle tri, ref IntersectionResult result)
    {
        // clear the previous results from the IntersectionResult
        result.Clear();

        // grab local variables for easier access
        Vector3 a = tri.PositionA;
        Vector3 b = tri.PositionB;
        Vector3 c = tri.PositionC;

        // check to see which side of the plane the points all
        // lay in. SideOf operation is a simple dot product and some comparison
        // operations, so these are a very quick checks
        SideOfPlane sa = pl.SideOf(a);
        SideOfPlane sb = pl.SideOf(b);
        SideOfPlane sc = pl.SideOf(c);

        // we cannot intersect if the triangle points all fall on the same side
        // of the plane. This is an easy early out test as no intersections are possible.
        if (sa == sb && sb == sc)
        {
            return;
        }

        // detect cases where two points lay straight on the plane, meaning
        // that the plane is actually parralel with one of the edges of the triangle
        else if ((sa == SideOfPlane.ON && sa == sb) ||
            (sa == SideOfPlane.ON && sa == sc) ||
            (sb == SideOfPlane.ON && sb == sc))
        {
            return;
        }

        // the UV coordinates of all the vertices, this will be used to re-calculate the
        // UV coordinates for the new intersection points.
        Vector3 uva = tri.UvA;
        Vector3 uvb = tri.UvB;
        Vector3 uvc = tri.UvC;

        // keep in mind that intersection points are shared by both
        // the upper HULL and lower HULL hence they lie perfectly
        // on the plane that cut them
        Vector3 qa;
        Vector3 qb;

        // check the cases where the points of the triangle actually lie on the plane itself
        // in these cases, there is only going to be 2 triangles, one for the upper HULL and
        // the other on the lower HULL
        // we just need to figure out which points to accept into the upper or lower hulls.
        if (sa == SideOfPlane.ON)
        {
            // if the point a is on the plane, test line b-c
            if (IntersectionProcessor.Intersect(pl, b, c, out qa))
            {
                // the computed UV coordinate of the intersection point
                Vector2 uvq = tri.GenerateUVCoords(qa);

                // line b-c intersected, construct out triangles and return approprietly
                result.AddIntersectionPoint(qa);
                result.AddIntersectionPoint(a);

                // our two generated triangles, we need to figure out which
                // triangle goes into the UPPER hull and which goes into the LOWER hull
                ExtendedTriangle ta = new ExtendedTriangle(a, b, qa, uva, uvb, uvq);
                ExtendedTriangle tb = new ExtendedTriangle(a, qa, c, uva, uvq, uvc);

                // b point lies on the upside of the plane
                if (sb == SideOfPlane.UP)
                {
                    result.AddUpperHull(ta).AddLowerHull(tb);
                }

                // b point lies on the downside of the plane
                else if (sb == SideOfPlane.DOWN)
                {
                    result.AddUpperHull(tb).AddLowerHull(ta);
                }
            }
        }

        // test the case where the b point lies on the plane itself
        else if (sb == SideOfPlane.ON)
        {
            // if the point b is on the plane, test line a-c
            if (IntersectionProcessor.Intersect(pl, a, c, out qa))
            {
                // the computed UV coordinate of the intersection point
                Vector2 uvq = tri.GenerateUVCoords(qa);

                // line a-c intersected, construct out triangles and return approprietly
                result.AddIntersectionPoint(qa);
                result.AddIntersectionPoint(b);

                // our two generated triangles, we need to figure out which
                // triangle goes into the UPPER hull and which goes into the LOWER hull
                ExtendedTriangle ta = new ExtendedTriangle(a, b, qa, uva, uvb, uvq);
                ExtendedTriangle tb = new ExtendedTriangle(qa, b, c, uvq, uvb, uvc);

                // a point lies on the upside of the plane
                if (sa == SideOfPlane.UP)
                {
                    result.AddUpperHull(ta).AddLowerHull(tb);
                }

                // a point lies on the downside of the plane
                else if (sa == SideOfPlane.DOWN)
                {
                    result.AddUpperHull(tb).AddLowerHull(ta);
                }
            }
        }

        // test the case where the c point lies on the plane itself
        else if (sc == SideOfPlane.ON)
        {
            // if the point c is on the plane, test line a-b
            if (IntersectionProcessor.Intersect(pl, a, b, out qa))
            {
                // the computed UV coordinate of the intersection point
                Vector2 uvq = tri.GenerateUVCoords(qa);

                // line a-c intersected, construct out triangles and return approprietly
                result.AddIntersectionPoint(qa);
                result.AddIntersectionPoint(c);

                // our two generated triangles, we need to figure out which
                // triangle goes into the UPPER hull and which goes into the LOWER hull
                ExtendedTriangle ta = new ExtendedTriangle(a, qa, c, uva, uvq, uvc);
                ExtendedTriangle tb = new ExtendedTriangle(qa, b, c, uvq, uvb, uvc);

                // a point lies on the upside of the plane
                if (sa == SideOfPlane.UP)
                {
                    result.AddUpperHull(ta).AddLowerHull(tb);
                }

                // a point lies on the downside of the plane
                else if (sa == SideOfPlane.DOWN)
                {
                    result.AddUpperHull(tb).AddLowerHull(ta);
                }
            }
        }

        // at this point, all edge cases have been tested and failed, we need to perform
        // full intersection tests against the lines. From this point onwards we will generate
        // 3 triangles
        else if (sa != sb && IntersectionProcessor.Intersect(pl, a, b, out qa))
        {
            // the computed UV coordinate of the intersection point
            Vector2 uvqa = tri.GenerateUVCoords(qa);

            // intersection found against a - b
            result.AddIntersectionPoint(qa);

            // since intersection was found against a - b, we need to check which other
            // lines to check (we only need to check one more line) for intersection.
            // the line we check against will be the line against the point which lies on
            // the other side of the plane.
            if (sa == sc)
            {
                // we likely have an intersection against line b-c which will complete this loop
                if (IntersectionProcessor.Intersect(pl, b, c, out qb))
                {
                    // the computed UV coordinate of the intersection point
                    Vector2 uvqb = tri.GenerateUVCoords(qb);

                    result.AddIntersectionPoint(qb);

                    // our three generated triangles. Two of these triangles will end
                    // up on either the UPPER or LOWER hulls.
                    ExtendedTriangle ta = new ExtendedTriangle(qa, b, qb, uvqa, uvb, uvqb);
                    ExtendedTriangle tb = new ExtendedTriangle(a, qa, qb, uva, uvqa, uvqb);
                    ExtendedTriangle tc = new ExtendedTriangle(a, qb, c, uva, uvqb, uvc);

                    if (sa == SideOfPlane.UP)
                    {
                        result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
                    }
                    else
                    {
                        result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
                    }
                }
            }
            else
            {
                // in this scenario, the point a is a "lone" point which lies in either upper
                // or lower HULL. We need to perform another intersection to find the last point
                if (IntersectionProcessor.Intersect(pl, a, c, out qb))
                {
                    // the computed UV coordinate of the intersection point
                    Vector2 uvqb = tri.GenerateUVCoords(qb);

                    result.AddIntersectionPoint(qb);

                    // our three generated triangles. Two of these triangles will end
                    // up on either the UPPER or LOWER hulls.
                    ExtendedTriangle ta = new ExtendedTriangle(a, qa, qb, uva, uvqa, uvqb);
                    ExtendedTriangle tb = new ExtendedTriangle(qa, b, c, uvqa, uvb, uvc);
                    ExtendedTriangle tc = new ExtendedTriangle(qb, qa, c, uvqb, uvqa, uvc);

                    if (sa == SideOfPlane.UP)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb).AddLowerHull(tc);
                    }
                    else
                    {
                        result.AddLowerHull(ta).AddUpperHull(tb).AddUpperHull(tc);
                    }
                }
            }
        }

        // if line a-b did not intersect (or the lie on the same side of the plane)
        // this simplifies the problem a fair bit. This means we have an intersection 
        // in line a-c and b-c, which we can use to build a new UPPER and LOWER hulls
        // we are expecting both of these intersection tests to pass, otherwise something
        // went wrong (float errors? missed a checked case?)
        else if (IntersectionProcessor.Intersect(pl, c, a, out qa) && IntersectionProcessor.Intersect(pl, c, b, out qb))
        {
            // in here we know that line a-b actually lie on the same side of the plane, this will
            // simplify the rest of the logic. We also have our intersection points
            // the computed UV coordinate of the intersection point
            Vector2 uvqa = tri.GenerateUVCoords(qa);
            Vector2 uvqb = tri.GenerateUVCoords(qb);

            result.AddIntersectionPoint(qa);
            result.AddIntersectionPoint(qb);

            // our three generated triangles. Two of these triangles will end
            // up on either the UPPER or LOWER hulls.
            ExtendedTriangle ta = new ExtendedTriangle(qa, qb, c, uvqa, uvqb, uvc);
            ExtendedTriangle tb = new ExtendedTriangle(a, qb, qa, uva, uvqb, uvqa);
            ExtendedTriangle tc = new ExtendedTriangle(a, b, qb, uva, uvb, uvqb);

            if (sa == SideOfPlane.UP)
            {
                result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
            }
            else
            {
                result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
            }
        }
    }
}