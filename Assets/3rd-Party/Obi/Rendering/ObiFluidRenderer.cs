using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


namespace Obi
{

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class ObiFluidRenderer : MonoBehaviour
{

	public ObiParticleRenderer[] particleRenderers;

	[Range(0,0.1f)]
	public float blurRadius = 0.02f;

	[Range(0.01f,2)]
	public float thicknessCutoff = 1.2f;

	private CommandBuffer renderFluid;
	private Material depth_BlurMaterial;
	private Material normal_ReconstructMaterial;
	private Material thickness_Material;
	private Color thicknessBufferClear = new Color(1,1,1,0); /**< clears alpha to black (0 thickness) and color to white.*/

	public Material colorMaterial;
	public Material fluidMaterial;
		
	private void Cleanup()
	{

		if (renderFluid != null){
			GetComponent<Camera>().RemoveCommandBuffer (CameraEvent.BeforeImageEffectsOpaque,renderFluid);
			renderFluid = null;
		}
		if (depth_BlurMaterial != null)
			Object.DestroyImmediate (depth_BlurMaterial);
		if (normal_ReconstructMaterial != null)
			Object.DestroyImmediate (normal_ReconstructMaterial);
		if (thickness_Material != null)
			Object.DestroyImmediate (thickness_Material);
	}

	private static Material CreateMaterial (Shader shader)
    {
		if (!shader || !shader.isSupported)
            return null;
        Material m = new Material (shader);
        m.hideFlags = HideFlags.HideAndDontSave;
        return m;
    }
	
	private void Setup()
	{

		if (depth_BlurMaterial == null)
		{
			depth_BlurMaterial = CreateMaterial(Shader.Find("Hidden/ScreenSpaceCurvatureFlow"));
		}

		if (normal_ReconstructMaterial == null)
		{
			normal_ReconstructMaterial = CreateMaterial(Shader.Find("Hidden/NormalReconstruction"));
		}

		if (thickness_Material == null)
		{
			thickness_Material = CreateMaterial(Shader.Find("Hidden/FluidThickness"));
		}

		bool shadersSupported = depth_BlurMaterial && normal_ReconstructMaterial && thickness_Material;

		if (!shadersSupported ||
			!SystemInfo.supportsImageEffects || 
			!SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth) ||
			!SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RFloat) ||
			!SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.ARGBHalf)
 			)
        {
            enabled = false;
			Debug.LogWarning("Obi Fluid Renderer not supported in this platform.");
            return;
        }

	}
	
	public void OnEnable()
	{
		GetComponent<Camera>().forceIntoRenderTexture = true;
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		Cleanup();
	}
	
	public void OnDisable()
	{
		Cleanup();
	}

	public void SetupFluidRenderingCommandBuffer()
	{
		renderFluid.Clear();

		if (particleRenderers == null || fluidMaterial == null)
			return;
		
		int refraction = Shader.PropertyToID("_Refraction");
		int foam = Shader.PropertyToID("_Foam");
		int depth = Shader.PropertyToID("_FluidDepthTexture");

		int thickness1 = Shader.PropertyToID("_FluidThickness1");
		int thickness2 = Shader.PropertyToID("_FluidThickness2");

		int smoothDepth = Shader.PropertyToID("_FluidSurface");

		int normals = Shader.PropertyToID("_FluidNormals");

		// refraction (background), foam and fluid depth buffers:
		renderFluid.GetTemporaryRT(refraction,-1,-1,0,FilterMode.Bilinear);
		renderFluid.GetTemporaryRT(foam,-1,-1,0,FilterMode.Bilinear);
		renderFluid.GetTemporaryRT(depth,-1,-1,24,FilterMode.Point,RenderTextureFormat.Depth);

		// thickness/color, surface depth and normals buffers:
		renderFluid.GetTemporaryRT(thickness1,-2,-2,16,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);
		renderFluid.GetTemporaryRT(thickness2,-2,-2,0,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);
		renderFluid.GetTemporaryRT(smoothDepth,-1,-1,0,FilterMode.Point,RenderTextureFormat.RFloat);
		renderFluid.GetTemporaryRT(normals,-1,-1,0,FilterMode.Bilinear,RenderTextureFormat.ARGBHalf);

		// Copy screen contents to refract them later.
		renderFluid.Blit (BuiltinRenderTextureType.CurrentActive, refraction);

		renderFluid.SetRenderTarget(depth); // fluid depth
		renderFluid.ClearRenderTarget(true,true,Color.clear); //clear 
		
		// draw fluid depth texture:
		foreach(ObiParticleRenderer renderer in particleRenderers){
			if (renderer != null){
				foreach(Mesh mesh in renderer.ParticleMeshes)
					renderFluid.DrawMesh(mesh,Matrix4x4.identity,renderer.ParticleMaterial,0,0);
			}
		}

		// draw fluid thickness and color:
		renderFluid.SetRenderTarget(thickness1);
		renderFluid.ClearRenderTarget(true,true,thicknessBufferClear); 

		foreach(ObiParticleRenderer renderer in particleRenderers){
			if (renderer != null){
				foreach(Mesh mesh in renderer.ParticleMeshes){
					renderFluid.DrawMesh(mesh,Matrix4x4.identity,thickness_Material,0,0);
					renderFluid.DrawMesh(mesh,Matrix4x4.identity,colorMaterial,0,0);
				}
			}
		}

		// blur fluid thickness:
		renderFluid.Blit(thickness1,thickness2,thickness_Material,1);
		renderFluid.Blit(thickness2,thickness1,thickness_Material,2);

		// draw foam: 
		renderFluid.SetRenderTarget(foam);
		renderFluid.ClearRenderTarget(true,true,Color.clear);

		foreach(ObiParticleRenderer renderer in particleRenderers){
			if (renderer != null){
				ObiFoamGenerator foamGenerator = renderer.GetComponent<ObiFoamGenerator>();
				if (foamGenerator != null && foamGenerator.advector != null && foamGenerator.advector.Particles != null){
					ParticleSystemRenderer psRenderer = foamGenerator.advector.Particles.GetComponent<ParticleSystemRenderer>();
					if (psRenderer != null)
						renderFluid.DrawRenderer(psRenderer,psRenderer.material);
				}
			}
		}
		
		// blur fluid surface:
		renderFluid.Blit(depth,smoothDepth,depth_BlurMaterial);
		renderFluid.ReleaseTemporaryRT(depth);

		// reconstruct normals from smoothed depth:
		renderFluid.Blit(smoothDepth,normals,normal_ReconstructMaterial);
		
		// render fluid:
		renderFluid.SetGlobalTexture("_FluidDepth", depth);
		renderFluid.SetGlobalTexture("_Foam", foam);
		renderFluid.SetGlobalTexture("_Refraction", refraction);
		renderFluid.SetGlobalTexture("_Thickness",thickness1);
		renderFluid.SetGlobalTexture("_Normals",normals);
		renderFluid.Blit(smoothDepth,BuiltinRenderTextureType.CameraTarget,fluidMaterial);	

	}

	void OnPreRender(){

		bool act = gameObject.activeInHierarchy && enabled;
		if (!act || particleRenderers == null || particleRenderers.Length == 0)
		{
			Cleanup();
			return;
		}

		Setup();

	 	Camera m_Cam = GetComponent<Camera>();

		Shader.SetGlobalMatrix("_Camera_to_World",m_Cam.cameraToWorldMatrix);
		Shader.SetGlobalMatrix("_World_to_Camera",m_Cam.worldToCameraMatrix);
		Shader.SetGlobalMatrix("_InvProj",m_Cam.projectionMatrix.inverse);

		float fovY = m_Cam.fieldOfView;
        float far = m_Cam.farClipPlane;
        float y = m_Cam.orthographic ? 2 * m_Cam.orthographicSize: 2 * Mathf.Tan (fovY * Mathf.Deg2Rad * 0.5f) * far;
        float x = y * m_Cam.aspect;
		Shader.SetGlobalVector("_FarCorner",new Vector3(x,y,far));

		depth_BlurMaterial.SetFloat("_BlurScale",m_Cam.orthographic ? 1 : m_Cam.pixelWidth/m_Cam.aspect * (1.0f/Mathf.Tan(fovY * Mathf.Deg2Rad * 0.5f)));
		depth_BlurMaterial.SetFloat("_BlurRadiusWorldspace",blurRadius);
		
		if (fluidMaterial != null)
		{		
			fluidMaterial.SetFloat("_ThicknessCutoff", thicknessCutoff);
		}

		if (renderFluid == null)
		{
			renderFluid = new CommandBuffer();
			renderFluid.name = "Render fluid";
			SetupFluidRenderingCommandBuffer();
			m_Cam.AddCommandBuffer (CameraEvent.BeforeImageEffectsOpaque, renderFluid);
		}
	}
}
}

