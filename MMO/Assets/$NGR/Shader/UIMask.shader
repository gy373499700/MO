Shader "Unlit/UIMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			
			Stencil
			{
				Ref 64
				ReadMask 64
				Comp NotEqual
				Pass Replace
			}
			ColorMask 0
			Cull off//must
			ZTest Less//Off//must
			ZWrite  Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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
			float4 _MainTex_ST;
			float4 _Offset;//xy is center offset,zw is wh scale
			float _WhRate;
			v2f vert (appdata v)
			{
				v2f o; 
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.vertex = float4(v.vertex.x * 2, v.vertex.y * 2,0, 1);
				o.vertex *= float4(_Offset.z, _Offset.w, 1, 1);
				o.vertex += float4(_Offset.x, _Offset.y, 0, 0);
#else
				//Editor Only 
				o.vertex = float4(v.vertex.x * 2, -v.vertex.y * 2,0, 1);
				o.vertex *= float4(_Offset.z, _Offset.w, 1, 1);
				o.vertex += float4(_Offset.x , -_Offset.y, 0, 0);

#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				return float4(1,0,0,0);
			}
			ENDCG
		}


		Pass
			{

				Stencil
			{
				Ref 64
				ReadMask 64
				Comp NotEqual
				Pass Replace
			}
				ColorMask RGB
				Cull off//must
				ZTest Less
				ZWrite Off

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

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
			float4 _MainTex_ST;
			float4 _Offset;//xy is center offset,zw is wh scale
			float _WhRate;
			v2f vert(appdata v)
			{
				v2f o;
#if SHADER_API_GLES|| SHADER_API_GLES3
				//Mobile
				o.vertex = float4(v.vertex.x * 2, v.vertex.y * 2, 0, 1);
				o.vertex *= float4(_Offset.z, _Offset.w, 1, 1);
				o.vertex += float4(_Offset.x, _Offset.y, 0, 0);
#else
				//Editor Only 
				o.vertex = float4(v.vertex.x * 2, -v.vertex.y * 2, 0, 1);
				o.vertex *= float4(_Offset.z, _Offset.w, 1, 1);
				o.vertex += float4(_Offset.x , -_Offset.y, 0, 0);

#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				return float4(1,0,0,0);
			}
				ENDCG
			}
	}
}
