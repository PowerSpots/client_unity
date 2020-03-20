Shader "UI/LiquidFill"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OverlayTex ("Overlay Texture", 2D) = "white" {}
        _MaskTex ("Fill Mask", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range (0, 1.1)) = 1
        _Fluid ("Fluid", Vector) = (0.05, 0, 0, 0)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        _StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

        Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                half4 maskcoord : TEXCOORD1;
            };
            
            float4 _MaskTex_ST;
            float4 _MaskTex_TexelSize;
            fixed4 _Fluid;
            fixed  _FillAmount;
            float  _AlphaSplitEnabled;

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            sampler2D _MaskTex;
            sampler2D _OverlayTex;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;

                float2 uv = TRANSFORM_TEX(IN.texcoord, _MaskTex);
                OUT.maskcoord.xy = uv + float2(0, 1-_FillAmount + sin(_Time.z * _Fluid.x * 2) * 0.025) + (_Time.z * _Fluid.xy);
                OUT.color = IN.color;

            #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
            #endif

                return OUT;
            }
            
            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
                if (_AlphaSplitEnabled)
                    color.a = tex2D (_AlphaTex, uv).r;

                return color;
            }

            fixed3 Screen (fixed3 cBase, fixed3 cBlend)
            {
                return (1 - (1 - cBase) * (1 - cBlend));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                IN.maskcoord.y = clamp(0, 1-_MaskTex_TexelSize.y, max(_MaskTex_TexelSize.y, IN.maskcoord.y));

                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

                fixed4 mask = tex2D(_MaskTex, IN.maskcoord.xy);
                fixed4 overlay = tex2D(_OverlayTex, IN.maskcoord.xy);

                // c.rgb = lerp(c.rgb * mask.a, overlay.rgb, overlay.a);
                c.rgb = c.rgb * mask.a + overlay.rgb;
                return saturate(fixed4(c.rgb, mask.a + overlay.a) * c.a);
            }
            ENDCG
        }
    }
}