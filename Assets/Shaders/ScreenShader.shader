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
                //return tex2D(_ScreenTexture, 1. - i.uv);

                float2 uvc = i.uv * 2. - 1.;
                uvc.x *= 16./9.;
                
                //params temp here
                float repLen = 0.5;
                float a = 0.15;
                float s = 1.0;

                float r = length(uvc);
                
                float d = (fmod(-r+s*_Time.y+length(float2(16./9., 1.)), repLen)/repLen)*2.-1.;
                r += a * d * pow(1.-abs(d), 2.0);

                //if(fmod(-r + s*_Time.y+100., repLen) < .01) return float4(1.0, 1.0, 1.0, 1.0);

                float o = atan2(uvc.y, uvc.x);
                return tex2D(_ScreenTexture, 1. - (float2(r * cos(o), r * sin(o))*float2(9./16., 1.0)*.5+.5));
            }
            ENDCG
        }
    }
}
