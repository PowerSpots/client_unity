﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
Shader "CameraFilterPack/FX_Dot_Circle" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(4.0, 32.0)) = 7.0
}
SubShader 
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Value;
struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}

float4 frag (v2f i) : COLOR
{
const int size = (int)_Value;
const float radius = size * 0.5 * 0.75;
float2 quadPos 		= floor(i.texcoord.xy * _ScreenResolution.xy  / (float)size) * (float)size;
float2 quad 		= quadPos/_ScreenResolution.xy;
float2 quadCenter 	= (quadPos + size/2.0);
float  dist 	 	= length(quadCenter - i.texcoord.xy * _ScreenResolution.xy);	
float4 texel 		= tex2D(_MainTex, quad);

float4 color = texel;
if (dist > radius)
{
color = float4(0.0,0.0,0.0,0.0);
}

return color;	
}

ENDCG
}

}
}