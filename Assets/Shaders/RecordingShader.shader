Shader "Unlit/UIShaders/RecordingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1., 1., 1., 1.)

        _xScale ("X Scale", Float) = 1.0

        _PrickAmount ("Prick Amount", Float) = 1.
        _PrickSelectID ("Prick Select ID", Float) = 0.

        _TriX ("Tri X", Vector) = (0.5, 0.9, 0.0, 0.0)
        _TriHeight ("Tri Height", Float) = 0.8

        _BarThickness ("Bar Thickness", Float) = .2

        _PrickDim ("Prick Dimensions", Vector) = (.11, 1.0, 0.0, 0.0)
        _PrickZoneX ("Prick Zone X", Float) = .45
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

            float _xScale;

            float4 _Color;

            //Everything is init space [-1, 1]

            float2 _TriX;
            float _TriHeight;

            float _BarThickness; //[0, 2]

            float2 _PrickDim;
            float _PrickZoneX;
            float _PrickAmount;
            float _PrickSelectID;

            #define PI 3.1415926

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(vOut i) : SV_Target
            {
                i.uv.y *= 2.;

                float o = sin(_Time.y*2.)*.07;
                float2 ri = float2(cos(o), sin(o));
                float2 up = float2(cos(o+PI*.5), sin(o+PI*.5));

                float2 p = i.uv*float2(_xScale,1.0);
                p = float2(dot(p, ri), dot(p, up));
                p.x *= .5;

                //Rescale
                _PrickZoneX *= _xScale*.5;
                _TriX = float2(_TriX.x*_xScale*.5, _TriX.x*_xScale*.5 + (_TriX.y - _TriX.x));
                _PrickDim.x = _PrickDim.x*(.7+.3*exp(-.4*(_xScale-2.)))*2.0/_xScale;
                
                //Tri
                float2 tp = abs(p);
                float m = (_TriX.y - _TriX.x) / _TriHeight;
                float ch = _TriX.y - m * tp.y;
                float isTri = step(tp.x, ch) * step(_TriX.x, tp.x);

                //Bar
                float isBar = step(abs(p.y), _BarThickness * .5) * step(abs(p.x), _TriX.x);

                //Pricks
                float2 pp = p;
                pp.x = pp.x / _PrickZoneX; float isPrick = step(abs(pp.x), 1.);
                pp.x = fmod(pp.x + 1., 2. / (_PrickAmount)) - 1. / (_PrickAmount); 
                float id = _PrickAmount * .5f * (p.x/_PrickZoneX + 1. - fmod(p.x/_PrickZoneX + 1., 2. / _PrickAmount)); float isSelected = step(abs(id - _PrickSelectID), 0.0001);
                pp.y = abs(pp.y);
                _PrickDim *= (1. + .2*float2(isSelected*1.5, isSelected));
                isPrick *= step(abs(pp.x), _PrickDim.x * .5) * step(abs(pp.y), _PrickDim.y * .5);
                
                //Composite
                float exist = 1. - (1.- isPrick) * (1. - isBar) * (1. - isTri);

                return exist * _Color;
            }
            ENDCG
        }
    }
}
