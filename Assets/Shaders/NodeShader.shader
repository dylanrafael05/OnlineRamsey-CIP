Shader "Unlit/NodeShader"
{
    Properties
    {

        [HideInInspector]
        _Radius ("Radius", Float) = 1

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

            #include "UnityCG.cginc"

            struct vIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<float2> Positions;

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(v.vertex.xy + Positions[instanceID], 0., 1.));
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (vOut i) : SV_Target
            {
                return float4(0.2, 0.2, 0.2, 1.);
            }

            ENDCG
        }
    }
}
