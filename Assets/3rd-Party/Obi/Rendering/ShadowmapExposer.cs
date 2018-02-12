using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Obi{
public class ShadowmapExposer : MonoBehaviour
{
	Light light;
	CommandBuffer afterShadow = null;
	public ObiParticleRenderer[] particleRenderers;
 
	public void Awake(){
      light = GetComponent<Light>();
	}

	public void OnEnable(){
		Cleanup();

		afterShadow = new CommandBuffer();
		afterShadow.name = "FluidShadows";
		light.AddCommandBuffer (LightEvent.AfterShadowMapPass, afterShadow);
	}
	
	public void OnDisable(){
		Cleanup();
	}

	private void Cleanup(){

		if (afterShadow != null){
			light.RemoveCommandBuffer (LightEvent.AfterShadowMapPass,afterShadow);
			afterShadow = null;
		}
	}


	public void SetupFluidShadowsCommandBuffer()
	{
		afterShadow.Clear();
		
		if (particleRenderers == null)
		return;
		
		foreach(ObiParticleRenderer renderer in particleRenderers){
			if (renderer != null){
				foreach(Mesh mesh in renderer.ParticleMeshes)
					afterShadow.DrawMesh(mesh,Matrix4x4.identity,renderer.ParticleMaterial,0,1);
			}
		}
	}

    // Use this for initialization
	void Update()
	{
          /*m_afterShadowPass = new CommandBuffer();
          m_afterShadowPass.name = "Shadowmap Expose";
 
          //The name of the shadowmap for this light will be "MyShadowMap"
          m_afterShadowPass.SetGlobalTexture ("_MyShadowMap", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
		 
          Light light = GetComponent<Light>();
          if (light)
          {
               //add command buffer right after the shadowmap has been renderered
               light.AddCommandBuffer (UnityEngine.Rendering.LightEvent.AfterShadowMap, m_afterShadowPass);
          }*/
 

		bool act = gameObject.activeInHierarchy && enabled;
		if (!act || particleRenderers == null || particleRenderers.Length == 0)
		{
			Cleanup();
			return;
		}

		if (afterShadow != null)
		{
			//afterShadow = new CommandBuffer();
			//afterShadow.name = "FluidShadows";
			SetupFluidShadowsCommandBuffer();
			//light.AddCommandBuffer (LightEvent.AfterShadowMapPass, afterShadow);
		}	
	}
}
}
