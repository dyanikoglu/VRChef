Shader "Obi/FlatSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float4 tangent;
			float3 normal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert (inout appdata_full v, out Input o) {
           	UNITY_INITIALIZE_OUTPUT(Input,o);

			o.normal = normalize(UnityObjectToWorldNormal(v.normal));
  			o.tangent.xyz = normalize(UnityObjectToWorldDir(v.tangent.xyz));
  			o.tangent.w = v.tangent.w * unity_WorldTransformParams.w;

     	}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			// build tangent rotation matrix here (saves one interpolator):
			float3 binormal = cross( IN.normal, IN.tangent.xyz) * IN.tangent.w; 
			float3x3 rotation = float3x3( IN.tangent.xyz, binormal, IN.normal );
			
			// get world space normal from position derivatives, and transform it to tangent space:
			half3 worldNormal = -normalize(cross(ddx(IN.worldPos), ddy(IN.worldPos)));
			o.Normal = mul(rotation,worldNormal);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
