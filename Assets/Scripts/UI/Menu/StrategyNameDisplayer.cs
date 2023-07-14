using Ramsey.Drawing;
using Unity.Mathematics;
using UnityEngine;

namespace Ramsey.UI
{

    public static class StrategyNameDisplayer
    {

        public static void UpdateName(string typeName, bool painter)
        {

            var textObj = painter ? UnityReferences.PainterNameText : UnityReferences.BuilderNameText;
            var text = textObj.text;

            var cut = GetCutIndices(text);
            text = text.Remove(cut.x, cut.y - cut.x);
            text = text.Insert(cut.x, typeName + "                                ");

            textObj.text = text;
            textObj.transform.localScale = Vector3.one * math.lerp(2.401f, 1.4f, math.smoothstep(7f, 13f, typeName.Length * 1f));

            //EXTREMELY BANDAID
            textObj.transform.localPosition = new(textObj.transform.localPosition.x, 244.4f + (typeName == "Constrained Random" ? 20f : 0f), textObj.transform.localPosition.z);

        }

        static int2 GetCutIndices(string text) 
            => new(text.IndexOf('>') + 1, text.IndexOf('<', 1));

    }

}