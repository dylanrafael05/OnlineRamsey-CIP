Shader "Unlit/DataGraph"
{
    Properties //massive quad we'll cut UV ~ [0, A]
    {

        _Color ("Color", Color) = (0.,0.,0.,1.)

        _TickCount ("Tick Count", Vector) = (4., 4., 0., 0.) //[xTick, yTick]
        _TickDim("Tick Dimensions", Vector) = (0.05, .2, 0., 0.)
        _Scale ("Scale", Vector) = (1., 1., 0., 0.) //[xScale, yScale]
        _Thickness ("Thickness", Float) = 1.
        _TriangleDim ("Triangle Dimensions", Vector) = (1., 1., 0., 0.) //[width, height]

        _UVScale ("UV Scale", Float) = 5.

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" } //overlay queue?
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

            vOut vert (vIn v)
            {
                vOut o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //can't include common.hlsl for some reason
            float2 perp(float2 v)
            {
                return cross(float3(v.x, v.y, 0.), float3(0., 0., -1.)).xy;
            }

            //im calling this sb instead of sd like sample binary cuz we're not raymarching
            float sbTickLine(float2 p, float2 fo, float tickCount, float scale, float barThick, float2 tickDim)
            {

                p = float2(dot(p, fo), dot(p, perp(fo)));

                float exists = 1.0; //Exists = InZone && (InBar || InTick)
                exists *= step(0., p.x) * step(p.x, scale);

                float partitionSize = scale / tickCount;
                p.x = fmod(p.x, partitionSize) - partitionSize * .5;
                p = abs(p);

                exists *= step(p.x, tickDim.x * .5) * step(p.y, tickDim.y * .5) + step(p.y, barThick * .5);

                return min(1., exists);

            }

            float sbETri(float2 p, float2 fo, float2 dim) //equilateral - dim = [spikeLength, width]
            {

                p = float2(dot(p, fo), dot(p, perp(fo)));
                p.y = abs(p.y);

                float m = -dim.y * .5 / dim.x;
                float y = dim.y * .5 - m * p.x;

                return step(0., p.x) * step(p.y, y);

            }

            float4 _Color;
            
            float2 _TickCount;
            float2 _TickDim;

            float2 _Scale;
            float _Thickness;
            float2 _TriangleDim;

            float _UVScale;
            

            fixed4 frag(vOut i) : SV_Target 
            {

                float exists = 0.0;

                float2 p = i.uv * .5 * _UVScale + _UVScale * .5; //UV ~ [-1, 1] -> [0, A] where A intuitively should be the actual scale

                //X
                exists += sbTickLine(p - float2(0., _Thickness*.5), float2(1., 0.), _TickCount.x, _Scale.x, _Thickness, _TickDim.x);
                exists += sbETri(p - float2(_Scale.x, 0.), float2(1., 0.), _TriangleDim);

                //Y
                exists += sbTickLine(p - float2(_Thickness * .5, 0.), float2(0., 1.), _TickCount.y, _Scale.y, _Thickness, _TickDim.y);
                exists += sbETri(p - float2(0., _Scale.y), float2(0., 1.), _TriangleDim);

                exists = min(exists, 1.);

                return exists * _Color;
            }
            ENDCG
        }
    }
}
