using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

// Should be attached to all emitter objects in scene. Automatically registers itself to camera component to be rendered in runtime.
public class AutoRegisterEmitter : MonoBehaviour {
    public int particleRendererMaxCount = 64;
    public Camera mainCam;

	// Use this for initialization
	void Start () {
        ObiFluidRenderer ofr = mainCam.GetComponent<ObiFluidRenderer>();

        if (ofr)
        {
            for (int i = 0; i < particleRendererMaxCount; i++)
            {
                if (ofr.particleRenderers[i] == null)
                {
                    ofr.particleRenderers[i] = this.GetComponent<ObiParticleRenderer>();
                    break;
                }
            }
        }
	}
}
