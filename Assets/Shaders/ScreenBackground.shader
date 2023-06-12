// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Screen/Background"
{
    Properties
    {

        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)

    }
    SubShader
    {
        Tags { "Queue"="Background" }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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

            #define SCL 0.01f
            #define SCROLL 0.7f

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float4 _BaseColor;
            float4 _HighlightColor;

            fixed4 frag (vOut i) : SV_Target
            {
                float2 dir = float2(sin(_Time.x * 3.f), cos(_Time.x * 3.3f));

                float2 p = (fmod(i.uv + SCL * SCROLL * dir, SCL) / SCL) * 2.f - 1.f;
                float2 id = (i.uv - (p * .5 + .5) * SCL) * float2(1., -1.) + .05*_Time.y;

                float s = step(length(p), 1.f - ((_SinTime.y*_SinTime.y)-0.*(sin(length(id)*15.)*.5+.5)) * 0.2f);

                return _BaseColor * (1 - s) + _HighlightColor * s;
            }

            ENDCG
        }
    }
}
