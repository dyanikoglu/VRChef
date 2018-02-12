using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Obi{

[ExecuteInEditMode]
[RequireComponent(typeof(ObiActor))]
public class ObiParticleRenderer : MonoBehaviour
{
	public bool render = true;
	public Color particleColor = Color.white; 
	public float radiusScale = 1;

	private ObiActor actor;
	private List<Mesh> meshes = new List<Mesh>();
	private Material material;

	// Geometry buffers:
	private List<Vector3> vertices = new List<Vector3>();
	private List<Vector3> normals = new List<Vector3>();
	private List<Color> colors = new List<Color>();
	private List<int> triangles = new List<int>();

	int particlesPerDrawcall = 0;
	int drawcallCount;

	private Vector3[] particleOffsets = new Vector3[4]{
		new Vector3(1,1,0),
		new Vector3(-1,1,0),
		new Vector3(-1,-1,0),
		new Vector3(1,-1,0)
	};

	public IEnumerable<Mesh> ParticleMeshes{
		get { return meshes; }
	}

	public Material ParticleMaterial{
		get { return material; }
	}

	public void Awake(){
		actor = GetComponent<ObiActor>();
	}

	public void OnEnable(){

		material = GameObject.Instantiate(Resources.Load<Material>("ObiMaterials/Particle"));
		material.hideFlags = HideFlags.HideAndDontSave;

		// TODO: automatically support the case where the actor changes solver, without the need to disable/enable the renderer:
		if (actor != null && actor.Solver != null)
		{
			// figure out the size of our drawcall arrays:
			particlesPerDrawcall = Constants.maxVertsPerMesh/4;
			drawcallCount = actor.positions.Length / particlesPerDrawcall + 1;
			particlesPerDrawcall = Mathf.Min(particlesPerDrawcall,actor.positions.Length);

			actor.Solver.RequireRenderablePositions();
			actor.Solver.OnFrameEnd += Actor_solver_OnFrameEnd;
		} 
	}

	public void OnDisable(){

		if (actor != null && actor.Solver != null)
		{
			actor.Solver.RelinquishRenderablePositions();
			actor.Solver.OnFrameEnd -= Actor_solver_OnFrameEnd;
		}

		ClearMeshes();

		GameObject.DestroyImmediate(material);
	}

	void Actor_solver_OnFrameEnd (object sender, EventArgs e)
	{
		if (actor == null || !actor.InSolver || !actor.isActiveAndEnabled){
			ClearMeshes();
			return;
		}

		ObiSolver solver = actor.Solver;

		// If the amount of meshes we need to draw the particles has changed:
		if (drawcallCount != meshes.Count){

			// Re-generate meshes:
			ClearMeshes();
			for (int i = 0; i < drawcallCount; i++){
				Mesh mesh = new Mesh();
				mesh.name = "Particle imposters";
				mesh.hideFlags = HideFlags.HideAndDontSave;
				mesh.MarkDynamic();
				meshes.Add(mesh);
			}

		}

		//Convert particle data to mesh geometry:
		for (int i = 0; i < drawcallCount; i++){

			// Clear all arrays
			vertices.Clear();
			normals.Clear();
			colors.Clear();
			triangles.Clear();
			
			Color color = Color.white;
			int index = 0;

			for(int j = i * particlesPerDrawcall; j < (i+1) * particlesPerDrawcall; ++j)
			{
				if (actor.active[j]){

					if (actor.colors != null && j < actor.colors.Length)
						color = actor.colors[j] * particleColor;
					else 
						color = particleColor;

					AddParticle(index,
								solver.renderablePositions[actor.particleIndices[j]],
								color,
								actor.solidRadii[j]*radiusScale);
					index++;
				}
			}

			Apply(meshes[i]);
		}
	
		if (render)
			DrawParticles();
	}

	private void DrawParticles(){

		// Send the meshes to be drawn:
		foreach(Mesh mesh in meshes)
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, gameObject.layer);

	}

	private void Apply(Mesh mesh){
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetColors(colors);
		mesh.SetTriangles(triangles,0,true);
	}

	private void ClearMeshes(){
		foreach(Mesh mesh in meshes)
			GameObject.DestroyImmediate(mesh);
		meshes.Clear();
	}

	private void AddParticle(int i, Vector3 position, Color color, float radius){
		
		int i4 = i*4;
		int i41 = i4+1;
		int i42 = i4+2;
		int i43 = i4+3;
		
		vertices.Add(position);
		vertices.Add(position);
		vertices.Add(position);
		vertices.Add(position);

		particleOffsets[0].z = radius;
		particleOffsets[1].z = radius;
		particleOffsets[2].z = radius;
		particleOffsets[3].z = radius;

		normals.Add(particleOffsets[0]);
		normals.Add(particleOffsets[1]);
		normals.Add(particleOffsets[2]);
		normals.Add(particleOffsets[3]);
		
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
        colors.Add(color);

		triangles.Add(i42);
		triangles.Add(i41);
		triangles.Add(i4);
		triangles.Add(i43);
        triangles.Add(i42);
        triangles.Add(i4);
    }

}
}

