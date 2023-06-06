Shader "Unlit/DataGraph"
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

            float4 _Color;
            float _Thickness;

            fixed4 frag (vOut i) : SV_Target
            {
                float exists = step(abs(i.uv.y), _Thickness);

                return exists * _Color;
            }
            ENDCG
        }
    }
}
