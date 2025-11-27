Shader "Custom/SkyGradientGlow"
{
    Properties
    {
        _MainTex ("Sky Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.4, 0.6, 0.9, 1)
        _BottomColor ("Bottom Color", Color) = (0.8, 0.9, 1, 1)
        _GradientStrength ("Gradient Strength", Range(0, 1)) = 0.5
        _GlowColor ("Glow Color", Color) = (1, 0.9, 0.7, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 2)) = 0.8
        _GlowPosition ("Glow Position Y", Range(0, 1)) = 0.7
        _GlowSize ("Glow Size", Range(0.1, 1)) = 0.3
    }
    
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "Queue"="Background"
            "IgnoreProjector"="True"
        }
        
        Pass
        {
            ZWrite Off
            ZTest Always
            
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
            float4 _TopColor;
            float4 _BottomColor;
            float _GradientStrength;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowPosition;
            float _GlowSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 gradient = lerp(_BottomColor, _TopColor, i.uv.y);
                
                fixed4 baseColor = lerp(texColor, texColor * gradient, _GradientStrength);
                
                float dist = abs(i.uv.y - _GlowPosition);
                float glow = exp(-dist * dist / (_GlowSize * _GlowSize)) * _GlowIntensity;
                
                baseColor.rgb += _GlowColor.rgb * glow;
                
                return baseColor;
            }
            ENDCG
        }
    }
}