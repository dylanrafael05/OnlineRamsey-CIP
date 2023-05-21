Shader "Unlit/RecordingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1., 1., 1., 1.)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            //Everything is init space [-1, 1]

            float2 _TriX; //[Bottom X, Top X]
            float _TriHeight;

            float _BarThickness; //[0, 2]

            float2 _PrickDim; //dimensions - is there a better name for this

            v2f vert (vOut v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //p = uv (-1, 1) get the abs of p x and y then do that half triangle thing and do the sdf of it isUnderTri && isOverY=0
                //then union it with all the other stuff the big line and modulate the p.x for the pricks
                //purely sdf given by a quad, can also give a special prick ID and when modulating, use our normal techniques to change that special prick
                //make it larger or change its color idk whatever

                float2 ri = float2(1., 0.);
                float2 up = float2(0., 1.);

                float2 p = uv;
                p = float2(dot(p, ri), dot(p, up));
                
                //Tri
                float2 tp = abs(p);
                float m = (_TriX.y - _TriX.x) / _TriHeight;
                float ch = _TriHeight - m * tp.y;
                float isTri = step(tp.x, ch) * step(_TriX.x, tp.x);

                //Bar
                float isBar = step(abs(p.y), _BarThickness * .5) * step(abs(p.x), _TriX.x);

                //Pricks
                //float2 pp = later

                return float4(1., 1., 1., 1.);
            }
            ENDCG
        }
    }
}
