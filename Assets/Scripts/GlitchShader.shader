Shader "Custom/URPGlitchEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Glitch Speed", Float) = 1.0
        _Intensity ("Glitch Intensity", Float) = 0.1
        _RGBSplit ("RGB Split", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "GlitchPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Speed;
            float _Intensity;
            float _RGBSplit;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            float2 jitterUV(float2 uv, float time, float intensity)
            {
                float scanline = floor(uv.y * 100.0);
                float glitch = (rand(float2(scanline, time)) - 0.5) * intensity;
                uv.x += glitch;
                return uv;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float time = _Time.y * _Speed;
                float2 uv = IN.uv;
                float2 jitteredUV = jitterUV(uv, time, _Intensity);

                float r = tex2D(_MainTex, jitteredUV + float2(_RGBSplit * 0.005, 0)).r;
                float g = tex2D(_MainTex, jitteredUV).g;
                float b = tex2D(_MainTex, jitteredUV - float2(_RGBSplit * 0.005, 0)).b;

                return float4(r, g, b, 1.0);
            }
            ENDHLSL
        }
    }
}
