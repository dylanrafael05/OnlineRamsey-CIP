Shader "Unlit/UIShaders/WheelSelect2"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1., 1., 0.6, 1.)
        _NodeColor ("Node Color", Color) = (1., 1., 1., 1.)
        _RedColor ("Red Color", Color) = (1.,0.,0.,1.)
        _BlueColor ("Blue Color", Color) = (0.,0.,1.,1.)

        _WheelRadius ("Radius", Float) = 1.
        _WheelThickness ("Wheel Thickness", Float) = 0.3

        _TickCount ("Tick Count", Integer) = 2

        _KnobColor("Knob Color", Color) = (1.,1.,1.,1.)
        _KnobRadius("Knob Radius Mult", Float) = 0.15
        _KnobLocation ("Knob Location", Float) = 0
        _NodeRadius ("Node Radius", Float) = 0.1
        
        _ThetaNormalizedOffset ("Offset", Float) = 0.

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

            #define PI 3.1415926

            float4 _BaseColor;
            float4 _NodeColor;
            float4 _RedColor;
            float4 _BlueColor;

            float _WheelRadius;
            float _WheelThickness;

            int _TickCount;

            float4 _KnobColor;
            float _KnobRadius;
            float _KnobLocation;
            float _NodeRadius;

            float _ThetaNormalizedOffset;

            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float4 samplePolygonWheel(float2 p, float r, int partitionCount, float thetaOffset, float thickness)
            {

                //
                float outlineThickness = .01;

                //
                float exists = 0.;
                float isNode = 0.;

                //
                float partitionSize = TAU / float(partitionCount);

                thetaOffset += partitionSize * _ThetaNormalizedOffset;// partitionSize * .5 * float((partitionCount + 1) % 2);
                float2 polar = toPolar(p); polar.y = amod(polar.y - thetaOffset, TAU);
                float rtheta = amod(polar.y, partitionSize) - partitionSize*.5;

                float2 startPoint = toCartesian(float2(r, partitionSize*.5));
                float2 endPoint = toCartesian(float2(r, -partitionSize*.5));

                float2 lp = toCartesian(float2(polar.x, rtheta));

                float sdEdge = abs(dot(lp - startPoint, normalize(perp(startPoint-endPoint)))) - .5*thickness;
                exists += step(sdEdge, 0.);
                float edgeOutline = step(-outlineThickness, sdEdge);

                //
                float altId = (amod(polar.y, partitionSize*2.) - amod(polar.y, partitionSize)) / partitionSize;
                float4 edgeCol = _BlueColor + altId * (_RedColor - _BlueColor);

                //
                float sdNode = min(length(lp - startPoint), length(lp - endPoint)) - _NodeRadius;
                float nodeOutline = step(-outlineThickness, sdNode);
                isNode += step(sdNode, 0.)*(1.-saturate(nodeOutline*exists*(1.-edgeOutline)));
                exists += isNode;

                //
                edgeCol.rgb *= 1.-edgeOutline;
                _NodeColor.rgb *= 1.-nodeOutline;

                //
                float knobTheta = _KnobLocation * partitionSize;
                float rKnobTheta = amod(knobTheta, partitionSize)-partitionSize*.5;
                float rKnob = r * cos(partitionSize*.5) / cos(rKnobTheta);
                float2 knobPos = toCartesian(float2(rKnob, knobTheta+thetaOffset));
                float sdKnob = length(p - knobPos) - _KnobRadius;
                float isKnob = step(sdKnob, 0.);
                _KnobColor.rgb *= step(sdKnob, -outlineThickness);
                exists += isKnob; //fix misalignment and collision is still a circle

                //
                exists = saturate(exists);

                return exists * ( (1.-isKnob) * (edgeCol + isNode * (_NodeColor - edgeCol)) + isKnob * _KnobColor);

            }

            fixed4 frag(vOut i) : SV_Target
            {

                float2 p = i.uv;

                return samplePolygonWheel(p, _WheelRadius, _TickCount, 0., _WheelThickness);

            }
            ENDCG
        }
    }
}
