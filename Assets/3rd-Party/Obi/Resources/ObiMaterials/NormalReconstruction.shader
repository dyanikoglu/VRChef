// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/NormalReconstruction"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{

		Pass
		{
			Name "NormalsFromDepth"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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

			float4 _MainTex_TexelSize;

			float3 NormalFromEyePos(float2 uv, float3 eyePos)
			{
				// get sample coordinates:
				float2 sx = uv + float2(_MainTex_TexelSize.x,0);
				float2 sy = uv + float2(0,_MainTex_TexelSize.y);

				float2 sx2 = uv - float2(_MainTex_TexelSize.x,0);
				float2 sy2 = uv - float2(0,_MainTex_TexelSize.y);

				// get eye space from depth at these coords, and compute derivatives:
				float3 dx = EyePosFromDepth(sx,tex2D(_MainTex, sx).x) - eyePos;
				float3 dy = EyePosFromDepth(sy,tex2D(_MainTex, sy).x) - eyePos;

				float3 dx2 = eyePos - EyePosFromDepth(sx2,tex2D(_MainTex, sx2).x);
				float3 dy2 = eyePos - EyePosFromDepth(sy2,tex2D(_MainTex, sy2).x);

				if (abs(dx.z) > abs(dx2.z))
					dx = dx2;

				if (abs(dy2.z) < abs(dy.z))
					dy = dy2;

				return normalize(cross(dx,dy));
			}	

			fixed4 frag (v2f i) : SV_Target
			{			
				float depth = tex2D(_MainTex, i.uv).r;

				// reconstruct eye space position from frustum corner and camera depth:
				float3 eyePos = EyePosFromDepth(i.uv,depth);

				// reconstruct normal from eye space position:
				float3 n = NormalFromEyePos(i.uv,eyePos);

				return fixed4(n*0.5+0.5,1);
			}
			ENDCG
		}

	}
}
