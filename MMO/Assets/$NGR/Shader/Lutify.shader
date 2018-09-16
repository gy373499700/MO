

Shader "Hidden/Lutify "
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		CGINCLUDE


#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0

		ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

	
		Pass
	{ 
		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "./Lutify.cginc"
		ENDCG
	}
		Pass
	{
		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag_2d
#include "./Lutify.cginc"
		ENDCG
	}


		
	}

		FallBack off
}
