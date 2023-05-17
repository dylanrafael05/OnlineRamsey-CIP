Shader "Unlit/GraphShaders/EdgeShader"
{
    Properties
    {

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

            struct vIn
            {
                float4 vertex : POSITION;
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD0;
            };

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, mul(Transforms[instanceID], v.vertex));
                o.color = Colors[instanceID];

                return o;
            }

            fixed4 frag (vOut i, uint instanceID : SV_InstanceID) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
