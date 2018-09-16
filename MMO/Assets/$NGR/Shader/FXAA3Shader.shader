Shader "Hidden/GDShader/FXAA3Effect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_TexelSize;

		//
		// FXAA3 PC Console version by Timothy Lottes for nVidia
		/*
		------------------------------------------------------------------------------                       
		COPYRIGHT (C) 2010, 2011 NVIDIA CORPORATION. ALL RIGHTS RESERVED.
		------------------------------------------------------------------------------                       
		TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED 
		*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS 
		OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF 
		MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. IN NO EVENT SHALL NVIDIA 
		OR ITS SUPPLIERS BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR 
		CONSEQUENTIAL DAMAGES WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR 
		LOSS OF BUSINESS PROFITS, BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, 
		OR ANY OTHER PECUNIARY LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE 
		THIS SOFTWARE, EVEN IF NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH 
		DAMAGES.
		*/


		inline half TexLuminance( float2 uv )
		{
			fixed3 pix = tex2Dlod( _MainTex, float4(uv,0.0f,0.0f) ).rgb;
			return Luminance(pix);//采样转换灰度
		}


		half4 CombineColours(half4 col1 , half4 col2)
		{
			return ( col1 + col2 ) * 0.5f;
		}

		half4 FXAA3( float2 pos , float4 extents , float4 dimensionFrame )
		{
			half4 retColour;
			
			half4 dir;
			dir.y = 0.0f;
			
			// North-East
			half lumaNE = TexLuminance( extents.zy );
			lumaNE += 1.0f / 384.0f;//3个通道的亮度768的一半进行偏移
			dir.x = -lumaNE;
			dir.z = -lumaNE;
			
			// South-West
			half lumaSW = TexLuminance( extents.xw );
			dir.x += lumaSW;
			dir.z += lumaSW;
			
			// North-West
			half lumaNW = TexLuminance( extents.xy );
			dir.x -= lumaNW;
			dir.z += lumaNW;

			// South-East
			half lumaSE = TexLuminance( extents.zw );
			dir.x += lumaSE;
			dir.z -= lumaSE;			
			
			// early exit
			
			// calc mins and maxes
			half lumaMin = min( min( lumaNW , lumaSW ) , min( lumaNE , lumaSE ) );
			half lumaMax = max( max( lumaNW , lumaSW ) , max( lumaNE , lumaSE ) );
			
			retColour = tex2Dlod( _MainTex , float4(pos , 0.0f , 0.0f ) );
			half centrePixelLuminance = Luminance( retColour );
			half lumaMinM = min( lumaMin , centrePixelLuminance );
			half lumaMaxM = max( lumaMin , centrePixelLuminance );
			
			// and see if they're within the threshold
			
			if( ( lumaMaxM - lumaMinM ) < max( 0.05f , lumaMax * 0.125f ) )
			{
				return retColour;
			}
			
			// (my comment breaks do not align with those of the original shader)
			half4 dir1pos;
			dir1pos.xy = normalize( dir.xyz ).xz;
			half dirAbsMinTimesC = min( abs( dir1pos.x ) , abs( dir1pos.y ) ) * 4.0f;
			//
			half4 dir2pos;
			dir2pos.xy = clamp( dir1pos.xy / dirAbsMinTimesC , -2.0f , 2.0f );
			dir1pos.zw = pos.xy;
			dir2pos.zw = pos.xy;
			//
			half2 texCoords;
			texCoords = dir1pos.zw - dir1pos.xy * dimensionFrame.zw;
			half4 temp1N = tex2Dlod( _MainTex , float4( texCoords , 0.0f , 0.0f ) );
			//
			texCoords = dir1pos.zw + dir1pos.xy * dimensionFrame.zw;
			half4 col1 = tex2Dlod( _MainTex , float4( texCoords , 0.0f , 0.0f ) );
			col1 = CombineColours( temp1N , col1 );
			//
			texCoords = dir2pos.zw - dir2pos.xy * dimensionFrame.xy;
			half4 temp2N = tex2Dlod( _MainTex , float4( texCoords , 0.0f , 0.0f ) );
			//
			texCoords = dir2pos.zw + dir2pos.xy * dimensionFrame.xy;
			half4 col2 = tex2Dlod( _MainTex , float4( texCoords , 0.0f , 0.0f ) );
			col2 = CombineColours( temp2N , col2 );
			
			// combine the colours
			col2 = CombineColours( col1 , col2 );
			half col2luminance = Luminance( col2.rgb );
			
			// decide which to return
			if( col2luminance < lumaMin || col2luminance > lumaMax )
			{
				retColour = col1;
			}else{
				retColour = col2;
			}
			
			return retColour;
			
		}

		half4 frag (v2f_img i) : COLOR
		{
			// get the constants for this pixel
			
			// calculate extents
			float4 extents;
			float2 offset = ( _MainTex_TexelSize.xy ) * 0.5f;
			extents.xy = i.uv - offset;
			extents.zw = i.uv + offset;

			// and the dimension frame
			float4 dimensionFrame;
			dimensionFrame.xy = _MainTex_TexelSize.xy * 2.0f;
			dimensionFrame.zw = _MainTex_TexelSize.xy * 0.5f;
		
			half4 color = FXAA3( i.uv , extents , dimensionFrame);
			
			return color;
		}
		
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
