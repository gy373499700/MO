Shader "Hidden/GDShader/DeferredLighting" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		pass{
			ZTest Equal
			Cull[CullMode]
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "deferred_lighting.cg"
			ENDCG
		}
	} 

	SubShader {
		Tags { "RenderType"="OpaqueBump" }
		LOD 200
		pass{
			ZTest Equal
			Cull[CullMode]
			CGPROGRAM
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "UnityCG.cginc"
			#include "deferred_lighting.cg"


			ENDCG
		}
	} 

	SubShader {
		Tags { "RenderType"="AlphaTest" }
		LOD 200
		pass{
			ZTest Equal
			Cull[CullMode]
			CGPROGRAM
			#define ALPHA_TEST 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "deferred_lighting.cg"
			

			ENDCG
		}
	} 
	
	SubShader {
		Tags { "RenderType"="AlphaTestBlack" }
		LOD 200
		pass{
			ZTest Equal
			Cull Off
			CGPROGRAM
			#define ALPHA_TEST_BLACK 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "deferred_lighting.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpAlphaTest" }
		LOD 200
		pass{
		ZTest Equal
			Cull Off
			CGPROGRAM
			#define ALPHA_TEST 1
			#define HAS_BUMP 1
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "deferred_lighting.cg"
			

			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="BumpOIT" }
		LOD 200
		pass{
		ZTest Equal
			Cull Off
			CGPROGRAM
			#define OIT 1
			#define HAS_BUMP 1

			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "deferred_lighting.cg"
			
			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="OIT" }
		LOD 200
		pass{
		ZTest Equal
			Cull Off
			CGPROGRAM
			#define OIT 1


			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			//#define SHADER_API_GLES
			#include "deferred_lighting.cg"
			
			ENDCG
		}
	} 
	SubShader{
		Tags{ "RenderType" = "OITGrass" }
		LOD 200
		pass {
		ZTest Equal
			Cull Off
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag

			#define ALPHA_TEST 1
			//#define OIT 1
			#define FORCE_FIELD 1
			#pragma target 3.0


			#include "deferred_lighting.cg"


			ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4M" }
		LOD 200
		pass {
			ZTest Equal
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "deferred_lighting.cg"
		ENDCG
		}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueT4MAO" }
		LOD 200
		pass {
			ZTest Equal
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "deferred_lighting.cg"
		ENDCG
			}
	}
	SubShader{
		Tags{ "RenderType" = "OpaqueFace" }
		LOD 200
		pass {
			ZTest Equal
			CGPROGRAM
			#pragma vertex gbuffer_vert
			#pragma fragment gbuffer_frag
			#include "deferred_lighting.cg"
			ENDCG
		}
	}
	/*SubShader{
		Tags{ "RenderType" = "OpaqueFaceInUI" }
		LOD 200
		pass {
		ZTest Equal
			CGPROGRAM
		#pragma vertex gbuffer_vert
		#pragma fragment gbuffer_frag
		#include "gbuffer.cg"
			ENDCG
		}
	}*/
	FallBack "Diffuse"
}
