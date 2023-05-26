using System.Collections.Generic;
using Ramsey.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Ramsey.Drawing
{
    public static class TextDrawer
    {
        private static int lastUsedIndex;
        private static Canvas canvas;
        private static List<Text> texts = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            var go = new GameObject("_textCanvas", typeof(Canvas));
            GameObject.DontDestroyOnLoad(go);
            go.layer = LayerMask.NameToLayer("Board");

            canvas = go.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.WorldSpace;
        }

        public static void Draw(float2 position, string content) 
        {
            if(lastUsedIndex >= texts.Count)
            {
                var go = new GameObject("_text"+lastUsedIndex, typeof(Text));
                GameObject.DontDestroyOnLoad(go);
                go.layer = LayerMask.NameToLayer("Board");

                go.transform.SetParent(canvas.transform, true);

                texts.Add(go.GetComponent<Text>());
            }

            texts[lastUsedIndex].transform.position = new float3(position, 0.2f);
            texts[lastUsedIndex].transform.localScale = new(1, 1, 1);
            texts[lastUsedIndex].color = Color.black;
            texts[lastUsedIndex].text = content;
            texts[lastUsedIndex].gameObject.SetActive(true);

            lastUsedIndex++;
        }

        public static void Flush()
        {
            lastUsedIndex = 0;

            foreach(var t in texts)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}