Shader "UI/DissolveEffect"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _DissolveColor("Dissolve Color", Color) = (0, 0, 0, 1)
        _BrushMask("Brush Mask", 2D) = "black" {}

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _Progress("Progress", Range(0, 1.2)) = 0

        _ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float4 texcoord  : TEXCOORD0;
            };

            fixed4    _Color;
            fixed     _Progress;
            fixed4    _DissolveColor;

            sampler2D _BrushMask;
            float4    _BrushMask_ST;
            sampler2D _MainTex;

            v2f vert(appdata_t IN)
            {

                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord.xy = IN.texcoord;

                float4 screenPos = ComputeScreenPos(OUT.vertex);
                float2 uv = screenPos.xy / screenPos.w;

                OUT.texcoord.zw = TRANSFORM_TEX(uv, _BrushMask);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color    = tex2D(_MainTex, IN.texcoord.xy) * IN.color;
                fixed  brush    = tex2D(_BrushMask, IN.texcoord.zw).r;
                fixed  dissolve = saturate( ((_Progress * 1.2 - 0.6) + brush) * 4 - 4 * 0.5 );
                
                return fixed4(lerp(_DissolveColor.rgb, color.rgb, dissolve), dissolve * color.a);
            }
            ENDCG
        }
    }
}
