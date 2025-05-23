Shader "UI/LinearProgressBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Dummy texture property
        
        
        _Color    ("Fill Color",    Color)    = (1,1,1,0)
        _Width    ("Width",         Float)    = 2.0
        _Height   ("Height",        Float)    = 1.0
        _Fill     ("Fill Amount",   Range(0,1)) = 1.0

            // Add stencil-related properties
        _Stencil ("Stencil Reference", Int) = 1
        _StencilComp ("Stencil Comparison", Int) = 8
        _StencilOp ("Stencil Operation", Int) = 0
        _StencilWriteMask ("Stencil Write Mask", Int) = 255
        _StencilReadMask ("Stencil Read Mask", Int) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry+1" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref 1                 
            Comp Always           
            Pass Replace          
            WriteMask 255         
            ReadMask 255
        }

        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct app { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            
            sampler2D _MainTex;
            fixed4 _Color;
            float _Width, _Height, _Fill;

            v2f vert(app v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            float sdCircle(float2 p, float2 c, float r)
            {
                return length(p - c) - r;
            }


            float sdBox(float2 p, float2 half)
            {
                float2 d = abs(p) - half;
                return length(max(d,0)) + min(max(d.x,d.y), 0);
            }

            float4 frag(v2f i) : SV_Target
            {

                float2 size = float2(_Width, _Height);
                float2 p    = (i.uv - 0.5) * size;
                float  r    = _Height * 0.5;


                float2 leftC = float2(-_Width*0.5 + r, 0);

                float2 rightC = float2(
                    -_Width*0.5 + r + (_Width - 2*r) * saturate(_Fill),
                    0
                );


                float dL = sdCircle(p, leftC,  r);
                float dR = sdCircle(p, rightC, r);
                float2 mid   = (leftC + rightC) * 0.5;
                float  halfW = (rightC.x - leftC.x) * 0.5;
                float dS = sdBox(p - mid, float2(halfW, r));

                float dist = min(min(dL, dR), dS);

                float aa = fwidth(dist);
                float alphaMask = smoothstep(0, aa, -dist);

                clip(alphaMask - 0.001);

                return float4(_Color.rgb, _Color.a * alphaMask);
            }
            ENDCG
        }
    }
}
