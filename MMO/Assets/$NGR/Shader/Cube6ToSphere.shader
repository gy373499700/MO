// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GDShader/Cube6ToSphere" {
	Properties {
		CubeToSphere("CubeToSphere", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			//ZTest Always
			ZWrite Off
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"

			sampler2D CubeFace0;//Z
			sampler2D CubeFace1;//X
			sampler2D CubeFace2;//-Z
			sampler2D CubeFace3;//-X
			sampler2D CubeFace4;//Y
			sampler2D CubeFace5;//-Y
			sampler2D CubeToSphere;

			float4 invViewport_Radius;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			float4 frag(v2f IN) : COLOR
			{
				float4 uv_index = tex2D(CubeToSphere,IN.uv);
				float id = uv_index.z*6.0f;
				if (id < 0.5f)
				{
					return tex2D(CubeFace0, uv_index.xy);
				}
				else if (id < 1.5f)
				{
					return tex2D(CubeFace1, uv_index.xy);
				}
				else if (id < 2.5f)
				{
					return tex2D(CubeFace2, uv_index.xy);
				}
				else if (id < 3.5f)
				{
					return tex2D(CubeFace3, uv_index.xy);
				}
				else if (id < 4.5f)
				{
					return tex2D(CubeFace4, uv_index.xy);
				}
				else if (id < 5.5f)
				{
					return tex2D(CubeFace5, uv_index.xy);
				}

				return 0;
			}
			ENDCG
		}
	pass {
		ZTest Always
			ZWrite Off
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "LightCommon.cg"


		samplerCUBE CubeMap;
		sampler2D SkyTexture;
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		float4 frag(v2f IN) : COLOR
		{
			float2 uv = IN.uv;
			uv.y = 1 - uv.y;
			float3 normal = Sphere2Normal(uv);
			float4 cube = texCUBE(CubeMap, normal);

			return cube;// lerp(sky, cube, cube.w);
		}
			ENDCG
	}
	} 
	
	FallBack "Diffuse"
}
