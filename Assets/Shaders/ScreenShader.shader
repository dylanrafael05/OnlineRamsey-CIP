Shader "Unlit/Fullscreen/Pulse"
{
    Properties
    {
        [HideInInspector] _ScreenTexture ("Texture", 2D) = "red" {}
        [HideInInspector] _TimeStart ("Time Start", Float) = -100.
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

            float _TimeStart;

            float stepped(float val, float step)
            {
                return val - fmod(val, step);
            }

            float min(float2 v)
            {
                return min(v.x, v.y);
            }

            float linearstep(float start, float end, float i)
            {
                return clamp((i-start)/(end-start), 0., 1.);
            }

            fixed4 frag (vOut i) : SV_Target
            {
                //return tex2D(_ScreenTexture, 1. - i.uv);

                float2 uvc = i.uv * 2. - 1.;
                uvc.x *= 16./9.;
                
                //params temp here
                float repLen = 0.5;
                float a = 0.15;
                float s = 1.5;
                float repeatAmount = 8.0;

                float r = length(uvc);
                
                float rr = length(float2(16. / 9., 1.)) - r + s * (_Time.y - _TimeStart);
                float d = fmod(rr, repLen);
                float offset = length(float2(16. / 9., 1.));// stepped(length(float2(16. / 9., 1.)), repLen) + repLen;
                d *= step(offset, rr) *step(rr, offset + repeatAmount * repLen);
                d = d/repLen * 2. - 1.;
                r += a * d * pow(1.-abs(d), 2.0);

                //if(fmod(-r + s*_Time.y+100., repLen) < .01) return float4(1.0, 1.0, 1.0, 1.0);

                float o = atan2(uvc.y, uvc.x);
                float4 col = tex2D(_ScreenTexture, 1. - (float2(r * cos(o), r * sin(o))*float2(9./16., 1.0)*.5+.5));
                col = lerp(col, float4(0.,0.,0.,1.), .2*linearstep(.15, .0, pow(min(float2(16./9., 1.) - abs(uvc)),2.0)));
                return col;
            }
            ENDCG
        }
    }
}
