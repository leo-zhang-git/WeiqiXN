Shader "XNShader/Ground"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Ground Texture Array", 2DArray) = "" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_ARRAY(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float4 color : COLOR;
                float4 uv : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }

            half4 SampleLayer(float3 positionWS, half weight, float layerIndex)
            {
                half4 color = SAMPLE_TEXTURE2D_ARRAY(_MainTex, sampler_MainTex, positionWS.xz * 0.02, layerIndex);
                return color * weight;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color =
                    SampleLayer(input.positionWS, input.color.r, input.uv.x) +
                    SampleLayer(input.positionWS, input.color.g, input.uv.y) +
                    SampleLayer(input.positionWS, input.color.b, input.uv.z) +
                    SampleLayer(input.positionWS, input.color.a, input.uv.w);

                return half4(color.rgb * _Color.rgb, 1.0h);
            }
            ENDHLSL
        }
    }
}
