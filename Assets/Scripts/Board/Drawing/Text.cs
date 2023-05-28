using System.Collections.Generic;
using Ramsey.Utilities;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TMP_Text;

namespace Ramsey.Drawing
{
    public static class TextRenderer
    {
        private static int lastUsedIndex;
        private static Canvas canvas;
        private static GameObject textPrefab;
        private static List<Text> texts = new();

        public static void Create()
        {
            canvas = GameObject.Find("Board Canvas").GetComponent<Canvas>();
            textPrefab = Resources.Load<GameObject>("Prefabs/TextPrefab");
        }

        private static Text CreateText()
        {
            var newgo = GameObject.Instantiate(textPrefab, Vector3.zero, Quaternion.identity);
            newgo.transform.SetParent(canvas.transform);

            return newgo.GetComponent<Text>();
        }

        public static void Draw(float2 position, string content) 
        {
            if(lastUsedIndex >= texts.Count)
            {
                texts.Add(CreateText());
            }

            texts[lastUsedIndex].transform.position = new float3(position, -3f);
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