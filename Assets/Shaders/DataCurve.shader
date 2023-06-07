Shader "Unlit/DataCurve"
{
    Properties
    {
        _Color ("Color", Color) = (1., 0., 0., 1.)
        _Thickness ("Thickness", Float) = 1.

        _Scale ("Scale", Vector) = (1., 1., 0., 0.)
        _SizeBounds ("Scale Bounds", Vector) = (10., 10., 0., 0.)
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
                float2 graphSpacePos : TEXCOORD1;
            };

            float2 _Scale;
            float2 _SizeBounds;

            vOut vert (vIn v)
            {
                vOut o;
                o.graphSpacePos = v.vertex.xy * _Scale;
                o.vertex = UnityObjectToClipPos(v.vertex * float4(_Scale.x, _Scale.y, 1., 1.));
                o.uv = v.uv;
                return o;
            }

            float2 amod(float2 p, float2 rep)
            {

                float2 s = sign(p);
                p = fmod(abs(p), rep);
                p = p + step(s, 0.) * (rep - 2. * p);
                return p;

            }

            float min(float2 v)
            {
                return min(v.x, v.y);
            }

            float4 _Color;
            float _Thickness;

            fixed4 frag(vOut i) : SV_Target //Not abs() graphPos rn cuz assumption is it's potential p ~ [0, inf)
            {
                float size = .04;
                float ditherStart = 0.4;

                float2 p = i.graphSpacePos;
                p = amod(p+size*.5, size)-size*.5;
                float sdBound = min(_SizeBounds - i.graphSpacePos);
                float r = smoothstep(ditherStart, 0., sdBound)* size * .9;
                float sd = length(p) - r;

                float exists = step(abs(i.uv.y), _Thickness) * smoothstep(0., ditherStart, sdBound) * step(0., sd);
                
                //if(abs(i.uv.y) <= .050) return float4(0.,0.,0.,1.);

                return exists* _Color;
            }
            ENDCG
        }
    }
}
