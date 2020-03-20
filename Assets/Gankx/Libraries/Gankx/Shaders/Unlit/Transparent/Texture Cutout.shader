// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Celf/Unlit/Transparent/Texture Cutout"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Intensity ("Intensity", float) = 1
		_MainTex ("Main Tex", 2D) = "white" {}
		_AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.1

		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", float) = 2 //Back
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", float) = 4 //LessEqual
		[Enum(Off, 0, On, 1)] _ZWrite("Z Write", float) = 0 //Off
	}

	CGINCLUDE
	ENDCG

	SubShader
	{
		Tags { "Queue" = "AlphaTest" "RenderType"="TransparentCutout" }
		Pass
		{
			Tags { "LIGHTMODE"="Always" }
			Lighting Off
			Fog { Mode Off }
			Cull [_CullMode]
			ZTest [_ZTest]
			ZWrite [_ZWrite]

			CGPROGRAM
			#include "../Include.cginc"

			float _AlphaCutoff;

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata i)
			{
				v2f o;
				o.color = i.color * _Color;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR0
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 color = i.color * _Intensity;
				color *= tex;
				clip(tex.a - _AlphaCutoff);
				return saturate(color);
			}
	
			ENDCG
		} 

		Pass {
			Name "Caster"
			Tags { "LightMode" = "ShadowCaster" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			
			struct v2f { 
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _MainTex_ST;

			v2f vert( appdata_base v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			uniform sampler2D _MainTex;
			float _AlphaCutoff;

			float4 frag( v2f i ) : SV_Target
			{
				float4 texcol = tex2D( _MainTex, i.uv );
				clip( texcol.a - _AlphaCutoff);
		
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}

}