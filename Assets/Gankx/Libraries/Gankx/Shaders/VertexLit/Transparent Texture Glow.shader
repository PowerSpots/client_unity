// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Celf/VertexLit/Transparent/Texture Glow" {
Properties {
	_Color ("Main Color", Color) = (.5, .5, .5, 1)
	_MainTex ("Main Tex (RGBA)", 2D) = "white" {}
    _Glow ("Glow (RGBA)", 2D) = "black" {}
}

CGINCLUDE
		#include "UnityCG.cginc"
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 2.0

		fixed4 _Color;
		sampler2D _MainTex ;
		sampler2D _Glow;

		struct v2f {
			float4  pos : SV_POSITION;
			float2  uv[3] : TEXCOORD0;
		};

		float4 _MainTex_ST;
		float4 _Glow_ST;

		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv[0] = TRANSFORM_TEX (v.texcoord, _MainTex);
            o.uv[1] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			o.uv[2] = TRANSFORM_TEX (v.texcoord, _Glow);
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 lay1 = tex2D( _MainTex, i.uv[0].xy );
			fixed4 glow = tex2D(_Glow,   i.uv[2]);
			fixed4 c = fixed4(_Color.rgb * 2, 1);
    		c *= lay1;
           	c.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[1]));
			c.rgb += glow.rgb * glow.a;
			return c;
		}
ENDCG

SubShader {
	Tags { "Queue" = "Transparent"  "RenderType"="Transparent" }
	Lighting Off
	Cull Back
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off

    Pass {
		Tags { "LIGHTMODE"="VertexLMRGBM" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
    }

	Pass {
		Tags { "LIGHTMODE"="VertexLM" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
    }

	Pass {
		Tags { "LightMode" = "Vertex" }
		Material { Diffuse (1,1,1,1) }
		Lighting On
		SetTexture [_MainTex] {	Combine texture * primary DOUBLE, texture * primary	} 
	}
}

//Fallback "VertexLit"
} 