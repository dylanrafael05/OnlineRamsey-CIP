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

            #include "UnityCG.cginc"

            StructuredBuffer<float4x4> Transforms;
            StructuredBuffer<int> Types;

            struct vIn
            {
                float4 vertex : POSITION;
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
            };

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(0., 0., 0., 1.));//mul(Transforms[instanceID], v.vertex));

                return o;
            }

            float4 _RedColor;
            float4 _BlueColor;

            fixed4 frag (vOut i, uint instanceID : SV_InstanceID) : SV_Target
            {
                return lerp(_BlueColor, _RedColor, float(Types[instanceID]));
            }
            ENDCG
        }
    }
}
