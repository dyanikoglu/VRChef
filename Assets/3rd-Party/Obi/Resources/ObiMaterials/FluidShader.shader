// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/FluidShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "ObiParticles.cginc"

			struct vin
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : POSITION;
			};

			v2f vert (vin v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;

				return o;
			}

			fout frag (v2f i)
			{
				fout fo;
				fo.color = fixed4(0,0,0,1);
			
				float3 eyePos,eyeNormal, worldPos, worldNormal, worldView;
				float thickness = SetupEyeSpaceFragment(i.uv,eyePos,eyeNormal);
				GetWorldSpaceFragment(eyePos,eyeNormal,worldPos,worldNormal,worldView);

				// custom fluid shading goes here

				OutputFragmentDepth(eyePos,fo);

				return fo;
			}
			ENDCG
		}		
	}
}
