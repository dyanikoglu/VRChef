using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpAreaCollision : MonoBehaviour {
    public Material mat;
    public bool canSlice = true;
    public bool colliderError = false;

	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator ExecuteAfterTime(float time, NonConvexMeshCollider ncmc)
    {
        yield return new WaitForSeconds(time);

        ncmc.Calculate();
    }

    IEnumerator EnableSlice(float time)
    {
        yield return new WaitForSeconds(time);

        canSlice = true;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(logString.StartsWith("[Physics.PhysX]"))
        {
            colliderError = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canSlice && collision.gameObject.CompareTag("Choppable"))
        {
            canSlice = false;

            MeshCut cutter = gameObject.AddComponent<MeshCut>();

            cutter.BeginCut(victim, transform.position, transform.right, capMaterial);

            GameObject[] pieces = MeshCut.Cut(collision.gameObject, gameObject.transform.position, gameObject.transform.up, mat);

            foreach (GameObject piece in pieces)
            {
                Rigidbody rb;
                if (piece.GetComponent<Rigidbody>())
                {
                    rb = piece.GetComponent<Rigidbody>();
                }

                else
                {
                    rb = piece.AddComponent<Rigidbody>();
                }

                rb.isKinematic = true;

                foreach (Collider c in piece.GetComponents<Collider>())
                {
                    Destroy(c);
                }

                MeshCollider mc = piece.AddComponent<MeshCollider>();
                mc.convex = true;

                if (colliderError)
                {
                    colliderError = false;

                    Destroy(piece.GetComponent<MeshCollider>());

                    NonConvexMeshCollider ncmc;
                    if (piece.GetComponent<NonConvexMeshCollider>())
                    {
                        ncmc = piece.GetComponent<NonConvexMeshCollider>();
                    }

                    else
                    {
                        ncmc = piece.AddComponent<NonConvexMeshCollider>();
                    }

                    ncmc.boxesPerEdge = 1;
                    ncmc.createColliderChildGameObject = false;
                    StartCoroutine(ExecuteAfterTime(0.05f, ncmc));
                }

                rb.isKinematic = false;
            }

            StartCoroutine(EnableSlice(0.5f));
            pieces[1].gameObject.tag = "Choppable";
        }
    }
}
