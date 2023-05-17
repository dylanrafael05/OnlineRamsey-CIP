Shader "Unlit/GraphShaders/EdgeShader"
{
    Properties
    {
        [HideInInspector] _RedColor ("Red Color", Color) = (1., 0., 0., 1.)
        [HideInInspector] _BlueColor ("Blue Color", Color) = (0., 0., 1., 1.)
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
            StructuredBuffer<float> Types;

            struct vIn
            {
                float4 vertex : POSITION;
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
                float type : TEXCOORD0;
            };

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, mul(Transforms[instanceID], v.vertex));
                o.type = Types[instanceID];

                return o;
            }

            float4 _RedColor;
            float4 _BlueColor;

            fixed4 frag (vOut i, uint instanceID : SV_InstanceID) : SV_Target
            {
                return lerp(_BlueColor, _RedColor, i.type);
            }
            ENDCG
        }
    }
}
