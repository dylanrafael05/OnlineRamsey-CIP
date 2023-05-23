Shader "Unlit/GraphShaders/RecordingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1., 1., 1., 1.)

        _PrickAmount ("Prick Amount", Float) = 1.
        _PrickSelectID ("Prick Select ID", Float) = 0.
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

            float4 _Color;

            //Everything is init space [-1, 1]

            float2 _TriX = float2(.7, .9); //[Bottom X, Top X]
            float _TriHeight = .8;

            float _BarThickness = 1.; //[0, 2]

            float2 _PrickDim = float2(.08, 1.8); //dimensions - is there a better name for this
            float _PrickZoneX = .67;
            float _PrickAmount;
            float _PrickSelectID;

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(vOut i) : SV_Target
            {
                float2 ri = float2(1., 0.);
                float2 up = float2(0., 1.);

                float2 p = i.uv;
                p = float2(dot(p, ri), dot(p, up));
                
                //Tri
                float2 tp = abs(p);
                float m = (_TriX.y - _TriX.x) / _TriHeight;
                float ch = _TriHeight - m * tp.y;
                float isTri = step(tp.x, ch) * step(_TriX.x, tp.x);

                //Bar
                float isBar = step(abs(p.y), _BarThickness * .5) * step(abs(p.x), _TriX.x);

                //Pricks
                float2 pp = p;
                pp.x = pp.x / _PrickZoneX; float isPrick = step(abs(pp.x), 1.);
                pp.x = fmod(pp.x + 1., 2. / (_PrickAmount)) - 1. / (_PrickAmount); 
                float id = _PrickAmount * .5f * (p.x - fmod(p.x + 1., 2. / _PrickAmount)); float isSelected = step(abs(id - _PrickSelectID), 0.);
                pp.y = abs(pp.y);
                _PrickDim *= (1.+isSelected*.1);
                isPrick *= step(pp.x, _PrickDim.x * .5) * step(pp.y, _PrickDim.y * .5);
                
                //Composite
                float exist = 1. - (1. - isPrick) * (1. - isBar) * (1. - isTri);

                return exist * _Color;
            }
            ENDCG
        }
    }
}
