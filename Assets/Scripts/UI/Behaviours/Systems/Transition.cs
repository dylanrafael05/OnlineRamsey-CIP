using System.Collections;
using Ramsey.Drawing;
using UnityEngine;

namespace Ramsey.UI
{
    public static class Transition 
    {
        public const float Duration = 0.5f;

        public static IEnumerator HideScreen()
        {
            var starttime = Time.time;

            while(Time.time - starttime < Duration)
            {
                var p = (Time.time - starttime) / Duration;
                UnityReferences.TransitionRenderer.material.SetFloat("_Progress", p);

                yield return null;
            }
            
            UnityReferences.TransitionRenderer.material.SetFloat("_Progress", 1);
        }

        public static IEnumerator ShowScreen()
        {
            var starttime = Time.time;

            while(Time.time - starttime < Duration)
            {
                var p = (Time.time - starttime) / Duration;
                UnityReferences.TransitionRenderer.material.SetFloat("_Progress", 1-p);

                yield return null;
            }
            
            UnityReferences.TransitionRenderer.material.SetFloat("_Progress", 0);
        }
    }
}