Shader "Unlit/Screen/MenuBackground2"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.,0.,0.,1.)
        _HighlightColor ("Highlight Color", Color) = (1.,1.,1.,1.)
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            vOut vert (vIn v)
            {
                vOut o;
                o.vertex = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, v.vertex));
                o.uv = v.uv;
                return o;
            }

            float4 _BaseColor;
            float4 _HighlightColor;
            float cover;

            #define highlightDim float2(0.07, 0.015)
            #define highlightDomainDim float2(0.12, 0.08)

            float sbHighlight(float2 p, float2 ps, float2 pe, float offset)
            {
                float2 fo = normalize(pe - ps);
                float2 up = perp(fo);
    
                p -= pe;
                p = float2(dot(p, fo)+offset, dot(p, up));
                p.x = amod(p.x + highlightDomainDim.x*.5, highlightDomainDim.x)-highlightDomainDim.x*.5;
                p.y = abs(p.y);
    
                cover = max(cover, step(p.y, highlightDomainDim.y*.5));
    
                p.y = p.y - (highlightDomainDim.y*.5 - highlightDim.y*.5);
                p = abs(p);
    
                return step(p.x, highlightDim.x*.5) * step(p.y, highlightDim.y*.5);
            }

            float sbHighlightPolygon(float2 p, float r, int partitionCount, float thetaOffset, float offset)
            {
                float partitionSize = TAU / float(partitionCount);
    
                float2 polar = toPolar(p);
                polar.y += partitionSize*.5-thetaOffset;
                float rtheta = amod(polar.y, partitionSize)-partitionSize*.5;
                p = toCartesian(float2(polar.x, rtheta));
    
                float id = (polar.y - amod(polar.y, partitionSize)) / partitionSize;
    
                float2 rPoints = float2(r, r); //temp
    
                float2 pStart = toCartesian(float2(rPoints.x, -partitionSize*.5));
                float2 pEnd = toCartesian(float2(rPoints.y, partitionSize*.5));
    
                cover = max(cover, step(0., dot(p - pStart, perp(pEnd - pStart))));
    
                return sbHighlight(p, pStart, pEnd, offset+id*length(pStart - pEnd));
    
            }

            fixed4 frag(vOut i) : SV_Target
            {
                float t = _Time.y*1.;

                i.uv = 1.-i.uv;
                float2 p = (i.uv*2.-1.)*float2(16./9., 1.);
    
                float exists = 0.;
                cover = 0.;
    
                float4 polyPoint = float4(.67, 0., .70, t * .9);
                exists += (1.-cover) * sbHighlightPolygon(p - polyPoint.xy, polyPoint.z, 8, polyPoint.a, t);
    
                exists += (1.-cover) * sbHighlight(p, float2(-1.,-1.), float2(1.,1.), -t*.8);
                exists += (1.-cover) * sbHighlight(p, float2(1., -1.), float2(-1., 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(-.5, -1.), float2(-.2, .8), t);
                exists += (1.-cover) * sbHighlight(p, float2(1.2, -.7), float2(.3, 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.9, -.2), float2(1., .3), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.1, 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(.7, -1.), float2(0., 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(.1, -1.), float2(-1.5, .8), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.6, .4), float2(1., -.4), t);
                exists += (1.-cover) * sbHighlight(p, float2(.4, -1.), float2(1.4, .8), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.4, -1.), float2(1., .2), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.77, -.8), float2(-.6, 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(1.05, -1.), float2(1.62, -.1), t);
                exists += (1.-cover) * sbHighlight(p, float2(1.77, -.25), float2(-.9, .9), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.77, -.5), float2(1.77, -.8), t);
                exists += (1.-cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.8, 1.), t);
                exists += (1.-cover) * sbHighlight(p, float2(-.72, -.8), float2(-1.5, 1.), t);

                p = rot(p+0.8, 2.); //smth like it but less unorganized and get the entire screen and not so uniform of course.. and prolly too much arund bottom left


                exists += (1. - cover) * sbHighlight(p, float2(-1., -1.), float2(1., 1.), -t * .8);
                exists += (1. - cover) * sbHighlight(p, float2(1., -1.), float2(-1., 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-.5, -1.), float2(-.2, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.2, -.7), float2(.3, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.9, -.2), float2(1., .3), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.1, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(.7, -1.), float2(0., 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(.1, -1.), float2(-1.5, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.6, .4), float2(1., -.4), t);
                exists += (1. - cover) * sbHighlight(p, float2(.4, -1.), float2(1.4, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.4, -1.), float2(1., .2), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.77, -.8), float2(-.6, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.05, -1.), float2(1.62, -.1), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.77, -.25), float2(-.9, .9), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.77, -.5), float2(1.77, -.8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.8, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-.72, -.8), float2(-1.5, 1.), t);

                p = rot(p - 1.8, -2.);


                exists += (1. - cover) * sbHighlight(p, float2(-1., -1.), float2(1., 1.), -t * .8);
                exists += (1. - cover) * sbHighlight(p, float2(1., -1.), float2(-1., 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-.5, -1.), float2(-.2, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.2, -.7), float2(.3, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.9, -.2), float2(1., .3), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.1, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(.7, -1.), float2(0., 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(.1, -1.), float2(-1.5, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.6, .4), float2(1., -.4), t);
                exists += (1. - cover) * sbHighlight(p, float2(.4, -1.), float2(1.4, .8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.4, -1.), float2(1., .2), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.77, -.8), float2(-.6, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.05, -1.), float2(1.62, -.1), t);
                exists += (1. - cover) * sbHighlight(p, float2(1.77, -.25), float2(-.9, .9), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.77, -.5), float2(1.77, -.8), t);
                exists += (1. - cover) * sbHighlight(p, float2(-1.3, -1.), float2(-1.8, 1.), t);
                exists += (1. - cover) * sbHighlight(p, float2(-.72, -.8), float2(-1.5, 1.), t);


                exists *= 4.;
                _HighlightColor.rgb = 0.;
                return _BaseColor + exists * (_HighlightColor - _BaseColor); 
            }
            ENDCG
        }
    }
}