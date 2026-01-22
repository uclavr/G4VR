// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Rainbow shader with lots of adjustable properties!

Shader "_Shaders/Rainbow" 
{
    Properties
    {
        _Brightness ("Brightness", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed3 GetSmoothRainbowColor(float y)
            {
                // Adjusted proportions for thicker blue/red bands
                if (y < 0.25)
                {
                    // Blue to Cyan
                    float t = smoothstep(0.00, 0.25, y);
                    return lerp(fixed3(0, 0, 1), fixed3(0, 1, 1), t);
                }
                else if (y < 0.45)
                {
                    // Cyan to Green
                    float t = smoothstep(0.25, 0.45, y);
                    return lerp(fixed3(0, 1, 1), fixed3(0, 1, 0), t);
                }
                else if (y < 0.65)
                {
                    // Green to Yellow
                    float t = smoothstep(0.45, 0.65, y);
                    return lerp(fixed3(0, 1, 0), fixed3(1, 1, 0), t);
                }
                else if (y < 0.80)
                {
                    // Yellow to Orange
                    float t = smoothstep(0.65, 0.80, y);
                    return lerp(fixed3(1, 1, 0), fixed3(1, 0.5, 0), t);
                }
                else
                {
                    // Orange to Red (thicker red region)
                    float t = smoothstep(0.80, 1.00, y);
                    return lerp(fixed3(1, 0.5, 0), fixed3(1, 0, 0), t);
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float y = saturate(i.uv.y);
                fixed3 color = GetSmoothRainbowColor(y);
                return fixed4(color * _Brightness, 1.0);
            }
            ENDCG
        }
    }
}



