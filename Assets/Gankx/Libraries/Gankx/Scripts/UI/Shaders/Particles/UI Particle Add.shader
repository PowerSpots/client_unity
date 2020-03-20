Shader "UI/Particles/Additive" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}

    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {	

		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UI_SOFT_CLIP

			sampler2D _MainTex;
			fixed4 _TintColor;
			float2 _MainTex_TexelSize;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef UI_SOFT_CLIP
				float4 worldPosition : TEXCOORD1;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t IN)
			{
				v2f v;
				#ifdef UI_SOFT_CLIP
				v.worldPosition = IN.vertex;
				#endif
				v.vertex = UnityObjectToClipPos(IN.vertex);

				v.color = IN.color;
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					IN.texcoord.y = 1 - IN.texcoord.y;
#endif
				v.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
				return v;
			}

			float4 _ClipRect; 
			float4 _ClipSoft;
			
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 col = 2.0f * IN.color * _TintColor * tex2D(_MainTex, IN.texcoord);

				#ifdef UI_SOFT_CLIP
					col.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
					if (col.a > 0)
					{
						float2 factor = float2(0.0, 0.0);
						float2 tempXY = (IN.worldPosition.xy - _ClipRect.xy) / _ClipSoft.xy;
						factor = max(factor, tempXY);
						float2 tempZW = (_ClipRect.zw - IN.worldPosition.xy) / _ClipSoft.zw;
						factor = min(factor, tempZW);
						col.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
					}
				#endif

				return col;
			}
			ENDCG 
		}
	}	
}
}
