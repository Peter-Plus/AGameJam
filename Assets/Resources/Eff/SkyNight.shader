Shader "Custom/SkyNight"
{
    Properties
    {
        _MainTex ("Sky Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.05, 0.1, 0.2, 1)
        _BottomColor ("Bottom Color", Color) = (0.1, 0.15, 0.3, 1)
        _GradientStrength ("Gradient Strength", Range(0, 1)) = 0.6
        _StarDensity ("Star Density", Range(0, 100)) = 50
        _StarSpeed ("Star Twinkle Speed", Range(0, 5)) = 1.0
        _StarBrightness ("Star Brightness", Range(0, 2)) = 1.0
        _AspectRatio ("Aspect Ratio (W/H)", Float) = 5.333
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
            float _StarDensity;
            float _StarSpeed;
            float _StarBrightness;
            float _AspectRatio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float stars(float2 uv, float density)
            {
                float2 grid = floor(uv * density);
                float2 id = frac(uv * density);
                
                float h = hash(grid);
                
                if (h > 0.95)
                {
                    float2 starPos = float2(hash(grid + 0.1), hash(grid + 0.2));
                    float dist = distance(id, starPos);
                    
                    // 每个星星独立闪烁（使用grid位置作为相位偏移）
                    float phase = hash(grid + 0.5) * 6.28;
                    float twinkle = sin(_Time.y * _StarSpeed + phase) * 0.5 + 0.5;
                    
                    if (dist < 0.1)
                    {
                        return (1.0 - dist * 10.0) * twinkle;
                    }
                }
                return 0.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 gradient = lerp(_BottomColor, _TopColor, i.uv.y);
                fixed4 baseColor = lerp(texColor, texColor * gradient, _GradientStrength);
                
                // 校正UV
                float2 correctedUV = i.uv;
                correctedUV.x *= _AspectRatio;
                
                // 星星
                float star = stars(correctedUV, _StarDensity);
                baseColor.rgb += star * _StarBrightness;
                
                return baseColor;
            }
            ENDCG
        }
    }
}