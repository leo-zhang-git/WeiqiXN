Shader "XNShader/Road"
{
    Properties
    {
        _Color ("Road Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        [Toggle(_ROAD_USE_NOISE)] _UseNoise ("Use Noise", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="TransparentCutout"
            "Queue"="Geometry+1"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Back
            Offset -1, -1

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _ROAD_USE_NOISE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half alpha = 1.0h;

            #if defined(_ROAD_USE_NOISE)
                half noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.positionWS.xz * 0.02).r;
                half blend = input.uv.x;
                blend *= noise + 0.5h;
                alpha = smoothstep(0.4h, 0.7h, blend);
            #endif

                clip(_Color.a * alpha - 0.5h);
                return half4(_Color.rgb, 1.0h);
            }
            ENDHLSL
        }
    }
}
