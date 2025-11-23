Shader "Custom/SlashEffect"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 3
        _EdgeGlow ("Edge Glow", Range(0, 1)) = 0.8
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha One
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _GlowIntensity;
            float _EdgeGlow;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 边缘更亮的渐变
                float edgeDist = abs(i.uv.y - 0.5) * 2.0;
                float edge = pow(1.0 - edgeDist, 2.0);
                
                // 整体渐变（从中心到两端）
                float centerDist = abs(i.uv.x - 0.5) * 2.0;
                float center = 1.0 - centerDist;
                
                float intensity = (edge * _EdgeGlow + center * 0.5) * _GlowIntensity;
                
                fixed4 col;
                col.rgb = _Color.rgb * intensity;
                col.a = _Color.a * intensity * 0.5;
                
                return col;
            }
            ENDCG
        }
    }
}