Shader "Unlit/GraphShaders/NodeShader"
{
    Properties
    {

        [HideInInspector]
        _Radius ("Radius", Float) = 1 //[0, 1] to avoid cutting
        [HideInInspector]
        _HighlightRadius ("Highlight Radius", Float) = 0.5

        _HighlightColor ("Highlight Color", Color) = (1., 1., 1., 1.)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

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
                float highlighted : TEXCOORD1;
            };

            StructuredBuffer<float2> Positions;
            StructuredBuffer<float> IsHighlighted;

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(v.vertex.xy + Positions[instanceID], 0., 1.));
                o.uv = v.uv;
                o.highlighted = IsHighlighted[instanceID];

                return o;
            }

            float _Radius;
            float _HighlightRadius;
            float4 _HighlightColor;

            fixed4 frag (vOut i) : SV_Target
            {
                float len = length(i.uv);

                float rtheta = atan2(i.uv.y, i.uv.x);
                float theta = rtheta + _Time.y;

                float hiRad = _HighlightRadius * i.highlighted * (sin(theta * 10.0) * 0.05 + 0.95);

                return step(len, _Radius) * lerp(_HighlightColor, float4(0., 0., 0., 1), smoothstep(_Radius, hiRad, len));
            }

            ENDCG
        }
    }
}
