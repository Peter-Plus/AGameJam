Shader "Custom/WindowGlowSoft"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1, 0.9, 0.6, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 3)) = 1.5
        _CoreSize ("Core Size", Range(0.5, 1)) = 0.9
        _FlickerSpeed ("Flicker Speed", Range(0, 5)) = 0.5
        _FlickerAmount ("Flicker Amount", Range(0, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
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

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _CoreSize;
            float _FlickerSpeed;
            float _FlickerAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 到中心距离
                float2 center = float2(0.5, 0.5);
                float2 dist = abs(i.uv - center);
                float maxDist = max(dist.x, dist.y);
                
                // 0.9内全亮，0.9到边界渐变为0
                float coreEdge = _CoreSize * 0.5;
                float fade = 1.0 - smoothstep(coreEdge, 0.5, maxDist);
                
                // 闪烁
                float flicker = sin(_Time.y * _FlickerSpeed) * _FlickerAmount + 1.0;
                
                // 应用发光和渐变
                col.rgb *= _GlowColor.rgb * _GlowIntensity * flicker;
                col.a *= fade;
                
                return col;
            }
            ENDCG
        }
    }
}