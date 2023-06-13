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
        private static int count;
        private static bool isSetup;

        private static Canvas canvas;
        private static GameObject textPrefab;
        private static List<Text> texts = new();

        private static void Create()
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
            if(count >= texts.Count)
            {
                texts.Add(CreateText());
            }

            texts[count].transform.position = new float3(position, -3f);
            texts[count].transform.localScale = new(1, 1, 1);
            texts[count].color = Color.black;
            texts[count].text = content;
            texts[count].gameObject.SetActive(true);

            count++;
        }

        public static void Begin()
        {
            if(!isSetup)
            {
                Create();
                isSetup = true;
            }
            
            count = 0;
        }

        public static void End()
        {
            for(int i = count; i < texts.Count; i++)
            {
                texts[i].gameObject.SetActive(false);
            }
        }
    }
}