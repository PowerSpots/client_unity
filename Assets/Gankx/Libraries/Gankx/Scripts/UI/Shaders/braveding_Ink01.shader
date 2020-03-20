// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:34124,y:32718,varname:node_1873,prsc:2|emission-1013-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:31948,y:32721,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:32209,y:32810,cmnt:RGB,varname:node_1086,prsc:2|A-4805-RGB,B-5983-RGB,C-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:31948,y:32907,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:31948,y:33071,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:603,x:32209,y:32984,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5983-A,C-5376-A;n:type:ShaderForge.SFN_SceneColor,id:8018,x:32353,y:33294,varname:node_8018,prsc:2;n:type:ShaderForge.SFN_Lerp,id:1013,x:33211,y:32790,varname:node_1013,prsc:2|A-52-OUT,B-8018-RGB,T-9286-OUT;n:type:ShaderForge.SFN_Tex2d,id:5353,x:32209,y:33128,ptovrint:False,ptlb:MaskMap,ptin:_MaskMap,varname:node_5353,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8376-OUT;n:type:ShaderForge.SFN_OneMinus,id:9881,x:32701,y:33088,varname:node_9881,prsc:2|IN-3914-OUT;n:type:ShaderForge.SFN_Blend,id:52,x:32894,y:32789,varname:node_52,prsc:2,blmd:3,clmp:True|SRC-3433-OUT,DST-1086-OUT;n:type:ShaderForge.SFN_RemapRange,id:4760,x:32565,y:33225,varname:node_4760,prsc:2,frmn:0.99,frmx:1,tomn:0,tomx:1|IN-3914-OUT;n:type:ShaderForge.SFN_Clamp01,id:3474,x:32891,y:33088,varname:node_3474,prsc:2|IN-9881-OUT;n:type:ShaderForge.SFN_Color,id:8925,x:32915,y:33370,ptovrint:False,ptlb:MaskColor,ptin:_MaskColor,varname:node_8925,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:3433,x:33165,y:33093,varname:node_3433,prsc:2|A-8925-RGB,B-7209-OUT,T-3474-OUT;n:type:ShaderForge.SFN_Vector1,id:7209,x:32891,y:33026,varname:node_7209,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:6374,x:31309,y:33359,ptovrint:False,ptlb:NoiseMap,ptin:_NoiseMap,varname:node_6374,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b87bd26a9e2c9d74699eb3d85259c723,ntxv:0,isnm:False|UVIN-3668-OUT;n:type:ShaderForge.SFN_TexCoord,id:9273,x:30852,y:33225,varname:node_9273,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:4030,x:31637,y:33364,varname:node_4030,prsc:2|A-6374-R,B-7492-OUT;n:type:ShaderForge.SFN_Vector1,id:7492,x:31449,y:33398,varname:node_7492,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Add,id:8376,x:31934,y:33292,varname:node_8376,prsc:2|A-9273-UVOUT,B-4030-OUT,C-6438-OUT;n:type:ShaderForge.SFN_Clamp01,id:1782,x:32770,y:33225,varname:node_1782,prsc:2|IN-4760-OUT;n:type:ShaderForge.SFN_Desaturate,id:9286,x:32943,y:33225,varname:node_9286,prsc:2|COL-1782-OUT;n:type:ShaderForge.SFN_Append,id:8050,x:30686,y:33487,varname:node_8050,prsc:2|A-7605-OUT,B-6791-OUT;n:type:ShaderForge.SFN_Multiply,id:9432,x:30867,y:33487,varname:node_9432,prsc:2|A-8050-OUT,B-8190-T;n:type:ShaderForge.SFN_ValueProperty,id:7605,x:30345,y:33488,ptovrint:False,ptlb:U speed,ptin:_Uspeed,varname:node_7605,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6791,x:30345,y:33587,ptovrint:False,ptlb:V speed,ptin:_Vspeed,varname:node_6791,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:8190,x:30686,y:33641,varname:node_8190,prsc:2;n:type:ShaderForge.SFN_Add,id:3668,x:31074,y:33376,varname:node_3668,prsc:2|A-9273-UVOUT,B-9432-OUT;n:type:ShaderForge.SFN_OneMinus,id:8700,x:32516,y:33068,varname:node_8700,prsc:2|IN-5353-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:3914,x:32380,y:33089,ptovrint:False,ptlb:inverter,ptin:_inverter,varname:node_3914,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8700-OUT,B-5353-RGB;n:type:ShaderForge.SFN_Vector4Property,id:1762,x:31637,y:33528,ptovrint:False,ptlb:TexOffset,ptin:_TexOffset,varname:node_1762,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Append,id:6438,x:31839,y:33548,varname:node_6438,prsc:2|A-1762-X,B-1762-Y;proporder:4805-5983-8925-5353-6374-7605-6791-3914-1762;pass:END;sub:END;*/

Shader "UI/UI_Ink01" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MaskColor ("MaskColor", Color) = (0.5,0.5,0.5,1)
        _MaskMap ("MaskMap", 2D) = "white" {}
        _NoiseMap ("NoiseMap", 2D) = "white" {}
        _Uspeed ("U speed", Float ) = 0
        _Vspeed ("V speed", Float ) = 0
        [MaterialToggle] _inverter ("inverter", Float ) = 1
        _TexOffset ("TexOffset", Vector) = (0,0,0,0)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            //#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform sampler2D _MaskMap; uniform float4 _MaskMap_ST;
            uniform float4 _MaskColor;
            uniform sampler2D _NoiseMap; uniform float4 _NoiseMap_ST;
            uniform float _Uspeed;
            uniform float _Vspeed;
            uniform fixed _inverter;
            uniform float4 _TexOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_7209 = 1.0;
                float4 node_8190 = _Time + _TimeEditor;
                float2 node_3668 = (i.uv0+(float2(_Uspeed,_Vspeed)*node_8190.g));
                float4 _NoiseMap_var = tex2D(_NoiseMap,TRANSFORM_TEX(node_3668, _NoiseMap));
                float2 node_8376 = (i.uv0+(_NoiseMap_var.r*0.1)+float2(_TexOffset.r,_TexOffset.g));
                float4 _MaskMap_var = tex2D(_MaskMap,TRANSFORM_TEX(node_8376, _MaskMap));
                float3 _inverter_var = lerp( (1.0 - _MaskMap_var.rgb), _MaskMap_var.rgb, _inverter );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = lerp(saturate((lerp(_MaskColor.rgb,float3(node_7209,node_7209,node_7209),saturate((1.0 - _inverter_var)))+(_MainTex_var.rgb*_Color.rgb*i.vertexColor.rgb)-1.0)),sceneColor.rgb,dot(saturate((_inverter_var*100.0001+-99.0001)),float3(0.3,0.59,0.11)));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*_Color.a*i.vertexColor.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
