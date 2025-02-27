// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Colors_NewPosterize" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
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
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float4 _ScreenResolution;
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
float2 uv = i.texcoord.xy;
float4 c = tex2D(_MainTex, uv);

c = pow(c, float4(_Value,_Value,_Value,_Value));
c = c * _Value2;
c = floor(c);
c = c / _Value2;
float d=1.0/_Value;
c = pow(c, float4(d,d,d,d));

float4 gray = float4(.399, _Value3, .314, 1.);
float4 newC = float4(.0, .0, .0, 1.);
newC.r = dot(c.r, gray.r);
newC.g = dot(c.g, gray.g);
newC.b = dot(c.b, gray.b);

return  newC;
}
ENDCG
}
}
}
