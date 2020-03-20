// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Celf/Toon/Cube Rim"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ToonShade ("ToonShader Cubemap(RGB)", Cube) = "_Skybox" {}
		RimColor("Rim Color", Color) = (0.54,0.54,0.54,0.0)
		RimParameters("Rim Params(X=Power, Y=Scale, Z=Color Factor, W=Blend", vector) = (2, 0.8, 1.8, 0.7)
		_Brightness("Brightness", float) = 1
	}

	SubShader
	{
		Tags {"Queue" = "Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" "DepthMode"="true"}
		LOD 200

		Pass
		{
		Tags { "LIGHTMODE"="Always" }
		Blend Off
		Lighting Off
		Cull Back

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma multi_compile NORIM PIXRIM VERTRIM
			#include "UnityCG.cginc"
			
			//#define NORIM
			//#define PIXRIM
			#define VERTRIM

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 RimColor;
			half4 RimParameters;
			half _Brightness;
			uniform samplerCUBE _ToonShade;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				half3 uv : TEXCOORD0;
				half3 normal: TEXCOORD1;
				#ifdef PIXRIM
				half3 viewDir : TEXCOORD2;
				#else
				#endif
			};
			
			#if defined(NORIM)
			#elif defined(PIXRIM)
			fixed3 CalcRimColor(half3 base, half3 viewDir, half3 normal)
			{
				half rim = 1.0h - saturate(dot(viewDir, normal));
				rim = min(pow(rim, (half)RimParameters.x) * (half)RimParameters.y, (half)1);
				rim = max(rim - (half)0.25, (half)0) * 1.33h;

				fixed3 color = lerp(RimColor.rgb, base, RimParameters.w) * RimParameters.z;
				color = lerp(base, color, rim);

				return color;
				//return fixed3(1,0,0);
			}
			#elif defined(VERTRIM)
			fixed3 CalcRimColor(half3 base, half rim)
			{
				rim = min(pow(rim, (half)RimParameters.x) * (half)RimParameters.y, (half)1);
				rim = max(rim - (half)0.25, (half)0) * 1.33h;

				fixed3 color = lerp(RimColor.rgb, base, RimParameters.w) * RimParameters.z;
				color = lerp(base, color, rim);

				return color;
				//return fixed3(0,1,0);
			}
			#endif

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.normal = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz);

				#if defined(NORIM)
				#elif defined(PIXRIM)
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				#elif defined(VERTRIM)
				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				half3 normal = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz);
				o.uv.z = (dot(viewDir, normal));
				#endif

				return o;
			}

			fixed4 frag(v2f i) : COLOR0
			{
				fixed4 color = _Color;
				fixed3 toonlight = texCUBE(_ToonShade, mul(UNITY_MATRIX_V, float4(i.normal, 0)).xyz).xyz;

				color.rgb *= tex2D(_MainTex, i.uv.xy).rgb * toonlight;

				#if defined(NORIM)
				#elif defined(PIXRIM)
				color.rgb = CalcRimColor(color.rgb, i.viewDir, i.normal);
				#elif defined(VERTRIM)
				color.rgb = CalcRimColor(color.rgb, 1.0h - saturate(i.uv.z));
				#endif

				color.rgb *= _Brightness;
				return color;
			}
		ENDCG
		}
	}

	//Fallback "VertexLit"
}
