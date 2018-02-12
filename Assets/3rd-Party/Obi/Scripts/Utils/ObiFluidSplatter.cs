using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obi;

namespace Obi
{
[RequireComponent(typeof(ObiSolver))]
public class ObiFluidSplatter : MonoBehaviour {
	
	ObiSolver solver;

	void Awake(){
		solver = GetComponent<Obi.ObiSolver>();
	}

	void OnEnable () {
		solver.OnCollision += Solver_OnCollision;
	}

	void OnDisable(){
		solver.OnCollision -= Solver_OnCollision;
	}
	
	void Solver_OnCollision (object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
	{
		/*for(int i = 0;  i < e.points.Length; ++i){
			if (e.distances[i] <= 0.05f){
				Splat(new Ray(e.points[i] + e.normals[i]*0.5f, -e.normals[i]));
			}
		}	*/	
	}

	// Generate a splat:
	void Splat (Ray ray) {

		//Debug.DrawRay(ray.origin,ray.direction*0.5f,Color.red,0.1f,false);
        
        RaycastHit hit;
		if (!Physics.Raycast(ray, out hit))
            return;
        
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;
        
        /*Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
        tex.Apply();*/
	
	}

}

}