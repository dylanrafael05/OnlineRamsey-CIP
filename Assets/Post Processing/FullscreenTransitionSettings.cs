using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.Rendering.PostProcessing;

namespace Ramsey.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FullscreenTransitionRenderer), PostProcessEvent.AfterStack, "Custom/FullscreenTransition", true)]
    public sealed class FullscreenTransitionSettings : PostProcessEffectSettings
    {

        //
        public BoolParameter RuntimeTransition = new() { value = true };
        [Range(0f, 1f)] public FloatParameter Interpolation = new() { value = 0f };

        public ColorParameter BaseColor = new() { value = Color.white };
        public ColorParameter StripeColor = new() { value = Color.cyan };
        public FloatParameter RadialOffset = new() { value = 0f };

        //
        public static float InterpolationOverride { get; set; } = 0f;

    }

    public sealed class FullscreenTransitionRenderer : PostProcessEffectRenderer<FullscreenTransitionSettings>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/FullscreenTransition"));

            sheet.properties.SetFloat("_Interpolation", settings.RuntimeTransition.value ? FullscreenTransitionSettings.InterpolationOverride : settings.Interpolation.value);

            sheet.properties.SetColor("_BaseColor", settings.BaseColor.value);
            sheet.properties.SetColor("_StripeColor", settings.StripeColor.value);

            sheet.properties.SetFloat("_RadialOffset", settings.RadialOffset.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}

