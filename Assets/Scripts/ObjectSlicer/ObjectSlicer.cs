/*
 * Author: Doğa Can Yanıkoğlu
 */

using System.Collections.Generic;
using UnityEngine;  

/*
 * Main class for Object Slicing feature.
 */
public class ObjectSlicer : MonoBehaviour {
    // public GameObject obje;
    public Material crossMat;

    //public void Start()
    //{
    //    BeginSlicing(obje, crossMat);
    //}

    /*
     * Public function provided for completing cutting actions with knife.
     */
    public void BeginSlicing(GameObject obj, Material crossAreaMat)
    {
        SliceObject(obj, crossAreaMat);
        obj.SetActive(false);
    }

    /*
     * Slices parameter GameObject recursively. Fills cross sections with parameter Material.
     */
    private GameObject[] SliceObject(GameObject obj, Material crossAreaMat) {

        // Use knife's trail in object to create proper cutting plane
        ExtendedPlane cuttingPlane = new ExtendedPlane();

        Vector3 refUp = obj.transform.InverseTransformDirection(transform.up);
        Vector3 refPt = obj.transform.InverseTransformPoint(transform.position);

        cuttingPlane.Compute(refPt, refUp);

        SlicedHull finalHull = Slice(obj, cuttingPlane);

        if (finalHull != null) {
            GameObject lowerParent = finalHull.CreateLowerHull(obj, crossAreaMat);
            GameObject upperParent = finalHull.CreateUpperHull(obj, crossAreaMat);

            if (obj.transform.childCount > 0) {
                foreach (Transform child in obj.transform) {
                    if (child != null && child.gameObject != null) {
                        if (child.childCount > 0) {
                            GameObject[] children = SliceObject(child.gameObject, crossAreaMat);

                            if (children != null) {
                                if (children[0] != null && lowerParent != null) {
                                    children[0].transform.SetParent(lowerParent.transform, false);
                                }

                                if (children[1] != null && upperParent != null) {
                                    children[1].transform.SetParent(upperParent.transform, false);
                                }
                            }
                        }
                        else {
                            ExtendedPlane childCuttingPlane = new ExtendedPlane();
                                
                            Vector3 childRefUp = child.gameObject.transform.InverseTransformDirection(transform.up);
                            Vector3 childRefPt = child.gameObject.transform.InverseTransformPoint(transform.position);

                            childCuttingPlane.Compute(childRefPt, childRefUp);

                            SlicedHull hull = Slice(child.gameObject, childCuttingPlane);

                            if (hull != null) {
                                GameObject childLowerHull = hull.CreateLowerHull(child.gameObject, crossAreaMat);
                                GameObject childUpperHull = hull.CreateUpperHull(child.gameObject, crossAreaMat);

                                if (childLowerHull != null && lowerParent != null) {
                                    childLowerHull.transform.SetParent(lowerParent.transform, false);
                                }

                                if (childUpperHull != null && upperParent != null) {
                                    childUpperHull.transform.SetParent(upperParent.transform, false);
                                }
                            }
                        }
                    }
                }
            }

            return new GameObject[] { lowerParent, upperParent };
        }

        return null;
    }

    /*
     * Overloaded function.
     * If given parameter is a GameObject instead of a mesh, get mesh component from the object, and call overloaded funtion on the mesh again.
     */
    private static SlicedHull Slice(GameObject obj, ExtendedPlane pl)
    {
        MeshFilter renderer = obj.GetComponent<MeshFilter>();

        if (renderer == null)
        {
            return null;
        }

        return Slice(renderer.sharedMesh, pl);
    }

