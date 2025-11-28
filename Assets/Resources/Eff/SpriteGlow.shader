Shader "Custom/SpriteGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Glow Settings)]
        _GlowColor ("Glow Color", Color) = (1, 0.9, 0.3, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 3)) = 0
        _GlowWidth ("Glow Width", Range(0, 0.1)) = 0.02
        
        [Header(Pulse Animation)]
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 3
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.3
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowWidth;
            float _PulseSpeed;
            float _PulseAmount;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            // 采样周围像素获取边缘alpha
            float SampleGlowAlpha(float2 uv, float2 offset)
            {
                float alpha = 0;
                // 8方向采样
                alpha += tex2D(_MainTex, uv + float2(offset.x, 0)).a;
                alpha += tex2D(_MainTex, uv + float2(-offset.x, 0)).a;
                alpha += tex2D(_MainTex, uv + float2(0, offset.y)).a;
                alpha += tex2D(_MainTex, uv + float2(0, -offset.y)).a;
                alpha += tex2D(_MainTex, uv + float2(offset.x, offset.y)).a * 0.707;
                alpha += tex2D(_MainTex, uv + float2(-offset.x, offset.y)).a * 0.707;
                alpha += tex2D(_MainTex, uv + float2(offset.x, -offset.y)).a * 0.707;
                alpha += tex2D(_MainTex, uv + float2(-offset.x, -offset.y)).a * 0.707;
                return alpha / 6.828;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv) * i.color;
                
                // 无发光时直接返回原色
                if (_GlowIntensity <= 0)
                {
                    mainColor.rgb *= mainColor.a;
                    return mainColor;
                }

                // 计算脉冲动画
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float glowStrength = _GlowIntensity * pulse;

                // 计算外发光
                float2 offset = _MainTex_TexelSize.xy * _GlowWidth * 100;
                float glowAlpha = SampleGlowAlpha(i.uv, offset);
                
                // 边缘发光：周围有像素但当前像素透明度低的地方
                float edgeGlow = saturate(glowAlpha - mainColor.a) * glowStrength;
                
                // 整体发光叠加
                float innerGlow = mainColor.a * glowStrength * 0.1;

                // 混合发光色和原色
                fixed3 glowEffect = _GlowColor.rgb * (edgeGlow + innerGlow);
                fixed3 finalColor = mainColor.rgb + glowEffect;
                float finalAlpha = saturate(mainColor.a + edgeGlow * _GlowColor.a);

                // 预乘alpha
                finalColor *= finalAlpha;
                
                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}