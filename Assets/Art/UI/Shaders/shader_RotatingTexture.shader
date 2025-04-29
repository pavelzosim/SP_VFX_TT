Shader "UI/RotatingTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RotationSpeed ("Rotation Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _RotationSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Calculate dynamic rotation angle using time
                float rotationAngle = _Time.y * _RotationSpeed;
                float cosTheta = cos(rotationAngle);
                float sinTheta = sin(rotationAngle);
                float2x2 rotationMatrix = float2x2(cosTheta, -sinTheta, sinTheta, cosTheta);

                // Apply UV rotation
                OUT.uv = mul(IN.uv - 0.5, rotationMatrix) + 0.5;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Clamp UV coordinates to prevent unfolding
                float2 clampedUV = saturate(IN.uv); // Ensures UV stays between 0 and 1
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, clampedUV);
                return color;
            }
            ENDHLSL
        }
    }
}