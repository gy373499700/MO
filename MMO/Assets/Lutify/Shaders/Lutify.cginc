// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#include "UnityCG.cginc"

sampler2D _MainTex;
sampler3D _LookupTex3D;
sampler2D _LookupTex2D;
float4 _Params;

/*
 * sRGB <-> Linear from http://entropymine.com/imageworsener/srgbformula/
 * using a bit more precise values than the IEC 61966-2-1 standard
 * see http://en.wikipedia.org/wiki/SRGB for more information
 */
float3 sRGB(float3 color)
{
	color = (color <= float3(0.0031308, 0.0031308, 0.0031308)) ? color * 12.9232102 : 1.055 * pow(color, 0.41666) - 0.055;
	return color;
}

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

float4 lookup_gamma(float4 o)
{
	o.rgb = tex3D(_LookupTex3D, o.rgb * _Params.x + _Params.y).rgb;
	return o;
}

float4 lookup_linear(float4 o)
{
	o.rgb = tex3D(_LookupTex3D, sRGB(o.rgb) * _Params.x + _Params.y).rgb;
	return Linear(o);
}

float4 frag(v2f_img i) : SV_Target
{
	float4 color = saturate(tex2D(_MainTex, i.uv));
	float4 x;

	#if defined(LUTIFY_LINEAR)
		x = lookup_linear(color);
	#else
		x = lookup_gamma(color);
	#endif

	#if defined(LUTIFY_SPLIT_H)
	return i.uv.x > 0.5 ? lerp(color, x, _Params.z) : color;
	#elif defined(LUTIFY_SPLIT_V)
	return i.uv.y > 0.5 ? lerp(color, x, _Params.z) : color;
	#else
	return lerp(color, x, _Params.z);
	#endif
}

float4 internal_tex3d(sampler2D tex, float4 uv, float2 pixelsize, float tilewidth)
{
	uv.y = 1.0 - uv.y;
	uv.z *= tilewidth;
	float shift = floor(uv.z);
	uv.xy = uv.xy * tilewidth * pixelsize + 0.5 * pixelsize;
	uv.x += shift * pixelsize.y;

	#if defined(LUTIFY_FILTERING_POINT)
		float w = step(0.5, uv.z - shift);
	#else
		float w = uv.z - shift;
	#endif

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

float4 frag_2d(v2f_img i) : SV_Target
{
	float4 color = saturate(tex2D(_MainTex, i.uv));
	float4 x;

	#if defined(LUTIFY_LINEAR)
		x = lookup_linear_2d(color);
	#else
		x = lookup_gamma_2d(color);
	#endif

	#if defined(LUTIFY_SPLIT_H)
	return i.uv.x > 0.5 ? lerp(color, x, _Params.w) : color;
	#elif defined(LUTIFY_SPLIT_V)
	return i.uv.y > 0.5 ? lerp(color, x, _Params.w) : color;
	#else
	return lerp(color, x, _Params.w);
	#endif
}