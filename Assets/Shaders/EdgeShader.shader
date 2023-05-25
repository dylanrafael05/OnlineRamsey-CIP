Shader "Unlit/GraphShaders/EdgeShader"
{
    Properties
    {
        [HideInInspector] _HighlightColor("Highlight Color", Color) = (1., 1., 1., 1.)
        [HideInInspector] _HighlightAmount("Highlight Amount", Float) = 0.5
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
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            StructuredBuffer<float4x4> Transforms;
            StructuredBuffer<float4> Colors;
            StructuredBuffer<float> IsHighlighted;

            float4 _HighlightColor;
            float _HighlightAmount; //0 to 1

            struct vIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float isHighlighted : TEXCOORD2;
            };

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, mul(Transforms[instanceID], v.vertex));
                o.color = Colors[instanceID];
                o.uv = v.uv;
                o.isHighlighted = IsHighlighted[instanceID];

                return o;
            }

            fixed4 frag(vOut i, uint instanceID : SV_InstanceID) : SV_Target
            {
                //Highlight
                float2 p = i.uv; //make sure -1 to 1
                p.y = abs(p.y);
                float highlight = step(_HighlightAmount, p.y) * i.isHighlighted;

                return lerp(i.color, _HighlightColor, highlight); //did this not work last time? if this message is here i havent tested it
            }
            ENDCG
        }
    }
}
