Shader "Unlit/GraphShaders/EdgeShader"
{
    Properties
    {
        [HideInInspector] _Thickness("Thickness", Float) = 0.15

        [HideInInspector] _HighlightColor("Highlight Color", Color) = (1., 1., 1., 1.)
        [HideInInspector] _HighlightSize("Highlight Size", Float) = 0.2
        [HideInInspector] _HighlightRepLength("Highlight Repeat Length", Float) = 0.2
        [HideInInspector] _HighlightThickness("Highlight Thickness", Float) = 0.05

        // [PerRendererData] _Transforms ("Transforms", Matrix) = 0.
        [PerRendererData] _Colors ("Colors", Color) = (1., 1., 1., 1.)
        [PerRendererData] _IsHighlighted ("IsHighlighted", Float) = 0.
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Geometry" "IgnoreProjector" = "True" }
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

            UNITY_INSTANCING_BUFFER_START(EdgeProperties)
                UNITY_DEFINE_INSTANCED_PROP(float4x4, _Transforms)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Colors)
                UNITY_DEFINE_INSTANCED_PROP(float, _IsHighlighted)
            UNITY_INSTANCING_BUFFER_END(props)

            float _Thickness;

            float4 _HighlightColor;
            float _HighlightThickness;
            float _HighlightSize; //[0, 1]
            float _HighlightRepLength;

            struct vIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct vOut
            {
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float isHighlighted : TEXCOORD2;
                float length : TEXCOORD3;
            };

            float3 worldScale(float4x4 m) { return float3(
                length(float3(m[0].x, m[1].x, m[2].x)), // scale x axis
                length(float3(m[0].y, m[1].y, m[2].y)), // scale y axis
                length(float3(m[0].z, m[1].z, m[2].z))  // scale z axis
            ); }

            vOut vert (vIn v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                vOut o;

                float4x4 transform = UNITY_ACCESS_INSTANCED_PROP(props, _Transforms);

                o.vertex = mul(UNITY_MATRIX_VP, mul(transform, v.vertex));
                o.color = UNITY_ACCESS_INSTANCED_PROP(props, _Colors);
                o.uv = v.uv;
                o.isHighlighted = UNITY_ACCESS_INSTANCED_PROP(props, _IsHighlighted);
                o.length = worldScale(transform);

                return o;
            }

            fixed4 frag(vOut i) : SV_Target
            {
                //Edge
                float4 edgeCol = i.color * step(abs(i.uv.y), _Thickness*.5);

                //Highlight
                float2 p = i.uv; //make sure -1 to 1
                p.y = abs(p.y);
                p.x = (p.x + 1.0)*i.length+_Time.y*2.;
                p.y = abs(p.y - _HighlightSize);
                float highlight = i.isHighlighted * step(p.y, _HighlightThickness) * step(0., fmod(p.x, _HighlightRepLength*2.0)-_HighlightRepLength);

                return (edgeCol + (_HighlightColor - edgeCol) * highlight);
            }
            ENDCG
        }
    }
}
