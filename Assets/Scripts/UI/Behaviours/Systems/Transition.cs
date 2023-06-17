using System.Collections;
using Ramsey.Drawing;
using UnityEngine;
using Ramsey.PostProcessing;
using Unity.Mathematics;

namespace Ramsey.UI
{
    public static class Transition 
    {
        public const float Duration = 0.5f;
        static bool running;

        public static IEnumerator HideScreen()
        {
            if (running) yield break;
            running = true;

            var starttime = Time.time;

            while(Time.time - starttime < Duration)
            {
                var p = (Time.time - starttime) / Duration;
                FullscreenTransitionSettings.InterpolationOverride = math.pow(p, 1.7f);

                yield return null;
            }

            FullscreenTransitionSettings.InterpolationOverride = 1f;

            running = false;
        }

        public static IEnumerator ShowScreen()
        {
            if (running) yield break;
            running = true;

            var starttime = Time.time;

            while(Time.time - starttime < Duration)
            {
                var p = (Time.time - starttime) / Duration;
                FullscreenTransitionSettings.InterpolationOverride = (1f-math.pow(p, 1.7f));

                yield return null;
            }

            FullscreenTransitionSettings.InterpolationOverride = 0f;

            running = false;
        }
    }
}