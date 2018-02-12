Shader "Obi/Fluid/Colors/FluidColorsBlend"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Particle color", Color) = (1,1,1,1)
	}

	SubShader { 

		Pass { 
			Name "FluidColors"
			Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
			
			Blend DstColor Zero
			ZWrite Off
			ColorMask RGB

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "ObiParticles.cginc"

			struct vin{
				float4 vertex   : POSITION;
				float3 corner   : NORMAL;
				fixed4 color    : COLOR;
				float3 texcoord  : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos   : POSITION;
				fixed4 color    : COLOR;
				float3 texcoord  : TEXCOORD0;
				float4 projPos : TEXCOORD1;
				float4 viewpos : TEXCOORD2;
			};

			fixed4 _Color;

			v2f vert(vin v)
			{ 
				v2f o;
				o.viewpos = mul(UNITY_MATRIX_V, v.vertex) + float4(v.corner.x, v.corner.y, 0, 0) * v.corner.z; // multiply by size.
				o.pos = mul(UNITY_MATRIX_P, o.viewpos);
				o.projPos = ComputeScreenPos(o.pos);
				o.texcoord = float3(v.corner.x*0.5+0.5, v.corner.y*0.5+0.5, v.corner.z);
				o.color = v.color;
				return o;
			} 

			fout frag(v2f i)
			{
				fout fo;
				fo.color = i.color * _Color; 

				// discard fragment if occluded by the scene:
				float sceneDepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.projPos)).r);

				if (sceneDepth < i.projPos.z)
					discard;

				// generate colored sphere:
				float3 n = BillboardSphereNormals(i.texcoord);
				OutputFragmentDepth(float4(i.viewpos + n * i.texcoord.z, 1.0f),fo);
			
				return fo;
			}
			 
			ENDCG

		}
	} 
FallBack "Diffuse"
}
