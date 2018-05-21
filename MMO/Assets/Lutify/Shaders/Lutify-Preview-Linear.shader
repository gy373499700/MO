// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/Lutify-Preview-Linear"
{
	Properties
	{
		_MainTex ("Texture", Any) = "white" {}
	}

	SubShader
	{
		Tags { "ForceSupported" = "True" }
		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off 
		ZWrite Off 
		ZTest Always

		Pass
		{			
			CGPROGRAM
				#pragma vertex vert_preview
				#pragma fragment frag_linear
				#include "./Lutify-Preview.cginc"
			ENDCG
		}
	}

	FallBack off
}
