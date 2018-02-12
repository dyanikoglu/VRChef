// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ScreenSpaceCurvatureFlow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "ObiParticles.cginc"	

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			//sampler2D_float _MainTex;
			float4 _MainTex_TexelSize;
			float _BlurScale;
			float _BlurRadiusWorldspace;

			float4 frag (v2f i) : SV_Target
			{
				float maxBlurRadius = 8.0;
				float blurDepthFalloff = 5.5;

				//discontinuities between different tap counts are visible. to avoid this we 
				//use fractional contributions between #taps = ceil(radius) and floor(radius) 

				float depth = Z2EyeDepth(SAMPLE_DEPTH_TEXTURE(_MainTex, i.uv));

				float radiusScaling = unity_OrthoParams.w < 0.5f ? depth : 0.0001;
				float radius = min(maxBlurRadius, _BlurScale * (_BlurRadiusWorldspace / radiusScaling));
				float radiusInv = 1.0/(radius+0.0001);
				float taps = ceil(radius);
				float frac = taps - radius;

				float sum = 0;
				float wsum = 0; 
				float count = 0;

				for(float  y= -taps; y <= taps; y += 1.0)
				{
			        for(float x = -taps; x <= taps; x += 1.0)
					{
						float sample = Z2EyeDepth(SAMPLE_DEPTH_TEXTURE_LOD(_MainTex, float4(i.uv + float2(_MainTex_TexelSize.x*x,_MainTex_TexelSize.y*y),0,0)));

						float r1 = length(float2(x, y)) * radiusInv;
						float w = exp(-(r1*r1));

						float r2 = (sample - depth) * blurDepthFalloff;
            			float g = exp(-(r2*r2));

						float wBoundary = step(radius, max(abs(x), abs(y)));
						float wFrac = 1.0 - wBoundary*frac;

						sum += sample * w * g * wFrac;
						wsum += w * g * wFrac;
						count += g * wFrac;
					}
				}
			
				if (wsum > 0)
					sum /= wsum;

				float blend = count/pow(2.0*radius+1.0,2);
				return lerp(depth, sum, blend); // we output linear eye depth.
					
			}
			ENDCG
		}
	}
}
