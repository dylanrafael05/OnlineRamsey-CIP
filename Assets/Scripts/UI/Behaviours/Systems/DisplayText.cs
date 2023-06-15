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

        public static void Update(GameState state)
        {
            TurnNumber.text = "" + state.TurnNum;
        }
    }
}