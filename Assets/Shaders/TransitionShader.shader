// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Screen/Transition"
{
    Properties
    {

        _Progress ("Progress", Range(0, 1)) = 0.

    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        LOD 100
        Cull Off
        ZWrite Off
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

            float _Progress;

            #define SCL 0.005f
            #define PI 3.1415926535f
            #define TAU (PI * 2)

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float2 rotate(float2 v, float a)
            {
                float s = sin(a);
                float c = cos(a);
                float2x2 m = float2x2(c, -s, s, c);
                return mul(m, v);
            }

            fixed4 frag (vOut i) : SV_Target
            {
                float f = pow(_Progress, 1.);

                float2 uv = abs(fmod(i.uv, SCL) / SCL * 2. - 1.);
                float2 uvs = rotate(uv, f * TAU);

                // return float4(0.,0.,0.,0.);

                return float4(0.,0.,0.,1.) * step(max(uvs.x, uvs.y), f);
            }

            ENDCG
        }
    }
}
