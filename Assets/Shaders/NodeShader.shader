Shader "Unlit/GraphShaders/NodeShader"
{
    Properties
    {

        
        [HideInInspector] _Radius ("Radius", Float) = 1 //[0, 1] to avoid cutting
        [HideInInspector] _HighlightRadius ("Highlight Radius", Float) = 0.5
        [HideInInspector] _HighlightThickness ("HighlightThickness", Float) = 0.1

        [HideInInspector] _NodeColor ("Node Color", Color) = (0., 0., 0., 1.)
        [HideInInspector] _HighlightColor ("Highlight Color", Color) = (1., 1., 1., 1.)

        [HideInInspector] _Mouse ("Mouse", Vector) = (0., 0., 0., 0.)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

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
                float highlighted : TEXCOORD1;
                float2 mouse : TEXCOORD2;
            };

            StructuredBuffer<float2> Positions;
            StructuredBuffer<float> IsHighlighted;
            
            float2 _Mouse;

            vOut vert (vIn v, uint instanceID : SV_InstanceID)
            {
                vOut o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(v.vertex.xy + Positions[instanceID], 0., 1.));
                o.uv = v.uv;
                o.highlighted = IsHighlighted[instanceID];
                o.mouse = (float2(_Mouse.x, -_Mouse.y) - o.vertex) * 10.;

                return o;
            }

            float _Radius;
            float _HighlightThickness;
            float _HighlightRadius;

            float4 _NodeColor;
            float4 _HighlightColor;

            float warpFactor(float2 pnt, float2 towards)
            {
                // return 0.;
                return pow(exp(-pow(length(pnt - towards),2.)), 2.) * .2;
            }

            float2 warpPoint(float2 pnt, float2 towards)
            {
                return pnt + (pnt - towards) * warpFactor(pnt, towards);
            }

            fixed4 frag (vOut i) : SV_Target
            {
                /*
                float len = length(i.uv);

                float rtheta = atan2(i.uv.y, i.uv.x);
                float theta = rtheta + _Time.y;

                float hiRad = _HighlightRadius * (sin(theta * 10.0) * 0.05 + 0.95);

                return step(len, _Radius) * lerp(_HighlightColor, float4(0., 0., 0., 1), smoothstep(_Radius, hiRad, len));*/

                float r = length(i.uv);
                float isHighlight = 0.0;

                //Highlight
                float o = atan2(i.uv.y, i.uv.x);
                isHighlight = step(abs(r - _HighlightRadius+sin(o*10. + _Time.y)*.05) - _HighlightThickness*.5, 0.) * i.highlighted;

                //Node 
                float d = length(warpPoint(i.uv, i.mouse)) - (_Radius * (1. + warpFactor(i.uv, i.mouse)));
                float4 col = step(d, 0.) * _NodeColor;

                //Composite
                return lerp(col, _HighlightColor, isHighlight);
            }

            ENDCG
        }
    }
}
