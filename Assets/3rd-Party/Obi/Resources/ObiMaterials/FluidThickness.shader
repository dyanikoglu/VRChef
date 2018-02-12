// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FluidThickness"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader { 

		Pass { 
			Name "FluidThickness"
			Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
			
			Blend One One  
			ZWrite Off
			ColorMask A

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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
				float4 texcoord  : TEXCOORD0;
				float4 projPos : TEXCOORD1;
			};

			v2f vert(vin v)
			{ 
				v2f o;
				float4 worldpos = mul(UNITY_MATRIX_MV, v.vertex) + float4(v.corner.x, v.corner.y, 0, 0) * v.corner.z;
				o.pos = mul(UNITY_MATRIX_P, worldpos);
				o.projPos = ComputeScreenPos(o.pos);
				o.texcoord = float4(v.corner.x*0.5+0.5, v.corner.y*0.5+0.5, v.corner.z,0);
				o.color = v.color;
				COMPUTE_EYEDEPTH(o.texcoord.w);
				return o;
			} 

			float4 frag(v2f i) : SV_Target
			{
				float sceneDepth = Z2EyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos)).r);

				// compare scene depth with particle depth
				if (sceneDepth < i.texcoord.w)
					discard;

				return BillboardSphereThickness(i.texcoord) * i.texcoord.z * i.color.a;
			}
			 
			ENDCG

		}

		Pass { 
			Name "ThicknessHorizontalBlur"

			Cull Off ZWrite Off ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float4 frag(v2f i) : SV_Target
			{
				float2 offset = float2(_MainTex_TexelSize.x,0);

				half4 sample1 = tex2D(_MainTex,i.uv+offset*3) * .006;
				half4 sample2 = tex2D(_MainTex,i.uv+offset*2) * .061;
				half4 sample3 = tex2D(_MainTex,i.uv+offset) * .242;
				half4 sample4 = tex2D(_MainTex,i.uv) * .383;
				half4 sample5 = tex2D(_MainTex,i.uv-offset) * .242;
				half4 sample6 = tex2D(_MainTex,i.uv-offset*2) * .061;
				half4 sample7 = tex2D(_MainTex,i.uv-offset*3) * .006;

				return sample1 + sample2 + sample3 + sample4 + sample5 + sample6 + sample7;
			}
			 
			ENDCG

		}

		Pass { 

			Name "ThicknessVerticalBlur"

			Cull Off ZWrite Off ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;	

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 offset = float2(0,_MainTex_TexelSize.y);

				half4 sample1 = tex2D(_MainTex,i.uv+offset*3) * .006;
				half4 sample2 = tex2D(_MainTex,i.uv+offset*2) * .061;
				half4 sample3 = tex2D(_MainTex,i.uv+offset) * .242;
				half4 sample4 = tex2D(_MainTex,i.uv) * .383;
				half4 sample5 = tex2D(_MainTex,i.uv-offset) * .242;
				half4 sample6 = tex2D(_MainTex,i.uv-offset*2) * .061;
				half4 sample7 = tex2D(_MainTex,i.uv-offset*3) * .006;

				return sample1 + sample2 + sample3 + sample4 + sample5 + sample6 + sample7;
			}
			 
			ENDCG

		}

	} 
FallBack "Diffuse"
}
