// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Screen/MenuBackground"
{
    Properties
    {

        _BaseColor ("Base Color", Color) = (1.,1.,1.,1.)
        _HighlightColor ("Highlight Color", Color) = (0.,0.,0.,1.)

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _HighlightColor;

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, v.vertex));// + float4(_WorldSpaceCameraPos.xy, 0., 0.));
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (vOut i) : SV_Target
            {

                i.uv = 1. - i.uv;

                float2 p = (i.uv*2.-1.)*float2(16./9., 1.);
                p -= float2(.67, 0.);
                float exists = 0.;

                // Polar circle

                float radius = 0.8;
                float3 trigParams = float3(10.*.8, .2*.3, 2.);
                float thickness = .05;

                float r = radius + trigParams.y * sin(trigParams.x * atanP(p) + _Time.y * trigParams.z);
                float R = length(p);

                float withinCircle = step(R - r, thickness*.5);
                exists += step(abs(R - r), thickness*.5);

                // Highlights coming out
                float legCount = 16.;
                float2 highlightSeparation = float2(.05, .05);
                float2 highlightDim = float2(.1, .05);
                float highlightSpeed = .4;

                float partitionSize = TAU / legCount;

                float2 polar = toPolar(p);
                polar.y += -polar.x*.1;
                float rtheta = abs(fmod(polar.y, partitionSize) - partitionSize*.5);

                float2 lp = toCartesian(float2(polar.x, rtheta));
                lp.x = amod(lp.x-highlightSpeed*_Time.y, highlightDim.x + highlightSeparation.x) - (highlightDim + highlightSeparation).x*.5;
                lp.y = lp.y - (highlightSeparation+highlightDim).y*.5;// - .02 * (sin(polar.x*10.)*.5+.5);
                lp.x -= .5*(highlightDim.y*.5 - abs(lp.y));
                lp = abs(lp);
                
                exists += (1.-withinCircle) * step(lp.x, highlightDim.x*.5) * step(lp.y, highlightDim.y * .5);

                return _BaseColor + exists * (_HighlightColor - _BaseColor);
            }

            ENDCG
        }
    }
}
