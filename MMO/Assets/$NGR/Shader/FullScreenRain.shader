Shader "Hidden/GDShader/FullScreenRain"
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
			ZTest	Greater
			ZWrite	Off
			Cull Off
			ColorMask RGB
			Blend One One//MinusSrcAlpha 
			CGPROGRAM

			
			#pragma vertex vert
			#pragma fragment frag   
			
			#include "Rain.cg"
			ENDCG
		}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 1
#include "Rain.cg"
		ENDCG
	}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 2
#include "Rain.cg"
		ENDCG
	}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 3
#include "Rain.cg"
		ENDCG
	}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 4
#include "Rain.cg"
		ENDCG
	}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 5
#include "Rain.cg"
		ENDCG
	}
		Pass
	{
		ZTest	Greater
		ZWrite	Off
		Cull Off
		ColorMask RGB
		Blend One One
		CGPROGRAM


#pragma vertex vert
#pragma fragment frag
#define LIGHT_COUNT 6
#include "Rain.cg"
		ENDCG
	}
	}
}
