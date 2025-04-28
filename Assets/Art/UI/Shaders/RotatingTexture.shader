Shader "Custom/URP_RotatingTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RotationSpeed ("Rotation Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            Name "ForwardLit"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RotationSpeed;
            float _Time;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float2 RotateUV(float2 uv, float angle)
            {
                uv -= 0.5; // Center
                float s = sin(angle);
                float c = cos(angle);
                float2 rotatedUV;
                rotatedUV.x = uv.x * c - uv.y * s;
                rotatedUV.y = uv.x * s + uv.y * c;
                rotatedUV += 0.5; // Back
                return rotatedUV;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float rotation = _Time * _RotationSpeed;
                float2 rotatedUV = RotateUV(IN.uv, rotation);
                half4 col = tex2D(_MainTex, rotatedUV);
                return col;
            }
            ENDHLSL
        }
    }
}
