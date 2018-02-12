Shader "Obi/Particles" {

Properties {
	_Color ("Particle color", Color) = (1,1,1,1)
}

	SubShader { 

		Pass { 

			Name "ParticleFwdBase"
			Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" "LightMode" = "ForwardBase"}
			Blend SrcAlpha OneMinusSrcAlpha  
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile_fwdbase

			#include "ObiParticles.cginc"

			struct vin{
				float4 vertex   : POSITION;
				float3 corner   : NORMAL;
				fixed4 color    : COLOR;
				float3 texcoord  : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos   : SV_POSITION;
				fixed4 color    : COLOR;
				float3 texcoord  : TEXCOORD0;
				float3 lightDir : TEXCOORD1;
				float4 viewpos : TEXCOORD2;
				LIGHTING_COORDS(3,4)
			};

			v2f vert(vin v)
			{ 
				v2f o;
		
				// particle positions are passed in world space, no need to use modelview matrix, just view.
				o.viewpos = mul(UNITY_MATRIX_V, v.vertex) + float4(v.corner.x, v.corner.y, 0, 0) * v.corner.z; // multiply by size.
				o.pos = mul(UNITY_MATRIX_P, o.viewpos);
				o.texcoord = float3(v.corner.x*0.5+0.5, v.corner.y*0.5+0.5, v.corner.z);
				o.color = v.color;

				o.lightDir = mul ((float3x3)UNITY_MATRIX_MV, ObjSpaceLightDir(v.vertex));

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			} 

			fixed4 _Color;
			fixed4 _LightColor0; 

			fout frag(v2f i) 
			{
				fout fo;

				fo.color =  half4(0,0,0,1); 

				// generate sphere normals:
				float3 n = BillboardSphereNormals(i.texcoord);

				// update fragment position:
				float sphereRadius = i.texcoord.z;
				float4 pixelPos = float4(i.viewpos + n * sphereRadius, 1.0f); 

				// clip space position:
				float4 pos = mul(UNITY_MATRIX_P,pixelPos);

				// simple lighting: ambient
				half3 amb = SampleSphereAmbient(n,pixelPos);

				// simple lighting: diffuse
		   	 	float ndotl = saturate( dot( n, normalize(i.lightDir) ) );
				UNITY_LIGHT_ATTENUATION(atten,i,0);

				// final lit color:
				fo.color.rgb = _Color * i.color * (_LightColor0 * ndotl * atten + amb);

				// normalized device coordinates:
				fo.depth = pos.z/pos.w;

				// in openGL calculated depth range is <-1,1> map it to <0,1>
				#if SHADER_API_OPENGL || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3	 
					fo.depth = 0.5*fo.depth + 0.5;
				#endif
			
				return fo;
			}
			 
			ENDCG

		} 

		Pass {
        	Name "ShadowCaster"
		        Tags { "LightMode" = "ShadowCaster" }
		        Offset 1, 1
		       
		        Fog {Mode Off}
		        ZWrite On ZTest LEqual
		 
				CGPROGRAM

				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#pragma multi_compile_shadowcaster

				#include "ObiParticles.cginc"
				 
				struct vin{
					float4 vertex   : POSITION;
					float3 corner   : NORMAL;
					float2 texcoord  : TEXCOORD0;
				};

				struct v2f {
				    float3 texcoord : TEXCOORD0;
					float4 viewpos : TEXCOORD1;
				};

				sampler3D  _DitherMaskLOD;
				 
				v2f vert( vin v , out float4 outpos : SV_POSITION )// clip space position output
				{
				    v2f o;

					o.viewpos = mul(UNITY_MATRIX_V, v.vertex) + float4(v.corner.x, v.corner.y, 0, 0) * v.corner.z;
					outpos = mul(UNITY_MATRIX_P, o.viewpos);
					o.texcoord = float3(v.corner.x*0.5+0.5, v.corner.y*0.5+0.5, v.corner.z);
				    return o;
				}
				 
				fout frag( v2f i , UNITY_VPOS_TYPE vpos : VPOS) 
				{
					fout fo;

					float3 n = BillboardSphereNormals(i.texcoord);

					// update fragment position:
					float sphereRadius = i.texcoord.z;
					float4 pixelPos = float4(i.viewpos + n * sphereRadius, 1.0f); 
	
					// project camera space position.
					float4 pos = UnityApplyLinearShadowBias( mul(UNITY_MATRIX_P,pixelPos) );

					fo.color = pos.z/pos.w; //similar to what SHADOW_CASTER_FRAGMENT does in case there's no depth buffer.
					fo.depth = pos.z/pos.w; 

					// in openGL calculated depth range is <-1,1> map it to <0,1>
					#if SHADER_API_OPENGL || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3
						fo.depth = fo.depth*0.5+0.5;
					#endif

	                // Use dither mask for alpha blended shadows, based on pixel position xy
	                // and alpha level. Our dither texture is 4x4x16.
				    float alpha = 1.0f; //TODO: give user control over this.
	                half alphaRef = tex3D(_DitherMaskLOD, float3(vpos.xy*0.25,alpha*0.9375)).a;
	                clip (alphaRef - 0.01);
           
					return fo;
				}
				ENDCG
		 
		    }

	} 
FallBack "Diffuse"
}

