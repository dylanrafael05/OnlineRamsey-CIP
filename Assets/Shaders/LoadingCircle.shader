// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UIShaders/LoadingCircle"
{
    Properties
    {

        _OuterRadius ("Outer Radius", Float) = 1 //[0, 1] to avoid cutting
        _InnerRadius ("Inner Radius", Float) = 0.9 //[0, 1] to avoid cutting

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
            
            float2 _Mouse;

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float _InnerRadius;
            float _OuterRadius;

            #define PI 3.1415926535f
            #define TAU (PI * 2)

            bool angle_between(float a, float b, float v) 
            {
                v = fmod(TAU + fmod(v, TAU), TAU);
                a = fmod(TAU + fmod(a, TAU), TAU);
                b = fmod(TAU + fmod(b, TAU), TAU);

                bool an = a <= v;
                bool nb = v <= b;

                return (a < b) 
                    ? (an & nb)
                    : (an | nb);
            }

            fixed4 frag (vOut i) : SV_Target
            {
                float r = length(i.uv);
                float t = atan2(i.uv.y, i.uv.x) + PI;

                float T  = _Time.w;
                float tT = _Time.y;

                float a = fmod(T + (sin(tT) * .5 + .5) * PI * .2, TAU);
                float b = fmod(a + (cos(tT) * .5 + .5) * PI * 1.4 + PI * .3, TAU);

                return float4(1., 1., 1., 1.) 
                    * float(_InnerRadius <= r)
                    * float(r <= _OuterRadius)
                    * float(angle_between(-a, -b, t));
            }

            ENDCG
        }
    }
}
