using System.Collections;
using Ramsey.Board;
using Ramsey.Drawing;
using Ramsey.Utilities;
using UnityEngine;

using Text = TMPro.TMP_Text;

namespace Ramsey.UI
{
    public static class DisplayText
    {
        private static Text TurnNumber => UnityReferences.TurnText;
        private static Text GoalText => UnityReferences.GoalText;

        public static void Update(GameState state)
        {
            GoalText.richText = true;

            TurnNumber.text = "" + state.TurnNum;
            GoalText.text = $"<color=#D0D0D0><size=70%>{state.MaxPath?.Length ?? 0}/</size></color>{state.TargetPathLength}";
        }
    }
}