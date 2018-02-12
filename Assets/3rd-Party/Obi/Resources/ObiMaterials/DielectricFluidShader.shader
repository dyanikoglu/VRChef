// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Obi/Fluid/DielectricFluid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Smoothness ("Smoothness", Range (0, 1)) = 0.8
		_ThicknessScale ("ThicknessScale", Range (0, 30)) = 5
		_RefractionCoeff ("Refraction", Range (-0.1, 0.1)) = 0.05
		_CloudinessColor ("CloudinessColor", Color) = (1,1,1,1)
		_Cloudiness ("Cloudiness", Range (0, 30)) = 0
	}

	SubShader
	{

		Pass
		{

			Name "DielectricFluid"
			Tags {"LightMode" = "ForwardBase"}

			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "ObiParticles.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityImageBasedLighting.cginc"

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

			sampler2D _Refraction;
			float4 _MainTex_TexelSize;

			float _ThicknessScale;
			float _RefractionCoeff;
			float _Smoothness;
			float _Cloudiness;
			half4 _CloudinessColor;

			UNITY_DECLARE_SHADOWMAP(_MyShadowMap);

			fout frag (v2f i)
			{
				fout fo;
				fo.color = fixed4(0,0,0,1);

				float3 eyePos,eyeNormal, worldPos, worldNormal, worldView;
				float thickness = SetupEyeSpaceFragment(i.uv,eyePos,eyeNormal);
				GetWorldSpaceFragment(eyePos,eyeNormal,worldPos,worldNormal,worldView);

				// directional light shadow (cascades)
				float4 viewZ = -eyePos.z;
				float4 zNear = float4( viewZ >= _LightSplitsNear );
				float4 zFar = float4( viewZ < _LightSplitsFar );
				float4 weights = zNear * zFar;

				// refection & refraction:
				Unity_GlossyEnvironmentData g;
				g.roughness	= 1-_Smoothness;
				g.reflUVW = reflect(-worldView,worldNormal);
				float3 reflection = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, g);
				fixed4 refraction = tex2D(_Refraction, i.uv + eyeNormal.xy * thickness * _RefractionCoeff);

				// lighting vectors:
				float3 lightDirWorld = normalize(_WorldSpaceLightPos0.xyz);
				half3 h = normalize( lightDirWorld + worldView );
				float nh = BlinnTerm ( worldNormal, h );
				float nl = DotClamped( worldNormal, lightDirWorld);	
				float nv = DotClamped( worldNormal, worldView);

				// absorbance, transmittance and reflectance.
				half3 absorbance = (1 - tex2D(_Thickness,i.uv).rgb) * -thickness * _ThicknessScale;
				half3 transmittance = lerp(refraction * exp(absorbance),_CloudinessColor.rgb,saturate(thickness * _Cloudiness));
				float fresnel = FresnelTerm(0.2,nv);

				// energy-conserving microfacet specular lightning:
				half V = SmithBeckmannVisibilityTerm (nl, nv, 1-_Smoothness);
				half D = NDFBlinnPhongNormalizedTerm(nh,RoughnessToSpecPower(1-_Smoothness));
    			float spec = (V * D) * (UNITY_PI/4);
				if (IsGammaSpace())
					spec = sqrt(max(1e-4h, spec));
				spec = max(0, spec * nl);

				// foam:
				fixed4 foam = tex2D(_Foam,i.uv) * half4(exp(absorbance),1);

				fo.color.rgb = lerp(transmittance,reflection,fresnel) + spec + foam;

				OutputFragmentDepth(eyePos,fo);
				return fo;
			}
			ENDCG
		}
	}
}
