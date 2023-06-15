Shader "Unlit/UIShaders/WheelShader"
{
    Properties
    {
        _Color ("Color", Color) = (1., 1., 1., 1.)

        _Radius ("Radius", Float) = 1.
        _WheelThickness ("Wheel Thickness", Float) = 0.3

        _TickCount ("Tick Count", Int) = 2
        _TickDim ("Tick Dimensions", Vector) = (0.05, 0.2, 0., 0.)
        _NodeLocation ("Node Location", Int) = 0

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
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

            #define PI 3.1415926

            float4 _Color;

            float _Radius;
            float _WheelThickness;

            int _TickCount;
            float2 _TickDim;
            int _NodeLocation;

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(vOut i) : SV_Target
            {

                float exists = 0.;

                // Wheel
                exists += step(abs(length(i.uv) - _Radius), _WheelThickness * .5);

                // Ticks
                float partitionSize = 2. * PI / float(_TickCount); //TODO: test without conversion
                float2 polar = float2(length(i.uv), fmod(atan2(i.uv.y, i.uv.x) + 2. * PI, 2. * PI));

                float rtheta = fmod(polar.y, partitionSize) - partitionSize * .5 + PI * .5;

                return float4(0., 0., 0., 0.);

            }
            ENDCG
        }
    }
}
