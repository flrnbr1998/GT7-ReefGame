Shader "UWeffect/UnderwaterEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseFrequency ("Noise Frequency", Float) = 1.0
        _NoiseSpeed ("Noise Speed", Float) = 1.0
        _NoiseScale ("Noise Scale", Float) = 1.0
        _PixelOffset ("Pixel Offset", Float) = 0.01
        _NoiseOffset ("Noise Offset", Float) = 0.0
        _MaxBlur ("Max Blur", Float) = 0.05
        _DepthScale ("Depth Scale", Float) = 0.1
        _BlurStartDepth ("Blur Start Depth", Float) = 0.1
        _BlurEndDepth ("Blur End Depth", Float) = 1.0
        _WaterLevel ("Water Level", Float) = 0.0
        _FadeDistance ("Fade Distance", Float) = 10.0
        _UnderwaterColor ("Underwater Color", Color) = (0, 0.5, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NoiseFrequency;
            float _NoiseSpeed;
            float _NoiseScale;
            float _PixelOffset;
            float _NoiseOffset;
            float _MaxBlur;
            float _DepthScale;
            float _BlurStartDepth;
            float _BlurEndDepth;
            float _WaterLevel;
            float _FadeDistance;
            float4 _UnderwaterColor;

            sampler2D_float _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample the depth at the current pixel
                float depth = tex2D(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w).r;

                // Calculate the blur amount based on depth
                float blurAmount = 0.0;
                if (depth > _BlurStartDepth)
                {
                    blurAmount = smoothstep(_BlurStartDepth, _BlurEndDepth, depth) * _MaxBlur;
                }

                // Apply noise offset
                float2 noiseUV = i.uv;
                noiseUV.y += sin(_NoiseOffset + noiseUV.x * _NoiseFrequency) * _PixelOffset;

                            // Sample the texture with added blur based on depth
                fixed4 col = tex2D(_MainTex, noiseUV);
                col += tex2D(_MainTex, noiseUV + float2(blurAmount, 0));
                col += tex2D(_MainTex, noiseUV - float2(blurAmount, 0));
                col += tex2D(_MainTex, noiseUV + float2(0, blurAmount));
                col += tex2D(_MainTex, noiseUV - float2(0, blurAmount));
                col /= 5.0; // Average the samples to create the blur effect

                // Apply underwater color effect
                float waterFactor = smoothstep(_WaterLevel - _FadeDistance, _WaterLevel, depth);
                col = lerp(col, _UnderwaterColor, waterFactor);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}