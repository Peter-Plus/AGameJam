Shader "Custom/SkyDusk"
{
    Properties
    {
        _MainTex ("Sky Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.2, 0.1, 0.3, 1)
        _MidColor ("Mid Color", Color) = (0.9, 0.4, 0.2, 1)
        _BottomColor ("Bottom Color", Color) = (1, 0.7, 0.3, 1)
        _MidPosition ("Mid Position", Range(0, 1)) = 0.5
        _GradientStrength ("Gradient Strength", Range(0, 1)) = 0.7
        _GlowColor ("Sun Glow Color", Color) = (1, 0.5, 0.2, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 3)) = 1.5
        _GlowPositionX ("Glow Position X", Range(0, 1)) = 0.5
        _GlowPositionY ("Glow Position Y", Range(0, 1)) = 0.65
        _GlowSize ("Glow Size", Range(0.05, 0.5)) = 0.15
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
            float4 _MidColor;
            float4 _BottomColor;
            float _MidPosition;
            float _GradientStrength;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowPositionX;
            float _GlowPositionY;
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
                
                // 三段渐变
                fixed4 gradient;
                if (i.uv.y > _MidPosition)
                {
                    float t = (i.uv.y - _MidPosition) / (1.0 - _MidPosition);
                    gradient = lerp(_MidColor, _TopColor, t);
                }
                else
                {
                    float t = i.uv.y / _MidPosition;
                    gradient = lerp(_BottomColor, _MidColor, t);
                }
                
                fixed4 baseColor = lerp(texColor, texColor * gradient, _GradientStrength);
                
                // 圆形落日光晕
                float2 sunPos = float2(_GlowPositionX, _GlowPositionY);
                float dist = distance(i.uv, sunPos);
                float glow = exp(-dist * dist / (_GlowSize * _GlowSize)) * _GlowIntensity;
                baseColor.rgb += _GlowColor.rgb * glow;
                
                return baseColor;
            }
            ENDCG
        }
    }
}