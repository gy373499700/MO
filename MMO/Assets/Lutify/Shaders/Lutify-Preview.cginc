// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_GUIClipTextureMatrix' with 'unity_GUIClipTextureMatrix'

// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _LookupTex2D;
float4 _Params;
		
struct appdata_t
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 vertex : SV_POSITION;
	float4 color : COLOR;
	float2 texcoord : TEXCOORD0;
	float2 clipUV : TEXCOORD1;
};
		
float4 _MainTex_ST;
float4 _Color;
float4x4 unity_GUIClipTextureMatrix;
		
sampler2D _GUIClipTexture;
float _BottomFrame;
float4 _FrameColor;

v2f vert_preview(appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	float4 eyePos = mul(UNITY_MATRIX_MV, v.vertex);
	o.clipUV = mul(unity_GUIClipTextureMatrix, eyePos);
	o.color = v.color;
	o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	return o;
}

/*
 * sRGB <-> Linear from http://entropymine.com/imageworsener/srgbformula/
 * using a bit more precise values than the IEC 61966-2-1 standard
 * see http://en.wikipedia.org/wiki/SRGB for more information
 */
float4 sRGB(float4 color)
{
	color.rgb = (color.rgb <= float3(0.0031308, 0.0031308, 0.0031308)) ? color.rgb * 12.9232102 : 1.055 * pow(color.rgb, 0.41666) - 0.055;
	return color;
}

float4 Linear(float4 color)
{
	color.rgb = (color.rgb <= float3(0.0404482, 0.0404482, 0.0404482)) ? color.rgb / 12.9232102 : pow((color.rgb + 0.055) * 0.9478672, 2.4);
	return color;
}

float4 internal_tex3d(sampler2D tex, float4 uv, float2 pixelsize, float tilewidth)
{
	uv.y = 1.0 - uv.y;
	uv.z *= tilewidth;
	float shift = floor(uv.z);
	uv.xy = uv.xy * tilewidth * pixelsize + 0.5 * pixelsize;
	uv.x += shift * pixelsize.y;
	float w = (_Params.w >= 1) ? step(0.5, uv.z - shift) : uv.z - shift;
	uv.xyz = lerp(tex2D(tex, uv.xy).rgb, tex2D(tex, uv.xy + float2(pixelsize.y, 0)).rgb, w);
	return uv;
}

float4 lookup_gamma_2d(float4 o)
{
	o = internal_tex3d(_LookupTex2D, o, _Params.xy, _Params.z);
	return o;
}

float4 lookup_linear_2d(float4 o)
{
	o = internal_tex3d(_LookupTex2D, sRGB(o), _Params.xy, _Params.z);
	return Linear(o);
}

float4 name_rect(float4 c, float v)
{
	c.rgb = lerp(_FrameColor.rgb, c.rgb, step(_BottomFrame, v));
	return c;
}

float4 frag_gamma(v2f i) : SV_Target
{
	float4 col;
	col.rgb = saturate(tex2D(_MainTex, i.texcoord).rgb * i.color.rgb);
	col.a = i.color.a * tex2D(_GUIClipTexture, i.clipUV).a;
	return name_rect(lookup_gamma_2d(col), i.texcoord.y);
}

float4 frag_linear(v2f i) : SV_Target
{
	float4 col;
	col.rgb = saturate(tex2D(_MainTex, i.texcoord).rgb * i.color.rgb);
	col.a = i.color.a * tex2D(_GUIClipTexture, i.clipUV).a;
	return name_rect(lookup_linear_2d(col), i.texcoord.y);
}
