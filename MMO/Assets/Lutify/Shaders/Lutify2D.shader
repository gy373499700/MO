// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/Lutify 2D"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	CGINCLUDE

		#pragma vertex vert_img
		#pragma fragment frag_2d
		#pragma fragmentoption ARB_precision_hint_fastest//快速 低精度FP16，16bit

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		// (0) - Gamma
		Pass
		{			
			CGPROGRAM
				#include "./Lutify.cginc"
			ENDCG
		}

		// (1) - Linear
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_LINEAR
				#include "./Lutify.cginc"
			ENDCG
		}

		// (2) - Gamma Split H
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_H
				#include "./Lutify.cginc"
			ENDCG
		}

		// (3) - Linear Split H
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_H
				#define LUTIFY_LINEAR
				#include "./Lutify.cginc"
			ENDCG
		}

		// (4) - Gamma Split V
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_V
				#include "./Lutify.cginc"
			ENDCG
		}

		// (5) - Linear Split V
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_V
				#define LUTIFY_LINEAR
				#include "./Lutify.cginc"
			ENDCG
		}

		// (6) - Gamma - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}

		// (7) - Linear - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_LINEAR
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}

		// (8) - Gamma Split H - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_H
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}

		// (9) - Linear Split H - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_H
				#define LUTIFY_LINEAR
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}

		// (10) - Gamma Split V - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_V
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}

		// (11) - Linear Split V - Point Filtering
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_SPLIT_V
				#define LUTIFY_LINEAR
				#define LUTIFY_FILTERING_POINT
				#include "./Lutify.cginc"
			ENDCG
		}
	}

	FallBack off
}
