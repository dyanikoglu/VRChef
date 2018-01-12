/*
 * Author: Doğa Can Yanıkoğlu
 */

using UnityEngine;

/*
 * Data Structure for storing calculated sliced hull vertices & tris.
 */
public class SlicedHull
{
    private Mesh upper_hull;
    private Mesh lower_hull;
    private Mesh upper_cross_section;
    private Mesh lower_cross_section;

    public SlicedHull(Mesh upperHull, Mesh lowerHull) : this(upperHull, lowerHull, null, null) { }

    public SlicedHull(Mesh upperHull, Mesh lowerHull, Mesh upperCrossSection, Mesh lowerCrossSection)
    {
        this.upper_hull = upperHull;
        this.lower_hull = lowerHull;
        this.upper_cross_section = upperCrossSection;
        this.lower_cross_section = lowerCrossSection;
    }

    public GameObject CreateUpperHull(GameObject original)
    {
        return CreateUpperHull(original, null);
    }

    public GameObject CreateUpperHull(GameObject original, Material crossSectionMat)
    {
        GameObject newObject = CreateUpperHull();

        if (newObject != null)
        {
            newObject.transform.localPosition = original.transform.localPosition;
            newObject.transform.localRotation = original.transform.localRotation;
            newObject.transform.localScale = original.transform.localScale;

            newObject.GetComponent<Renderer>().sharedMaterials = original.GetComponent<MeshRenderer>().sharedMaterials;

            if (newObject.transform.childCount > 0 && crossSectionMat != null)
            {
                newObject.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = crossSectionMat;
            }
        }

        return newObject;
    }

    public GameObject CreateUpperHull()
    {
        GameObject newObject = CreateEmptyObject("UpperHull", upper_hull);

        if (newObject != null)
        {
            GameObject crossSection = CreateEmptyObject("CrossSection", upper_cross_section);

            if (crossSection != null)
            {
                crossSection.transform.parent = newObject.transform;
            }
        }

        return newObject;
    }

    public GameObject CreateLowerHull(GameObject original)
    {
        return CreateLowerHull(original, null);
    }

    public GameObject CreateLowerHull(GameObject original, Material crossSectionMat)
    {
        GameObject newObject = CreateLowerHull();

        if (newObject != null)
        {
            newObject.transform.localPosition = original.transform.localPosition;
            newObject.transform.localRotation = original.transform.localRotation;
            newObject.transform.localScale = original.transform.localScale;

            newObject.GetComponent<Renderer>().sharedMaterials = original.GetComponent<MeshRenderer>().sharedMaterials;

            if (newObject.transform.childCount > 0 && crossSectionMat != null)
            {
                newObject.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = crossSectionMat;
            }
        }

        return newObject;
    }

    public GameObject CreateLowerHull()
    {
        GameObject newObject = CreateEmptyObject("LowerHull", lower_hull);

        if (newObject != null)
        {
            GameObject crossSection = CreateEmptyObject("CrossSection", lower_cross_section);

            if (crossSection != null)
            {
                crossSection.transform.parent = newObject.transform;
            }
        }

        return newObject;
    }

    public Mesh UpperHull
    {
        get { return this.upper_hull; }
    }

    public Mesh LowerHull
    {
        get { return this.lower_hull; }
    }

    public Mesh UpperHullCrossSection
    {
        get { return this.upper_cross_section; }
    }

    public Mesh LowerHullCrossSection
    {
        get { return this.lower_cross_section; }
    }

    private static GameObject CreateEmptyObject(string name, Mesh hull)
    {
        if (hull == null)
        {
            return null;
        }

        GameObject newObject = new GameObject(name);

        newObject.AddComponent<MeshRenderer>();
        MeshFilter filter = newObject.AddComponent<MeshFilter>();

        filter.mesh = hull;

        return newObject;
    }
}