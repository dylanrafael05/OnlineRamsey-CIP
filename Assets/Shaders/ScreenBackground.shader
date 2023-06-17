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
            #include "Common.hlsl"

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
                o.uv = mul(UNITY_MATRIX_M, v.vertex).xy * 0.008;

                return o;
            }

            float4 _BaseColor;
            float4 _HighlightColor;

            float mixsdf(float r1, float r2, float t) 
            {
                return step(lerp(r1, r2, t), 0);
            }

            float circle(float2 uv, float r)
            {
                return length(uv) - r;
            }

            float square(float2 uv, float r)
            {
                return abs(uv.x) + abs(uv.y) - r;
            }

            float2 rotate(float2 uv, float t)
            {
                float s = sin(t);
                float c = cos(t);

                float2x2 m = float2x2(c, s, -s, c);

                return mul(m, uv);
            }

            fixed4 frag (vOut i) : SV_Target
            {
                float2 dir = float2(sin(_Time.x * 3.f), cos(_Time.x * 3.3f));

                float2 uv = (amod(i.uv + SCL * SCROLL * dir, SCL) / SCL) * 2.f - 1.f;
                uv = rotate(uv, _Time.x);

                float2 id = i.uv - SCL*(uv*.5+.5);
                float wave = sin(_Time.y*2.+dot(50.*float2(1.,1.), id))*.5+.5;
                float r = 1.f - (_SinTime.y*_SinTime.y) * 0.2f*0. - .3* wave;

                float s = mixsdf(
                    circle(uv, r),
                    square(uv, r),
                    _SinTime.x * _SinTime.x
                );

                return _BaseColor * (1 - s) + _HighlightColor * s;
            }

            ENDCG
        }
    }
}
