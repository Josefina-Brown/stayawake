// PosterizePostProcess.shader

Shader "PosterizePostProcess"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PosterizeLevels("Posterize Levels", Float) = 4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "Posterize"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _PosterizeLevels;

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

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float3 Posterize(float3 color, float levels)
            {
                return floor(color * levels) / levels;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                col = Posterize(col, _PosterizeLevels);
                return half4(col, 1);
            }

            ENDHLSL
        }
    }
    FallBack Off
}
