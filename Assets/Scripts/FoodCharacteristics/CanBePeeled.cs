﻿using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class CanBePeeled : CanBeChopped {

    public GameObject objectFlesh;

    // Use this for initialization
    void Start()
    {
        detachChildrenOnSlice = true;
        base.Start();
    }

    public override void BeginSlice(Vector3 anchorPoint, Vector3 normalDirection)
    {
        this._anchorPoint = anchorPoint;
        this._normalDirection = normalDirection;

        // Set a root object for all sliced pieces
        if (this._rootObject == null)
        {
            _rootObject = Instantiate(rootObjectAfterSlice, this.transform.position, Quaternion.identity);
            _rootObject.name = this.gameObject.name + "_Sliced";
            this.transform.SetParent(_rootObject.transform);
        }

        // Detach children meshes from object, if it exists
        if (detachChildrenOnSlice && transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                // If this is a child with mesh, detach it from the parent.
                if (child.GetComponent<MeshFilter>())
                {
                    child.SetParent(_rootObject.transform);
                    /*MeshCollider cmc = child.gameObject.AddComponent<MeshCollider>();
                    cmc.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
                    cmc.skinWidth = colliderSkinWidth;
                    cmc.convex = true;
                    child.gameObject.AddComponent<Rigidbody>();*/
                }
            }
        }

        // Run the cpu-heavy object cutting process async from script execution. This multithreaded solution resolves most of freezing problems in game. 
        this.StartCoroutine(Cut());
        StartCoroutine(SliceDelay());
    }


    public override IEnumerator Cut()
    {
        // Jump to main thread for running UNITY API calls
        yield return Ninja.JumpToUnity;

        // set the blade relative to victim
        _blade = new Plane(gameObject.transform.InverseTransformDirection(-_normalDirection),
            gameObject.transform.InverseTransformPoint(_anchorPoint));

        // get the victims mesh
        _victim_mesh = gameObject.GetComponent<MeshFilter>().mesh;

        Vector3[] victim_vertices = _victim_mesh.vertices;
        Vector3[] victim_normals = _victim_mesh.normals;
        Vector2[] victim_uvs = _victim_mesh.uv;
        Vector4[] victim_tangents = _victim_mesh.tangents;

        int subMeshCount = _victim_mesh.subMeshCount;

        // Jump back to background thread to do heavy processes.
        yield return Ninja.JumpBack;

        // reset values
        _new_vertices.Clear();

        _leftSide = new Mesh_Maker();
        _rightSide = new Mesh_Maker();

        bool[] sides = new bool[3];
        int[] indices;
        int p1, p2, p3;


        // go throught the submeshes
        for (int sub = 0; sub < subMeshCount; sub++)
        {

            yield return Ninja.JumpToUnity;
            indices = _victim_mesh.GetTriangles(sub);
            yield return Ninja.JumpBack;

            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                sides[0] = _blade.GetSide(victim_vertices[p1]);
                sides[1] = _blade.GetSide(victim_vertices[p2]);
                sides[2] = _blade.GetSide(victim_vertices[p3]);

                // whole triangle
                if (sides[0] == sides[1] && sides[0] == sides[2])
                {

                    if (sides[0])
                    { // left side

                        _leftSide.AddTriangle(
                            new Vector3[] { victim_vertices[p1], victim_vertices[p2], victim_vertices[p3] },
                            new Vector3[] { victim_normals[p1], victim_normals[p2], victim_normals[p3] },
                            new Vector2[] { victim_uvs[p1], victim_uvs[p2], victim_uvs[p3] },
                            new Vector4[] { victim_tangents[p1], victim_tangents[p2], victim_tangents[p3] },
                            sub);
                    }
                    else
                    {

                        _rightSide.AddTriangle(
                            new Vector3[] { victim_vertices[p1], victim_vertices[p2], victim_vertices[p3] },
                            new Vector3[] { victim_normals[p1], victim_normals[p2], victim_normals[p3] },
                            new Vector2[] { victim_uvs[p1], victim_uvs[p2], victim_uvs[p3] },
                            new Vector4[] { victim_tangents[p1], victim_tangents[p2], victim_tangents[p3] },
                            sub);
                    }

                }
                else
                { // cut the triangle

                    Cut_this_Face(
                        new Vector3[] { victim_vertices[p1], victim_vertices[p2], victim_vertices[p3] },
                        new Vector3[] { victim_normals[p1], victim_normals[p2], victim_normals[p3] },
                        new Vector2[] { victim_uvs[p1], victim_uvs[p2], victim_uvs[p3] },
                        new Vector4[] { victim_tangents[p1], victim_tangents[p2], victim_tangents[p3] },
                        sub);
                }
            }
        }

        // Jump to main thread for running UNITY API calls
        yield return Ninja.JumpToUnity;

        // The capping Material will be at the end
        Material[] mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
        if (mats[mats.Length - 1].name != capMaterial.name)
        {
            Material[] newMats = new Material[mats.Length + 1];
            mats.CopyTo(newMats, 0);
            newMats[mats.Length] = capMaterial;
            mats = newMats;
        }
        _capMatSub = mats.Length - 1; // for later use

        // Left Mesh
        Mesh left_HalfMesh = _leftSide.GetMesh();
        left_HalfMesh.name = "Split Mesh Left";

        // Right Mesh
        Mesh right_HalfMesh = _rightSide.GetMesh();
        right_HalfMesh.name = "Split Mesh Right";

        // assign the game objects
        gameObject.name = "left side";
        gameObject.GetComponent<MeshFilter>().mesh = left_HalfMesh;

        GameObject leftSideObj = gameObject;

        GameObject rightSideObj = new GameObject("right side", typeof(MeshFilter), typeof(MeshRenderer));
        rightSideObj.transform.position = gameObject.transform.position;
        rightSideObj.transform.rotation = gameObject.transform.rotation;
        rightSideObj.GetComponent<MeshFilter>().mesh = right_HalfMesh;

        if (gameObject.transform.parent != null)
        {
            rightSideObj.transform.parent = gameObject.transform.parent;
        }

        rightSideObj.transform.localScale = gameObject.transform.localScale;

        if (right_HalfMesh.vertexCount > left_HalfMesh.vertexCount)
        {
            //objectFlesh.transform.localPosition = new Vector3(0, 0, 0);
            //objectFlesh.transform.SetParent(rightSideObj.transform);
            objectFlesh.transform.parent = rightSideObj.transform;
            objectFlesh.transform.position = rightSideObj.transform.position;
            objectFlesh.transform.rotation = rightSideObj.transform.rotation;

        }
        else
        {
            //objectFlesh.transform.localPosition = new Vector3(0,0,0);
            //objectFlesh.transform.SetParent(leftSideObj.transform);
            objectFlesh.transform.parent = leftSideObj.transform;
            objectFlesh.transform.position = leftSideObj.transform.position;
            objectFlesh.transform.rotation = leftSideObj.transform.rotation;
        }

        // assign mats
        leftSideObj.GetComponent<MeshRenderer>().materials = mats;
        rightSideObj.GetComponent<MeshRenderer>().materials = mats;

        // Handle new colliders of left & right pieces
        HandleCollisions(leftSideObj);
        HandleCollisions(rightSideObj);

        // Create required components & set parameters on right piece, copy component values to new one.
        CanBePeeled cbc = rightSideObj.AddComponent<CanBePeeled>();
        cbc.capMaterial = this.capMaterial;
        cbc.sliceTimeout = this.sliceTimeout;
        cbc.colliderSkinWidth = this.colliderSkinWidth;
        cbc.detachChildrenOnSlice = this.detachChildrenOnSlice;
        cbc.newPieceMassMultiplier = this.newPieceMassMultiplier;
        cbc.canBeChoppedWhileOnHand = this.canBeChoppedWhileOnHand;
        cbc._rootObject = this._rootObject;
        cbc.choppingSoundBoard = this.choppingSoundBoard;
        cbc.smallerAllowedPieceVolume = this.smallerAllowedPieceVolume;
        cbc.objectFlesh = this.objectFlesh;
        ////

        // Sound effects preparation
        AudioSource asrc = rightSideObj.AddComponent<AudioSource>();
        asrc.spatialBlend = 1f;
        asrc.volume = 0.4f;
        asrc.playOnAwake = false;
        ////

        // Other Stuff
        VRTK.VRTK_InteractableObject vrtk_io = rightSideObj.AddComponent<VRTK_InteractableObject>();
        VRTK_FixedJointGrabAttach vrtk_fjga = rightSideObj.AddComponent<VRTK_FixedJointGrabAttach>();
        vrtk_io.isGrabbable = true;
        vrtk_fjga.precisionGrab = true;
        /////

        //// Check if new pieces are too small. If so, prevent chopping them into more smaller parts.
        Renderer rend = GetComponent<Renderer>();
        if (rend.bounds.size.x * rend.bounds.size.y * rend.bounds.size.z < smallerAllowedPieceVolume)
        {
            currentlyChoppable = false;
        }

        rend = rightSideObj.GetComponent<Renderer>();
        if (rend.bounds.size.x * rend.bounds.size.y * rend.bounds.size.z < smallerAllowedPieceVolume)
        {
            cbc.currentlyChoppable = false;
        }

        // End thread
        yield return Ninja.JumpBack;

        yield break;
    }

    public override void HandleCollisions(GameObject piece)
    {
        Rigidbody rb;

        if (!piece.GetComponent<Rigidbody>())
        {
            rb = piece.AddComponent<Rigidbody>();
            rb.mass = GetComponent<Rigidbody>().mass * newPieceMassMultiplier;
        }

        else
        {
            rb = piece.GetComponent<Rigidbody>();
        }

        rb.isKinematic = true;

        // Reset mesh collider
        if (piece.GetComponent<Collider>())
        {
            Destroy(piece.GetComponent<Collider>());
        }

        MeshCollider mc = piece.AddComponent<MeshCollider>();
        mc.skinWidth = colliderSkinWidth;
        mc.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh | MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.EnableMeshCleaning;
        mc.convex = true;

        rb.isKinematic = false;
    }

}