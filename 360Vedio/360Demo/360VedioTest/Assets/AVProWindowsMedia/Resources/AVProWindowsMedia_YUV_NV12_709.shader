Shader "Hidden/AVProWindowsMedia/CompositeNV12_709"
{
	Properties 
	{
		_MainTex ("Luma (Y)", 2D) = "white" {}
		_MainUV ("Color (UV)", 2D) = "white" {}
		_TextureWidth ("Texure Width", Float) = 256.0
	}
	SubShader 
	{
		Pass
		{ 
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#pragma multi_compile SWAP_RED_BLUE_ON SWAP_RED_BLUE_OFF
#include "AVProWindowsMedia_Shared.cginc"
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _MainUV;
float _TextureWidth;
#if UNITY_VERSION >= 530
uniform float4 _MainTex_ST2;
#else
uniform float4 _MainTex_ST;
#endif
float4 _MainTex_TexelSize;

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

#if UNITY_VERSION >= 530
	o.uv.xy = (v.texcoord.xy * _MainTex_ST2.xy + _MainTex_ST2.zw);
#else
	o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif

	// On D3D when AA is used, the main texture & scene depth texture
	// will come out in different vertical orientations.
	// So flip sampling of the texture when that is the case (main texture
	// texel size will have negative Y).
	#if SHADER_API_D3D9
	if (_MainTex_TexelSize.y < 0)
	{
		o.uv.y = 1-o.uv.y;
	}
	#endif
	

	return o;
}

float4 frag (v2f i) : COLOR
{
	float yy = i.uv.y * 0.666666;
	float2 uv = float2(i.uv.x, yy);
	float4 col = tex2D(_MainTex, uv.xy);
	
	float f = (uv.x * _TextureWidth) / 1.0;
	float fr = frac(f);
	float y = col.r;
	if (fr >= 0.25)
		y = col.g;
	if (fr >= 0.5)
		y = col.b;
	if (fr >= 0.75)
		y = col.a;
	
	
	//uv = float2(i.uv.x, 0.66666 + yy * 0.5);//0.666666 + i.uv.y*0.666666);
	col = tex2D(_MainTex, uv);
	
	return col;
} 

ENDCG
		}
	}
	
	FallBack Off
}