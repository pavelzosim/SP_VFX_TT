Shader "UI/OneLightSource"
{
    Properties
    {
        _MainTex ("Image Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
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

            sampler2D _MainTex;
            float4 _LightColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                // Multiply by light color for optional light effect
                float4 finalColor = texColor * _LightColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}