using Unity.Mathematics;
using UnityEngine;

using TextInput = TMPro.TMP_InputField;
using Text = TMPro.TMP_Text;
using Ramsey.Utilities.UI;
using System;

namespace Ramsey.Utilities
{

    public static class Textboxes
    {
        private static GameObject prefab;

        private static bool isSetup;
        public static GameObject InputPrefab 
        { 
            get 
            {
                if(!isSetup) Create();
                return prefab;
            }
        }

        private static RectTransform rect;

        private static void Create() 
        {
            prefab = Resources.Load<GameObject>("Prefabs/TextInput");
            rect = GameObject.Find("Menu").GetComponent<RectTransform>();
        }

        public static InputBox CreateTextbox(float2 position, float2 scale, string name, IInputVerifier verifier) 
        {
            if(!isSetup)
            {
                Create();
                isSetup = true;
            }

            var go = GameObject.Instantiate(InputPrefab);
            var trans = go.GetComponent<RectTransform>();

            trans.SetParent(rect);

            trans.position   = position.xyzV();
            trans.localPosition = new(trans.localPosition.x, trans.localPosition.y, 0f);
            trans.localScale = new Vector3(scale.x, scale.y, 1);

            //trans.SetPositionAndRotation(trans.position, Quaternion.identity);

            return new(go, name, verifier);
        }

        public static void SetPlaceholder(this TextInput ti, string value)
        {
            ti.transform.GetComponentInChildren<Text>().text = value;
        }
    }
}