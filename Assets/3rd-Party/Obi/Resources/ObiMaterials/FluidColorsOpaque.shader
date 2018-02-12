Shader "Obi/Fluid/Colors/FluidColorsOpaque"
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
				float4 viewpos : TEXCOORD1;
			};

			fixed4 _Color;

			v2f vert(vin v)
			{ 
				v2f o;
				o.viewpos = mul(UNITY_MATRIX_V, v.vertex) + float4(v.corner.x, v.corner.y, 0, 0) * v.corner.z; // multiply by size.
				o.pos = mul(UNITY_MATRIX_P, o.viewpos);
				o.texcoord = float3(v.corner.x*0.5+0.5, v.corner.y*0.5+0.5, v.corner.z);
				o.color = v.color;
				return o;
			} 

			fout frag(v2f i)
			{
				fout fo;
				fo.color =  half4(i.color.rgb * _Color.rgb,0); 

				// generate sphere normals:
				float3 n = BillboardSphereNormals(i.texcoord);
				OutputFragmentDepth(float4(i.viewpos + n * i.texcoord.z, 1.0f),fo);

				return fo;
			}
			 
			ENDCG

		}
	} 
FallBack "Diffuse"
}
