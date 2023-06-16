Shader "Hidden/Custom/FullscreenTransition"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        #include "../Shaders/Common.hlsl"

        #define PI 3.14159265

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

        sampler2D _DitherTexture;

        float _Interpolation;
        
        float4 _BaseColor;
        float4 _StripeColor;
        float _RadialOffset;
       
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            
            //
            float3 sampleCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;

            //
            float repLen = .2+_Interpolation*.2;
            float startSize = -.2;
            float randMag = .1;

            //
            float2 p = (i.texcoord*2.-1.)*float2(16./9.,1.);

            //
            float o = _Time.y;
            float2 fo = float2(cos(o), sin(o));
            float2 up = float2(cos(o+PI*.5), sin(o+PI*.5));
            p = float2(dot(p, fo), dot(p, up));

            float2 lp = amod(p+repLen*.5, repLen)-repLen*.5;
            float2 id = p-lp;

            //
            float size = startSize + (eps.x + repLen - startSize + randMag) * _Interpolation;
            size += randMag*(hash21(id/repLen+eps.xx)*2.-1.);// size += 10.;
            //TODO: make individual squares rotate into place as per a hash which determines the start rot since that'll also depetermint the speed'
            //
            lp = abs(lp);
            float sd = sqrt(pow(max(0., lp.x - size*.5), 2.0) + pow(max(0., lp.y - size*.5), 2.0));
            sd += step(sd, 0.) * max(lp - size);

            //
            float repLenRadial = .5*5.*1.5;
            float stripeSize = .25*5.*1.5;
            float smoothThickness = .25*5.*1.5;
            
            float r = 3.*_Interpolation + _RadialOffset + _Time.y + length(float2(16./9., 1.)) - length(id);
            float lr = abs(fmod(r, repLenRadial)-repLenRadial*.5);
            float stripe = smoothstep((stripeSize+smoothThickness)*.5, (stripeSize-smoothThickness)*.5, lr);
            float3 hitCol = lerp(_BaseColor, _StripeColor, stripe).rgb;

            //
            float hit = step(sd, 0.);
            return float4(lerp(sampleCol, hitCol, hit), 1.0);

        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}