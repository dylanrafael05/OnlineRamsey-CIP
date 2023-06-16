Shader "Unlit/UIShaders/WheelSelect"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1., 1., 0.6, 1.)
        _NodeColor ("Node Color", Color) = (1., 1., 1., 1.)

        _WheelRadius ("Radius", Float) = 1.
        _WheelThickness ("Wheel Thickness", Float) = 0.3

        _TickCount ("Tick Count", Integer) = 2
        _TickDim ("Tick Dimensions", Vector) = (0.05, 0.2, 0., 0.)

        _NodeLocation ("Node Location", Integer) = 0
        _NodeRadius ("Node Radius", Float) = 0.1

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

            float4 _BaseColor;
            float4 _NodeColor;

            float _WheelRadius;
            float _WheelThickness;

            int _TickCount;
            float2 _TickDim;

            float _NodeRadius;
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
                exists += step(abs(length(i.uv) - _WheelRadius), _WheelThickness * .5);

                // Ticks
                float partitionSize = 2. * PI / float(_TickCount); //TODO: test without cast
                float2 polar = float2(length(i.uv), fmod(atan2(i.uv.y, i.uv.x) + 2. * PI, 2.*PI));

                float rtheta = fmod(polar.y, partitionSize) - partitionSize * .5 + PI * .5; //About PI*.5

                float2 newSpace = float2(cos(rtheta), sin(rtheta));
                float2 p = abs(polar.x * newSpace - float2(0., _WheelRadius));
                exists += step(p.x, _TickDim.x * .5) * step(p.y, _TickDim.y * .5);

                // Node
                float id = (polar.y - fmod(polar.y, partitionSize)) / partitionSize;
                float isNode = step(abs(id - float(_NodeLocation)), 0.001);
                exists += isNode * step(length(polar.x * newSpace - float2(0., _WheelRadius)) - _NodeRadius, 0.);

                // Composite
                exists = step(0.01, exists);
                return exists * (isNode * _NodeColor + (1 - isNode) * _BaseColor);

            }
            ENDCG
        }
    }
}
