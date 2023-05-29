Shader "Unlit/Fullscreen/Vignette"
{
    Properties
    {
        _ScreenTexture ("Texture", 2D) = "red" {}
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

            sampler2D _ScreenTexture;
            
            vOut vert (vIn v)
            {
                vOut o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (vOut i) : SV_Target
            {
                float2 uvc = i.uv * 2. - 1.;
                
                // float r = length(uvc);

                // uvc = uvc + float2(_SinTime.w / 10. + sin(r * 20.) / 40., _CosTime.w / 10. + sin(r * 20.) / 40.);

                return tex2D(_ScreenTexture, 1. - (uvc * .5 + .5));
            }
            ENDCG
        }
    }
}
