using System.Collections;
using Ramsey.Drawing;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.UI
{
    public static class CheckMeantEscape 
    {
        private static Coroutine coro;

        public static void EscapePressed(RealtimeGameBehavior behaviour, InputData input)
        {
            IEnumerator CheckMeantEscape()
            {
                var startTime = Time.time;
                UnityReferences.ConfirmMenuText.gameObject.SetActive(true);

                while(Time.time - startTime < 2)
                {
                    yield return null;

                    var col = UnityReferences.ConfirmMenuText.color;
                    col.a = 1 - Mathf.Pow((Time.time - startTime) / 2, 2);

                    UnityReferences.ConfirmMenuText.color = col;

                    if(input.escape)
                    {
                        UnityReferences.ConfirmMenuText.gameObject.SetActive(false);
                        coro = null;

                        behaviour.ExitToMenu();
                        yield break;
                    }
                }

                UnityReferences.ConfirmMenuText.gameObject.SetActive(false);
                coro = null;
            }

            if(coro is null) coro = Coroutines.Start(CheckMeantEscape);
        }
    }
}