    /*
     * Overloaded function.
     * If given parameter is a mesh, begin slicing process on this mesh.
     * To do that, firstly detect triangles at below & above of cutting plane.
     * After that, detect triangles that crosses with cutting plane. These triangles should be processed for creating a cross section.
     * Create 2 new convex objects, and their cross sections.
     */
    private static SlicedHull Slice(Mesh sharedMesh, ExtendedPlane pl)
    {
        if (sharedMesh == null)
        {
            return null;
        }

        Vector3[] ve = sharedMesh.vertices;
        Vector2[] uv = sharedMesh.uv;
        int[] indices = sharedMesh.triangles;

        int indicesCount = indices.Length;

        IntersectionResult result = new IntersectionResult();

        List<ExtendedTriangle> upperHull = new List<ExtendedTriangle>();
        List<ExtendedTriangle> lowerHull = new List<ExtendedTriangle>();
        List<Vector3> crossHull = new List<Vector3>();

        for (int index = 0; index < indicesCount; index += 3)
        {
            int i0 = indices[index + 0];
            int i1 = indices[index + 1];
            int i2 = indices[index + 2];

            ExtendedTriangle newTri = new ExtendedTriangle(ve[i0], ve[i1], ve[i2], uv[i0], uv[i1], uv[i2]);

            if (newTri.Split(pl, result))
            {
                int upperHullCount = result.UpperHullCount;
                int lowerHullCount = result.LowerHullCount;
                int interHullCount = result.IntersectionPointCount;

                for (int i = 0; i < upperHullCount; i++)
                {
                    upperHull.Add(result.UpperHull[i]);
                }

                for (int i = 0; i < lowerHullCount; i++)
                {
                    lowerHull.Add(result.LowerHull[i]);
                }

                for (int i = 0; i < interHullCount; i++)
                {
                    crossHull.Add(result.IntersectionPoints[i]);
                }
            }
            else
            {
                SideOfPlane side = pl.SideOf(ve[i0]);

                if (side == SideOfPlane.UP || side == SideOfPlane.ON)
                {
                    upperHull.Add(newTri);
                }
                else
                {
                    lowerHull.Add(newTri);
                }
            }
        }

        Mesh finalUpperHull = CalculateHull(upperHull);
        Mesh finalLowerHull = CalculateHull(lowerHull);
        Mesh[] crossSections = CalculateHull(crossHull, pl.Normal);

        if (crossSections != null)
        {
            return new SlicedHull(finalUpperHull, finalLowerHull, crossSections[0], crossSections[1]);
        }

        return new SlicedHull(finalUpperHull, finalLowerHull);
    }

    /*
     * Overloaded function.
     * Required for calculating cross section area. Takes some vertices as parameter, and triangulates them with monotonechain algorithm ( O(nlgn) worst case)
     */
    private static Mesh[] CalculateHull(List<Vector3> intPoints, Vector3 planeNormal)
    {
        Vector3[] newVertices;
        Vector2[] newUvs;
        int[] newIndices;

        if (TriangulateFace.MonotoneChain(intPoints, planeNormal, out newVertices, out newIndices, out newUvs))
        {
            Mesh upperCrossSection = new Mesh
            {
                vertices = newVertices,
                uv = newUvs,
                triangles = newIndices
            };

            upperCrossSection.RecalculateNormals();

            int indiceCount = newIndices.Length;
            int[] flippedIndices = new int[indiceCount];

            for (int i = 0; i < indiceCount; i += 3)
            {
                flippedIndices[i] = newIndices[i];
                flippedIndices[i + 1] = newIndices[i + 2];
                flippedIndices[i + 2] = newIndices[i + 1];
            }

            Mesh lowerCrossSection = new Mesh
            {
                vertices = newVertices,
                uv = newUvs,
                triangles = flippedIndices
            };

            lowerCrossSection.RecalculateNormals();

            return new Mesh[] { upperCrossSection, lowerCrossSection };
        }

        return null;
    }

    /*
     * Overloaded function.
     * Required for calculating upper & lower part of sliced object. Takes some vertices as parameter, and creates a new hull from them. Uses same UV coordinates from complete object directly.
     */
    private static Mesh CalculateHull(List<ExtendedTriangle> hull)
    {
        int count = hull.Count;

        if (count <= 0)
        {
            return null;
        }

        Mesh newMesh = new Mesh();

        Vector3[] newVertices = new Vector3[count * 3];
        Vector2[] newUvs = new Vector2[count * 3];
        int[] newIndices = new int[count * 3];

        int addedCount = 0;

        for (int i = 0; i < count; i++)
        {
            ExtendedTriangle newTri = hull[i];

            int i0 = addedCount + 0;
            int i1 = addedCount + 1;
            int i2 = addedCount + 2;

            newVertices[i0] = newTri.PositionA;
            newVertices[i1] = newTri.PositionB;
            newVertices[i2] = newTri.PositionC;

            newUvs[i0] = newTri.UvA;
            newUvs[i1] = newTri.UvB;
            newUvs[i2] = newTri.UvC;

            newIndices[i0] = i0;
            newIndices[i1] = i1;
            newIndices[i2] = i2;

            addedCount += 3;
        }

        newMesh.vertices = newVertices;
        newMesh.uv = newUvs;
        newMesh.triangles = newIndices;

        newMesh.RecalculateNormals();

        return newMesh;
    }
}