Shader "XNShader/Road"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        [Toggle(_ROAD_BLEND_ALPHA)] _UseBlendAlpha ("Use Blend Alpha", Float) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        LOD 200
        OFFSET -1, -1

        CGPROGRAM
        // 加入unity built-in shader的贴花指令，在opaque渲染队列之后之后执行透明度混合
        #pragma surface surf Standard fullforwardshadows decal:blend
        #pragma shader_feature_local _ROAD_BLEND_ALPHA

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 noise = tex2D(_NoiseTex, IN.worldPos.xz * 0.02);
            fixed4 c = _Color * (noise.y * 0.75 + 0.25);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
#if defined(_ROAD_BLEND_ALPHA)
            float blend = IN.uv_MainTex.x;
            blend *= noise.x + 0.5;
            blend = smoothstep(0.4, 0.7, blend);
            o.Alpha = blend;
#else
            o.Alpha = 1;
#endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
