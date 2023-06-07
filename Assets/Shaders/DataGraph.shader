Shader "Unlit/DataGraph"
{
    Properties //massive quad we'll cut UV ~ [-A, A]
    {

        _Color ("Color", Color) = (0.,0.,0.,1.)

        _TickCount ("Tick Count", Vector) = (4., 4., 0., 0.) //[xTick, yTick]
        _Scale ("Scale", Vector) = (1., 1., 0., 0.) //[xScale, yScale]
        _Thickness ("Thickness", Float) = 1.
        _Triangle ("Triangle Dimensions", Vector) = (1., 1., 0., 0.) //[width, height]

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
            };

            vOut vert (vIn v)
            {
                vOut o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(vOut i) : SV_Target 
            {
                return float4(1.,1.,1.,1.);
            }
            ENDCG
        }
    }
}
