#ifndef OBIPARTICLES_INCLUDED
#define OBIPARTICLES_INCLUDED

#include "UnityCG.cginc"
#include "UnityStandardUtils.cginc"
#include "AutoLight.cginc"

float4x4 _Camera_to_World;
float4x4 _World_to_Camera;
float4x4 _InvProj;

float3 _FarCorner;

float _ThicknessCutoff;

sampler2D _MainTex;
sampler2D _Foam;
sampler2D _Thickness;
sampler2D _Normals;
sampler2D _CameraDepthTexture;

struct fout {
 	half4 color : COLOR;
	float depth : DEPTH;
};

float3 BillboardSphereNormals(float2 texcoords)
{
	float3 n;
	n.xy = texcoords*2.0-1.0;
	float r2 = dot(n.xy, n.xy);
	clip (1 - r2);   // clip pixels outside circle
	n.z = sqrt(1.0 - r2);
	return n;
}

float BillboardSphereThickness(float2 texcoords)
{
	float2 n = texcoords*2.0-1.0;
	float r2 = dot(n.xy, n.xy);
	clip (1 - r2);   // clip pixels outside circle
	return sqrt(1.0 - r2)*2.0f*exp(-r2*2.0f);
}

half3 SampleSphereAmbient(float3 eyeNormal, float3 eyePos)
{
	#if UNITY_SHOULD_SAMPLE_SH
		half3 worldNormal = mul(transpose((float3x3)UNITY_MATRIX_V),eyeNormal);  
		half3 worldPos = mul(_Camera_to_World,half4(eyePos,1.0));  	
		return ShadeSHPerPixel(half4(worldNormal, 1.0),half3(0,0,0),worldPos);
	#else
		return UNITY_LIGHTMODEL_AMBIENT;
	#endif
}

float Z2EyeDepth(float z) {
    if (unity_OrthoParams.w < 0.5)
        return LinearEyeDepth(z);
	else{

		// since we're not using LinearEyeDepth in orthographic, we must reverse depth direction ourselves:
		#if UNITY_REVERSED_Z 
			z = 1-z;
		#endif

        return ((_ProjectionParams.z - _ProjectionParams.y) * z + _ProjectionParams.y);
	}
}

// returns eye space position from linear eye depth.
float3 EyePosFromDepth(float2 uv,float eyeDepth){

	if (unity_OrthoParams.w < 0.5){
		float3 ray = (float3(-0.5f,-0.5f,0) + float3(uv,-1)) * _FarCorner;
		return ray * eyeDepth / _FarCorner.z;
	}else{
		return float3((uv-half2(0.5f,0.5f)) * _FarCorner.xy,-eyeDepth);
	}
}

float SetupEyeSpaceFragment(in float2 uv, out float3 eyePos, out float3 eyeNormal)
{
	float eyeZ = tex2D(_MainTex, uv).r; // we expect linear depth here.
	float thickness = tex2D(_Thickness,uv).a;

	if (thickness * 10 < _ThicknessCutoff)
		discard;

	// reconstruct eye space position/direction from frustum corner and camera depth:
	eyePos = EyePosFromDepth(uv,eyeZ);

	// get normal from texture: 
	eyeNormal = (tex2D(_Normals,uv)-0.5) * 2;

	return thickness;
}

void GetWorldSpaceFragment(in float3 eyePos, in float3 eyeNormal, 
						   out float3 worldPos, out float3 worldNormal, out float3 worldView)
{
	// Get world space position, normal and view direction:
	worldPos 	= mul(_Camera_to_World,half4(eyePos,1)).xyz;
	worldNormal = mul((float3x3)_Camera_to_World,eyeNormal);
	worldView   = normalize(UnityWorldSpaceViewDir(worldPos.xyz));
}

void OutputFragmentDepth(in float3 eyePos, inout fout fo)
{
	float4 clipPos = mul(unity_CameraProjection,float4(eyePos,1));
	fo.depth = clipPos.z/clipPos.w;

	fo.depth = 0.5*fo.depth + 0.5;

	// DX11 and some other APIs make use of reverse zbuffer since 5.5. Must inverse value before outputting.
	#if UNITY_REVERSED_Z 
		fo.depth = 1-fo.depth;
	#endif
}

#endif
