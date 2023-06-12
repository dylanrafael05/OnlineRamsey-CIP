using System.Collections;
using Ramsey.Board;
using Ramsey.Drawing;
using Ramsey.Utilities;
using UnityEngine;

using Text = TMPro.TMP_Text;

namespace Ramsey.UI
{
    public static class GameOverHandler
    {
        private static Text GameObject => UnityReferences.OverText;

        public static IEnumerator ShowRoutine() 
        {
            const float Len = 3.7f;
            const float Size = 100f;

            var start = Time.time;

            while((Time.time - start) < Len)
            {
                var t = (Time.time - start) / Len;

                var f = 1 - (1 - t) * (1 - t);

                UnityReferences.OverText.fontSize = Size * f;
                UnityReferences.OverText.color = new Color(
                    UnityReferences.OverText.color.r, 
                    UnityReferences.OverText.color.g, 
                    UnityReferences.OverText.color.b, 
                    Mathf.Clamp01((1-f)*3));

                yield return null;
            }

            UnityReferences.OverText.gameObject.SetActive(false);
        }

        public static void Display(GameState state)
        {
            GameObject.text = state.IsGameWon ? "Game Over" : "Graph too Large";
            GameObject.gameObject.SetActive(true);

            Coroutines.StartCoroutine(ShowRoutine);
            
            UnityReferences.ScreenMaterial.SetFloat("_TimeStart", Time.timeSinceLevelLoad);
        }
    }
}