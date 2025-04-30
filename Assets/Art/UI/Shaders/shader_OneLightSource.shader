Shader "UI/DiffuseWithDrDl"
{
    Properties
    {
        _MainTex    ("Texture",          2D)   = "white" {}
        _ColorRefl  ("Light Reflection", Color)= (1,1,1,1)
        _LightInt   ("Light Intensity",  Range(0,5)) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; float3 normalOS:NORMAL; };
            struct Varyings   { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float3 normalWS:TEXCOORD1; };
        ENDHLSL

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                CBUFFER_START(UnityPerMaterial)
                    float4 _ColorRefl;
                    float  _LightInt;
                    float4 _MainTex_ST;
                CBUFFER_END

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv         = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.normalWS   = normalize(TransformObjectToWorldNormal(IN.normalOS));
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    Light mainLight = GetMainLight();
                    float3 L        = mainLight.direction;
                    float  NdotL    = max(0, dot(IN.normalWS, L));
                    float3 diffuse  = _ColorRefl.rgb * _LightInt * NdotL;

                    half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                    texColor.rgb  *= diffuse;
                    return texColor;
                }
            ENDHLSL
        }
    }
}
